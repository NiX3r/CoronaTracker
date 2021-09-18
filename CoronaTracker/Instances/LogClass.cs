using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            writer = new StreamWriter("log.txt", true);
        }

        public static void Log(string value)
        {
            log += $"\n[{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}] » {value}";
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
            log.Substring(1);
            writer.Write(log);
            writer.Flush();
        }

        public static void Close()
        {
            writer.Close();
        }

    }
}
