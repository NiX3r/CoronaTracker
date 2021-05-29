using CoronaTracker.Database;
using CoronaTracker.SubForms;
using CoronaTracker.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoronaTracker
{
    static class Program
    {
        [STAThread]
        static void Main()
        {

            RestAPI.GetCovidDataAsync();
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
                Application.Run(ProgramVariables.LoginUI);
            }
        }
    }
}
