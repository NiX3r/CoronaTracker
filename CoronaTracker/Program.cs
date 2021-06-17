using CoronaTracker.Database;
using CoronaTracker.Database.DatabaseInstances;
using CoronaTracker.SubForms;
using CoronaTracker.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoronaTracker
{
    static class Program
    {

        private const bool isDev = true;

        [STAThread]
        static void Main()
        {

            var task = RestAPI.GetCovidDataAsync();
            DatabaseMethods.SetupDatabase();
            ProgramVariables.Version = "1.0.0";
            if (!DatabaseMethods.CheckVersion().Equals(""))
            {
                System.Diagnostics.Process.Start("https://www.trackcorona.live");
                MessageBox.Show("You're running on older version! Please download newest version.");
                Application.Exit();
            }
            else
            {
                ProgramVariables.ProgramUI = new UI();
                ProgramVariables.LoginUI = new LoginForm();

                ProgramVariables.ProgramUI.Hide();

                Application.EnableVisualStyles();
                if (isDev)
                {
                    ProgramVariables.ID = 2;
                    ProgramVariables.Fullname = "Daniel Iliev";
                    ProgramVariables.ProfileURL = "https://i1.wp.com/bnel242.com/wp-content/uploads/2019/12/purple-space.jpg?ssl=1";
                    Application.Run(ProgramVariables.ProgramUI);
                }
                else
                    Application.Run(ProgramVariables.LoginUI);
            }
        }

    }
}
