using CoronaTracker.Database;
using CoronaTracker.Database.DatabaseInstances;
using CoronaTracker.SubForms;
using CoronaTracker.Timers;
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

        private const bool isDev = false;

        [STAThread]
        static void Main()
        {
            
            DatabaseMethods.SetupDatabase();
            ProgramVariables.Version = "1.2.0";
            if (!DatabaseMethods.CheckVersion().Equals(""))
            {
                System.Diagnostics.Process.Start(DatabaseMethods.GetLinkToLastestVersion());
                MessageBox.Show("You're running on older version! Please download newest version.");
                Application.Exit();
            }
            else
            {
                ProgramVariables.RefreshConnection = new RefreshConnectionTimer();
                ProgramVariables.ProgramUI = new UI();
                ProgramVariables.LoginUI = new LoginForm();

                ProgramVariables.RefreshConnection.ChangeStatus(true);
                ProgramVariables.ProgramUI.Hide();

                int id = DatabaseMethods.HasAutoLogin();
                if (id != 0)
                {
                    ProgramVariables.ID = id;
                    EmployeeInstance user = DatabaseMethods.GetEmployeeByID(id);
                    ProgramVariables.Fullname = user.Fullname;
                    ProgramVariables.ProfileURL = user.ProfilePictureURL;
                    Application.Run(ProgramVariables.ProgramUI);
                }
                else
                    Application.Run(ProgramVariables.LoginUI);
            }
        }

    }
}
