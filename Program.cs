using GFDecompress.STC;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;

namespace GFDecompress
{
    class Program
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        #region .dat 복호화

        // .dat 복호화
        public static string DatFileDecompress(byte[] data, byte[] key)
        {
            byte[] temp = Xor(data, key);
            byte[] output;
            using (var compressedStream = new MemoryStream(temp))
            using (var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
            using (var resultStream = new MemoryStream())
            {
                zipStream.CopyTo(resultStream);
                output = resultStream.ToArray();
            }
            return Encoding.UTF8.GetString(output);
        }

        public static byte[] Xor(byte[] text, byte[] key)
        {
            byte[] xor = new byte[text.Length];
            for (int i = 0; i < text.Length; i++)
            {
                xor[i] = (byte)(text[i] ^ key[i % key.Length]);
            }
            return xor;
        }

        #endregion

        #region .stc 파싱

        // .stc 파싱
        public static JArray ParseStc(string stcFile, int startOffset = 0)
        {
            log.Info(".stc parse >> {0}", stcFile);

            JArray output = new JArray();

            // stc 읽기
            byte[] stcStream = File.ReadAllBytes(stcFile);
            StcBinaryReader reader = new StcBinaryReader(stcStream);

            int code = reader.ReadUShort();         // 예: 5005
            int unknown = reader.ReadUShort();      // ??
            log.Debug("file: {0}, code: {1}, unknown: {2}", stcFile, code, unknown);

            int row = reader.ReadUShort();
            int col = reader.ReadByte();

            if (row > 0 && col > 0)
            {
                // 컬럼별 크기
                List<string> colSizes = new List<string>();
                for (int i = 0; i < col; i++)
                {
                    int size = reader.ReadByte();
                    switch (size)
                    {
                        case 5:
                            colSizes.Add(i + ":" + "Integer");
                            break;
                        case 11:
                            colSizes.Add(i + ":" + "String");
                            break;
                        default:
                            colSizes.Add(i + ":" + "Unknown");
                            break;
                    }
                }
                log.Debug("column_info >> {0}", string.Join("|", colSizes));

                // 실제 정보가 있는 오프셋으로 이동
                reader._offset = startOffset;

                try
                {
                    for (int i = 0; i < row; i++)
                    {
                        switch (stcFile)
                        {
                            // GunList
                            case "5005.stc":
                                output.Add(JObject.FromObject(new Gun(reader)));
                                break;
                            // SquadList
                            case "5006.stc":
                                output.Add(JObject.FromObject(new Squad(reader)));
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }

            return output;
        }

        #endregion

        static void Main(string[] args)
        {
            #region NLog Configuration
            var config = new LoggingConfiguration();

            var logconsole = new ColoredConsoleTarget("logconsole");
            logconsole.Layout = new SimpleLayout() { Text = "${message} ${exception:format=tostring}" };
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logconsole);

            LogManager.Configuration = config;
            #endregion

            Console.OutputEncoding = Encoding.UTF8;

            // catchdata.dat 복호화
            try
            {
                // 복호화
                log.Info(".dat decrypt >> {0}", "catchdata.dat");
                byte[] data = File.ReadAllBytes("catchdata.dat");
                byte[] key = Encoding.ASCII.GetBytes("c88d016d261eb80ce4d6e41a510d4048");
                string output = DatFileDecompress(data, key);

                // 폴더 생성
                if (!Directory.Exists("output_catchdata"))
                    Directory.CreateDirectory("output_catchdata");
                File.WriteAllText("output_catchdata\\catchdata.txt", output);

                // 정보별 추출
                string[] lines = output.Split('\n').Select(p => p.Trim()).ToArray();
                foreach (string line in lines)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(line))
                            continue;
                        JObject json = JObject.Parse(line);
                        string jKey = json.Properties().Select(p => p.Name).FirstOrDefault();

                        log.Debug(".dat export >> " + jKey);
                        File.WriteAllText("output_catchdata\\" + jKey + ".txt", json.ToString());
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                    }
                }

                // 폴더 열기
                Process.Start(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\output_catchdata");
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            // *.stc 파싱
            try
            {
                // 폴더 생성
                if (!Directory.Exists("output_stc"))
                    Directory.CreateDirectory("output_stc");

                // 인형 정보
                JArray GunList = ParseStc("5005.stc", 102);
                File.WriteAllText("output_stc\\gun_list.txt", GunList.ToString());

                // 중장비 정보 - 파싱 전 중장비 컬럼 순서 정렬 필요! (Squad.cs)
                //JArray SquadList = ParseStc("input\\5006.stc", 73);   
                //File.WriteAllText("output_stc\\squad_list.txt", SquadList.ToString());

                // 폴더 열기
                Process.Start(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\output_stc");
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }
    }
}
