﻿using CoronaTracker.Enums;
using CoronaTracker.Instances;
using CoronaTracker.SubForms;
using CoronaTracker.Timers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static CoronaTracker.Enums.EmployeePoseEnum;

namespace CoronaTracker.Utils
{

    /// <summary>
    /// Util class for program variables
    /// </summary>

    class ProgramVariables
    {
        // Instance for main program thread
        public static MainProgramThread ProgramThread { get; set; }
        // Instance for logged in employee fullname
        public static String Fullname { get; set; }
        // Instance for logged in employee profile picture url
        public static String ProfileURL { get; set; }
        // Instance for logged in employee pose
        public static EmployeePose Pose { get; set; }
        // Instance for logged in employee id
        public static int ID { get; set; }
        // Instance for this program version
        public static String Version { get; set; }
        // Instance for refresh database connection timer
        public static RefreshConnectionTimer RefreshConnection { get; set; }
        // Instance for send discord webhooks
        public static DiscordWebhook Webhook { get; set; }
        // Instance for cache
        public static Dictionary<string, Form> FormCache { get; set; }
        // Instance for Covid data cache
        public static Dictionary<string, CovidInfo> CovidCache { get; set; }
    }
}
