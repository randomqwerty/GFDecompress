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

namespace GFDecompress
{
    class StcDownloader
    {
        string dataVersion;
        string location;

        //생성자
        public StcDownloader(string _location = "kr") {

            location = _location;

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(getGameHost(location) + "Index/version");
            req.UserAgent = "Dalvik/1.6.0 (Linux; U; Android 4.4.2; SM-N935F Build/JLS36C)";
            req.Headers.Add("X-Unity-Version", "5.2.5f1");
            req.ContentType = @"application/x-www-form-urlencoded";

            HttpWebResponse wResp = (HttpWebResponse)req.GetResponse();
            Stream respPostStream = wResp.GetResponseStream();
            StreamReader readerpost = new StreamReader(respPostStream);

            JObject obj = JObject.Parse(readerpost.ReadToEnd());
            dataVersion = obj["data_version"].ToString();

            Console.WriteLine("검색된 데이터 버전: " + dataVersion);

        }

        public void downloadStc() {
            string url = getStcUrl();

            // 삭제 후 폴더 생성
            if (Directory.Exists("stc"))
                Directory.Delete("stc", true);
            Directory.CreateDirectory("stc");

            Console.WriteLine("최신 데이터 다운로드 중...");
            Console.WriteLine("Url: " + url);

            try
            {
                WebClient client = new WebClient();
                client.DownloadFile(getStcUrl(), "./stc/stc.zip");
                Console.WriteLine("다운로드 성공");
                Console.WriteLine("압축해제 실행...");
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

        public string getStcUrl() {
            var md5Hash = MD5.Create();
            string hash = Crypto.GetMd5Hash(md5Hash, dataVersion);

            return getCDNHost(location) + "data/stc_" + dataVersion + hash + ".zip";
        }

        public long getCurrentTime() {
            var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            return (long)timeSpan.TotalSeconds;
        }

        public string getGameHost(string _location, int chanel = 1001) {//채널코드 한섭만 적용됨, 나중에 패킷파싱으로 다른서버 알아내야함
            switch (_location) {
                case "kr":
                    return "http://gf-game.girlfrontline.co.kr/index.php/" + chanel.ToString() + "/";
                case "jp":
                    return "http://gfjp-game.sunborngame.com/index.php/"+ chanel.ToString() + "/";
                case "en":
                    return "http://gf-game.sunborngame.com/index.php/" + chanel.ToString() + "/";
                default:
                    return "http://s1.gw.gf.ppgame.com/index.php/" + chanel.ToString() + "/";
            }
        }

        public string getCDNHost(string _location)
        {
            switch (_location) {
                case "kr":
                    return "http://gfkrcdn.imtxwy.com/";
                case "jp":
                    return "https://s3-ap-northeast-1.amazonaws.com/gf1-jpfile-server/";
                case "en":
                    return "http://dkn3dfwjnmzcj.cloudfront.net/";
                default:
                    return "http://oss-rescnf.gf.ppgame.com/";
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
    }
}
