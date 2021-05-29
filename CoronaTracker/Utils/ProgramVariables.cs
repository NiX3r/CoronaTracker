using CoronaTracker.Instances;
using CoronaTracker.SubForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoronaTracker.Utils
{
    class ProgramVariables
    {

        public static UI ProgramUI { get; set; }
        public static LoginForm LoginUI { get; set; }
        public static String Fullname { get; set; }
        public static String ProfileURL { get; set; }
        public static int ID { get; set; }
        public static List<CovidInfo> CovidData { get; set; }
        public static String Version { get; set; }

    }
}
