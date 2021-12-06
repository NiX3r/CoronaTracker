using CoronaTracker.Database;
using CoronaTracker.Database.DatabaseInstances;
using CoronaTracker.Instances;
using CoronaTracker.Timers;
using CoronaTracker.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using static CoronaTracker.Enums.EmployeePoseEnum;

namespace CoronaTracker.SubForms
{
    public partial class MainProgramThread : Form
    {

        private bool isDev = false;
        private LoginForm loginForm;
        private UI mainUI;
        private LoadingForm loadingForm;
        private Thread loadingThread;

        public MainProgramThread(string[] args)
        {
            LogClass.Log("Initialize main program thread");
            InitializeComponent();
            this.loginForm = new LoginForm();
            this.mainUI = new UI();
            this.loadingForm = new LoadingForm();
            //this.ShowLoadingUI();

            loadingThread = new Thread(() =>
            {
                Application.Run(new LoadingForm());
            });
            loadingThread.Start();

            InitializeProgram(args);
        }

        private void InitializeProgram(string[] args)
        {
            ProgramVariables.Version = "2.2.0";
            LogClass.Log("Program started. Version: " + ProgramVariables.Version);

            LogClass.Log("Creating discord webhook instance");
            ProgramVariables.Webhook = new DiscordWebhook();
            //ProgramVariables.Webhook.WebHook = SecretClass.GetWebhookLink();
            LogClass.Log("Webhook instance created");

            LogClass.Log("Loading input parameters");
            if (args.Length > 0)
            {
                foreach (string s in args)
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
            if (isDev)
                ProgramVariables.Version += " - DEV MODE";
            if (versionCheck.Equals("-1"))
            {
                ProgramVariables.Version += " - pre-release";
            }
            else if (!versionCheck.Equals(""))
            {
                System.Diagnostics.Process.Start(versionCheck);
                MessageBox.Show("You're running on older version! Please download newest version.");
                Application.Exit();
            }
            LogClass.Log("Data from database loaded");
            LogClass.Log("Initializing program variables");

            ProgramVariables.FormCache = new Dictionary<string, Form>();
            ProgramVariables.FormCache.Add("REPORT", new ReportBug());
            ProgramVariables.FormCache.Add("Home", new HomeSubForm());
            ProgramVariables.FormCache.Add("Dashboard", new DashboardSubForm());
            ProgramVariables.FormCache.Add("Countries", new CountriesSubForm());
            ProgramVariables.FormCache.Add("Patient", new PatientSubForm());
            ProgramVariables.FormCache.Add("Vaccine", new VaccineTypeSubForm());
            ProgramVariables.FormCache.Add("Settings", new SettingsSubForm());

            ProgramVariables.CovidCache = new Dictionary<string, CovidInfo>();

            ProgramVariables.RefreshConnection = new RefreshConnectionTimer();

            ProgramVariables.RefreshConnection.ChangeStatus(true);
            LogClass.Log("Program variables initialized");
            int id = DatabaseMethods.HasAutoLogin();

            this.loadingThread.Abort();
            if (isDev)
            {
                ProgramVariables.ID = 38;
                EmployeeInstance user = DatabaseMethods.GetEmployeeByID(id);
                ProgramVariables.Fullname = user.Fullname;
                ProgramVariables.ProfileURL = user.ProfilePictureURL;
                ProgramVariables.Pose = EmployeePose.Developer;

                ProgramVariables.ProgramThread.ShowMainUI();
            }
            else if (id != 0)
            {
                ProgramVariables.ID = id;
                EmployeeInstance user = DatabaseMethods.GetEmployeeByID(id);
                ProgramVariables.Fullname = user.Fullname;
                ProgramVariables.ProfileURL = user.ProfilePictureURL;
                ProgramVariables.Pose = DatabaseMethods.GetPoseByID(id);

                this.ShowMainUI();
            }
            else
            {
                //Application.Run(ProgramVariables.LoginUI);
                this.ShowLoginUI();
            }
        }

        public void KillTheApp()
        {
            LogClass.Log($"Ending program");
            LogClass.Save();
            this.loginForm.Close();
            this.mainUI.Close();
            this.loadingForm.Close();
            Application.Exit();
        }

        public void ShowLoginUI()
        {
            LogClass.Log("Show login UI");
            this.loginForm.Show();
        }

        public void HideLoginUI()
        {
            LogClass.Log("Hide login UI");
            this.loginForm.Hide();
        }

        public LoginForm GetLoginUI()
        {
            return this.loginForm;
        }

        public void ShowMainUI()
        {
            LogClass.Log("Show main UI");
            this.mainUI.Show();
        }

        public void HideMainUI()
        {
            LogClass.Log("Hide main UI");
            this.mainUI.Hide();
        }

        public UI GetMainUI()
        {
            return this.mainUI;
        }

        public void ShowLoadingUI()
        {
            LogClass.Log("Show loading UI");
            this.loadingForm.Show();
        }

        public void HideLoadingUI()
        {
            LogClass.Log("Hide loading UI");

            this.loadingForm.Hide();
        }

        public LoadingForm GetLoadingUI()
        {
            return this.loadingForm;
        }

    }
}
