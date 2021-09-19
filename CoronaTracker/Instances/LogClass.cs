using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CoronaTracker.Instances
{
    class LogClass
    {

        private static string log;
        private static int index;
        private static StreamWriter writer;

        public static void LogClassInitialize()
        {
            log = "";
            index = 0;
        }

        public static void Log(string value, [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string caller = "null", [CallerFilePath] string file = "null")
        {
            log += $"\n[{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}] {file.Substring(file.LastIndexOf('\\') + 1)}:{caller}:{lineNumber} » {value}";
            index++;
            if(index == 100)
            {
                index = 0;
                Save();
                log = "";
            }
        }

        public static void Save()
        {
            writer = new StreamWriter("log.txt", true);
            log.Substring(1);
            writer.Write(log);
            writer.Flush();
            writer.Close();
        }

    }
}
