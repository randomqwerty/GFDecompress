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

			if (clientVersion == "29999") {
				minversion = 3010;
			}

            Console.WriteLine("Retrived data version: " + dataVersion);

        }
        //stc 다운

        public void downloadStc() {
            string url = getStcUrl();

            // 삭제 후 폴더 생성
            if (Directory.Exists("stc"))
                Directory.Delete("stc", true);
            Directory.CreateDirectory("stc");

            Console.WriteLine("\n Downloading latest stc data from " + url);

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
                Console.WriteLine("Downloaded successfullly");
                Console.WriteLine("Decompressing");
                ZipFile.ExtractToDirectory("./stc/stc.zip","./stc");
                Console.WriteLine("Decompression succesful");
                Console.WriteLine("Deleting compressed file");
                File.Delete("./stc/stc.zip");
                Console.WriteLine("Deleted succesfully" + Environment.NewLine);
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
            StreamReader output2 = null;
            StreamReader output3 = null;
            StreamReader output4 = null;
            StreamReader error = null;
            StreamReader error2 = null;
            StreamReader error3 = null;
            StreamReader error4 = null;

            Python.run("Scripts\\deserializer.py", $"Assets_raw\\{location}\\{filename}", ref output, ref error);
            Python.run("Scripts\\deserializer2.py", $"Assets_raw\\{location}\\{filename}", ref output2, ref error2);
            Python.run("Scripts\\deserializer3.py", $"Assets_raw\\{location}\\{filename}", ref output3, ref error3);
            Python.run("Scripts\\deserializer4.py", $"Assets_raw\\{location}\\{filename}", ref output4, ref error4);

            Console.WriteLine("Downloading textes.ab, texttable.ab, avgtext.ab, textlpatch.ab");

            string[] textes = new string[2];
            string[] texttable = new string[2];
            string[] avgtext = new string[2];
            string[] textlpatch = new string[2];

            int i = 0;
            while (true) {
                string str = output.ReadLine();
                if (str == null)
                    break;
                textes[i] = str;
                i++;
            }

            int j = 0;
            while (true)
            {
                string str2 = output2.ReadLine();
                if (str2 == null)
                    break;
                texttable[j] = str2;
                j++;
            }

            int k = 0;
            while (true)
            {
                string str3 = output3.ReadLine();
                if (str3 == null)
                    break;
                avgtext[k] = str3;
                k++;
            }

            int l = 0;
            while (true)
            {
                string str4 = output4.ReadLine();
                if (str4 == null)
                    break;
                textlpatch[l] = str4;
                l++;
            }

            string url = textes[0] + textes[1] + ".ab";
            string url2 = texttable[0] + texttable[1] + ".ab";
            string url3 = avgtext[0] + avgtext[1] + ".ab";
            string url4 = textlpatch[0] + textlpatch[1] + ".ab";

            if (url == ".ab")
            {
                Console.WriteLine("Error with deserializer.py, make sure it exists (redownload from this GitHub) and make sure Python with unitypack is installed.");
                Console.ReadKey();
                Environment.Exit(1);
            }

            if (url2 == ".ab")
            {
                Console.WriteLine("Error with deserializer2.py, make sure it exists (redownload from this GitHub) and make sure Python with unitypack is installed.");
                Console.ReadKey();
                Environment.Exit(1);
            }

            if (url3 == ".ab")
            {
                Console.WriteLine("Error with deserializer3.py, make sure it exists (redownload from this GitHub) and make sure Python with unitypack is installed.");
                Console.ReadKey();
                Environment.Exit(1);
            }

            if (url4 == ".ab")
            {
                Console.WriteLine("Error with deserializer4.py, make sure it exists (redownload from this GitHub) and make sure Python with unitypack is installed.");
                Console.ReadKey();
                Environment.Exit(1);
            }

            Console.WriteLine(url);
            client.DownloadFile(url, $"Assets_raw\\{location}\\asset_textes.ab");

            Console.WriteLine(url2);
            client.DownloadFile(url2, $"Assets_raw\\{location}\\asset_texttable.ab");

            Console.WriteLine(url3);
            client.DownloadFile(url3, $"Assets_raw\\{location}\\asset_textavg.ab");

            if (textlpatch[1] != null)
            {
                Console.WriteLine(url4);
                client.DownloadFile(url4, $"Assets_raw\\{location}\\asset_textlpatch.ab");
            }

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
				case "tw":
					return "http://sn-game.txwy.tw/index.php/1001/";
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
				case "tw":
					return "http://sncdn.imtxwy.com/";
                default:
                    return "http://gf-cn.cdn.sunborngame.com/";
            }
        }

        public string getUpdateHost(string _location) {
            switch (_location) {
                case "kr":
                    return "http://sn-list.girlfrontline.co.kr/";
                case "jp":
					return "https://gfjp-cdn.sunborngame.com/";
                case "en":
                    return "http://gfus-cdn.sunborngame.com/";
				case "tw":
					return "http://sn-list.txwy.tw/";
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
