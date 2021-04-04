using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GFDecompress
{
    class Python
    {
        public static void run(string cmd, string args, ref StreamReader output, ref StreamReader error  ) {
            ProcessStartInfo start = new ProcessStartInfo();
            string envVar =  Environment.GetEnvironmentVariable("LOCALAPPDATA");
            start.FileName = $@"{envVar}\\Programs\\Python\\Python38\\python.exe";
            start.Arguments = string.Format("{0} {1}",cmd, args);
            start.UseShellExecute = false;
            start.CreateNoWindow = false;
            start.WorkingDirectory = Directory.GetCurrentDirectory();

            start.RedirectStandardOutput = true;
            start.RedirectStandardError = true;

            using (Process process = Process.Start(start)) {
                output = process.StandardOutput;
                error = process.StandardError;
            }
        }
    }
}
