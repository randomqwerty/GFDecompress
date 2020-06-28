using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO.Compression;
using System.Dynamic;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace GFDecompress
{
    class Downloader
    {
        string dataVersion;
        string clientVersion;
        double minversion;
        string abVersion;
        string location;

        //생성자
        public Downloader(string _location = "kr") {

            location = _location;

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(getGameHost(location) + "Index/version");
            req.UserAgent = "Dalvik/2.1.0 (Linux; U; Android 9; SM-N935k Build/PPR1.180610.011)";
            req.Headers.Add("X-Unity-Version", "2017.4.33f1");
            req.ContentType = @"application/x-www-form-urlencoded";

            HttpWebResponse wResp = (HttpWebResponse)req.GetResponse();
            Stream respPostStream = wResp.GetResponseStream();
            StreamReader readerpost = new StreamReader(respPostStream);

            JObject obj = JObject.Parse(readerpost.ReadToEnd());
            dataVersion = obj["data_version"].ToString();
            clientVersion = obj["client_version"].ToString();
            minversion = Math.Round(double.Parse(clientVersion) / 100) * 10;
            abVersion = obj["ab_version"].ToString();

            Console.WriteLine("검색된 데이터 버전: " + dataVersion);

        }
        //stc 다운

        public void downloadStc() {
            string url = getStcUrl();

            // 삭제 후 폴더 생성
            if (Directory.Exists("stc"))
                Directory.Delete("stc", true);
            Directory.CreateDirectory("stc");

            Console.WriteLine("최신 데이터 다운로드 중...");
            Console.WriteLine("Url: " + url);

            foreach (var item in new DirectoryInfo("stc").GetFiles())
            {
                if (item.Name == "desktop.ini")
                    continue;
                File.Delete($"stc/{item.Name}");
            }

            try
            {
                WebClient client = new WebClient();
                client.DownloadFile(getStcUrl(), "./stc/stc.zip");
                Console.WriteLine("다운로드 성공");
                Console.WriteLine("압축해제 중...");
                ZipFile.ExtractToDirectory("./stc/stc.zip","./stc");
                Console.WriteLine("압축해제 성공");
                Console.WriteLine("압축파일 삭제...");
                File.Delete("./stc/stc.zip");
                Console.WriteLine("완료!");
            }
            catch (Exception e){
                Console.WriteLine(e);
            }
        }
        //어셋  다운
        public void downloadAsset() {
            string key = "kxwL8X2+fgM=";
            string iv = "M9lp+7j2Jdwqr+Yj1h+A";

            byte[] bkey = Convert.FromBase64String(key);
            byte[] biv = Convert.FromBase64String(iv);

            string encryptedVersion = Crypto.GetDesEncryted($"{minversion}_{abVersion}_AndroidResConfigData", bkey, biv.Take(8).ToArray());

            string filename = Regex.Replace(encryptedVersion, @"[^a-zA-Z0-9]", "") + ".txt";

            if (!Directory.Exists("Assets_raw"))
                Directory.CreateDirectory("Assets_raw");

            if (!Directory.Exists($"Assets_raw\\{location}"))
                Directory.CreateDirectory($"Assets_raw\\{location}");

            WebClient client = new WebClient();
            client.DownloadFile(getAssetUrl(filename), $"Assets_raw\\{location}\\{filename}");

            StreamReader output = null;
            StreamReader error = null;

            Python.run("Scripts\\deserializer.py", $"Assets_raw\\{location}\\{filename}", ref output, ref error);

            Console.WriteLine("textes.ab 다운받는 중...");


            string[] textes = new string[2];
            int i = 0;
            while (true) {
                string str = output.ReadLine();
                if (str == null)
                    break;
                textes[i] = str;
                i++;
            }
            string url = textes[0] + textes[1] + ".dat";
            
            if(url == ".dat")
            {
                Console.WriteLine("deserializer.py가 존재하지 않습니다. 파일이 존재하는지 다시 한번 확인해주세요.");
                Environment.Exit(1);
            }

            Console.WriteLine(url);
            client.DownloadFile(url, $"Assets_raw\\{location}\\textes.zip");

            Console.WriteLine("압축해제 중...");
            try
            {
                ZipFile.ExtractToDirectory($"Assets_raw\\{location}\\textes.zip", $"Assets_raw\\{location}");
            }
            catch {
                File.Delete($"Assets_raw\\{location}\\asset_textes.ab");
                ZipFile.ExtractToDirectory($"Assets_raw\\{location}\\textes.zip", $"Assets_raw\\{location}");
            }
            
            Console.WriteLine("압축파일 삭제");
            File.Delete($"Assets_raw\\{location}\\textes.zip");

            //Console.WriteLine("에셋 추출 중...");
            //ProcessStartInfo extractor = new ProcessStartInfo("Scripts\\extractexe\\extract.exe", $@"Assets_raw\\{location}\\asset_textes.ab");
            //Process extract = Process.Start(extractor);
            //extract.WaitForExit();

            //Python.run("Scripts\\extractpy\\abunpack.py", $"-0 Asset\\{location} Assets_raw\\{location}\\asset_textes.ab", ref output, ref error);
            //Console.WriteLine(error.ReadToEnd());
            
        }

        public string getStcUrl() {
            var md5Hash = MD5.Create();
            string hash = Crypto.GetMd5Hash(md5Hash, dataVersion);

            return getCDNHost(location) + "data/stc_" + dataVersion + hash + ".zip";
        }

        public string getAssetUrl(string filename) {
            return getUpdateHost(location) + filename;
        }

        public long getCurrentTime() {
            var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            return (long)timeSpan.TotalSeconds;
        }

        public string getGameHost(string _location) {
            switch (_location) {
                case "kr":
                    return "http://gf-game.girlfrontline.co.kr/index.php/1001/";
                case "jp":
                    return "http://gfjp-game.sunborngame.com/index.php/1001/";
                case "en":
                    return "http://gf-game.sunborngame.com/index.php/1001/";
                default:
                    return "http://gfcn-game.gw.merge.sunborngame.com/index.php/1000/";
            }
        }

        public string getCDNHost(string _location)
        {
            switch (_location) {
                case "kr":
                    return "http://gfkrcdn.imtxwy.com/";
                case "jp":
                    return "https://gfjp-cdn.sunborngame.com/";
                case "en":
                    return "https://gfus-cdn.sunborngame.com/";
                default:
                    return "http://gf-cn.cdn.sunborngame.com/";
            }
        }

        public string getUpdateHost(string _location) {
            switch (_location) {
                case "kr":
                    return "http://sn-list.girlfrontline.co.kr/";
                case "jp":
                    //return "https://s3-ap-northeast-1.amazonaws.com/gf1-jpfile-server/";
                    return "https://d2p0tz30gps08r.cloudfront.net/";
                case "en":
                    //return "http://gf-transit.sunborngame.com/";
                    return "http://dkn3dfwjnmzcj.cloudfront.net/";
                default:
                    return "http://gf-cn.cdn.sunborngame.com/";
            }
        }
    }

    public class Crypto {
        public static string GetMd5Hash(MD5 md5Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        public static string GetDesEncryted(string _data, byte[] _key, byte[] _iv) {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();

            des.Key = _key;
            des.IV = _iv;

            MemoryStream ms = new MemoryStream();

            CryptoStream stream = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);

            byte[] data = Encoding.UTF8.GetBytes(_data.ToCharArray());

            stream.Write(data, 0, data.Length);
            stream.FlushFinalBlock();

            return Convert.ToBase64String(ms.ToArray());
        }
    }
}
