using CoronaTracker.SubForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoronaTracker.Instances
{
    class LogClass
    {

        private static string log;
        private static int index;
        private static StreamWriter writer;

        public static LogForm LogForm;
        public static bool DoLog;
        public static bool DoLogForm;

        public static void LogClassInitialize()
        {
            log = "";
            index = 0;
            DoLog = true;
            DoLogForm = false;
            LogForm = new LogForm();
        }

        public static void Log(string value, [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string caller = "null", [CallerFilePath] string file = "null")
        {
            string line = $"\n[{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}] {file.Substring(file.LastIndexOf('\\') + 1)}:{caller}:{lineNumber} » {value}";
            log += line;

            if (DoLogForm)
            {
                if(Application.OpenForms.OfType<LogForm>().FirstOrDefault() != null)
                    LogForm.Log(line);
            }

            index++;
            if(index == 50)
            {
                index = 0;
                Save();
                log = "";
            }
        }

        public static void Save()
        {
            writer = new StreamWriter("logger.log", true);
            if(log.Length > 2)
                log.Substring(1);
            writer.Write(log);
            writer.Flush();
            writer.Close();
        }

        public static string GetLog()
        {
            return log;
        }

    }
}
