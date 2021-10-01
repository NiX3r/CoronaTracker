using CoronaTracker.Database;
using CoronaTracker.Database.DatabaseInstances;
using CoronaTracker.Enums;
using CoronaTracker.Instances;
using CoronaTracker.SubForms;
using CoronaTracker.Timers;
using CoronaTracker.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using static CoronaTracker.Enums.EmployeePoseEnum;

namespace CoronaTracker
{
    static class Program
    {

        public static bool isDev = false;

        [STAThread]
        static void Main(string[] args)
        {

            ProgramVariables.Version = "1.5.0";
            LogClass.LogClassInitialize();
            LogClass.Log("Program started. Version: " + ProgramVariables.Version);

            LogClass.Log("Creating discord webhook instance");
            ProgramVariables.Webhook = new DiscordWebhook();
            ProgramVariables.Webhook.ProfilePicture = "";
            ProgramVariables.Webhook.UserName = "";
            ProgramVariables.Webhook.WebHook = SecretClass.GetWebhookLink();
            LogClass.Log("Webhook instance created");

            LogClass.Log("Loading input parameters");
            if (args.Length > 0)
            {
                foreach(string s in args)
                {
                    if (s.Equals("-devmode"))
                    {
                        isDev = true;
                    }
                    else if (s.Equals("-logoff"))
                    {
                        LogClass.DoLog = false;
                    }
                    else if (s.Equals("-showlog"))
                    {
                        LogClass.DoLogForm = true;
                        if (Application.OpenForms.OfType<LogForm>().FirstOrDefault() == null)
                            LogClass.LogForm.Show();
                    }
                    else if (s.Contains("-v"))
                    {
                        ProgramVariables.Version = s.Substring(2);
                    }
                }
            }
            LogClass.Log("Input parameters loaded");
            LogClass.Log("Loading initialize database");
            DatabaseMethods.SetupDatabase();
            LogClass.Log("Database initialized");
            LogClass.Log("Loading data from database");
            String versionCheck = DatabaseMethods.CheckVersion();
            if(isDev)
                ProgramVariables.Version += " - DEV MODE";
            if (versionCheck.Equals("-1"))
            {
                ProgramVariables.Version += " - alfa";
            }
            else if (!versionCheck.Equals(""))
            {
                System.Diagnostics.Process.Start(versionCheck);
                MessageBox.Show("You're running on older version! Please download newest version.");
                Application.Exit();
            }
            LogClass.Log("Data from database loaded");
            LogClass.Log("Initializing program variables");
            ProgramVariables.RefreshConnection = new RefreshConnectionTimer();
            ProgramVariables.ProgramUI = new UI();
            ProgramVariables.LoginUI = new LoginForm();

            ProgramVariables.RefreshConnection.ChangeStatus(true);
            ProgramVariables.ProgramUI.Hide();
            LogClass.Log("Program variables initialized");
            int id = DatabaseMethods.HasAutoLogin();
            if (isDev)
            {
                ProgramVariables.ID = 38;
                EmployeeInstance user = DatabaseMethods.GetEmployeeByID(id);
                ProgramVariables.Fullname = user.Fullname;
                ProgramVariables.ProfileURL = user.ProfilePictureURL;
                ProgramVariables.Pose = EmployeePose.Developer;
                Application.Run(ProgramVariables.ProgramUI);
            }
            else if (id != 0)
            {
                ProgramVariables.ID = id;
                EmployeeInstance user = DatabaseMethods.GetEmployeeByID(id);
                ProgramVariables.Fullname = user.Fullname;
                ProgramVariables.ProfileURL = user.ProfilePictureURL;
                ProgramVariables.Pose = DatabaseMethods.GetPoseByID(id);
                Application.Run(ProgramVariables.ProgramUI);
            }
            else
                Application.Run(ProgramVariables.LoginUI);
        }

    }
}
