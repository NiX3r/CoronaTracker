using CoronaTracker.Instances;
using CoronaTracker.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoronaTracker.SubForms
{
    public partial class HomeSubForm : Form
    {
        public HomeSubForm()
        {
            InitializeComponent();
        }

        private void HomeSubForm_VisibleChanged(object sender, EventArgs e)
        {
            if (ProgramVariables.Fullname != null)
            {

                CovidInfo czech = null;
                czech = RestAPI.GetCovidDataAsync("czechia").Result;

                if(czech != null)
                {
                    label4.Text = "Recovered: " + czech.recovered.ToString() +
                              "\nConfirmed: " + czech.confirmed.ToString() +
                              "\nCritical: " + czech.critical.ToString() +
                              "\nDeaths: " + czech.deaths.ToString();
                }

            }
        }

        private void label5_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://rapidapi.com/Gramzivi/api/covid-19-data/");
        }

        private void label6_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.flaticon.com/free-icon/coronavirus_2760147");
        }

        private void label7_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com");
        }

        private void label8_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://stackoverflow.com");
        }

        private void label9_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.w3schools.com");
        }

        private void label11_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.mockaroo.com");
        }
    }
}
