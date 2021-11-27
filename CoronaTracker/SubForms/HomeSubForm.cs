﻿using CoronaTracker.Instances;
using CoronaTracker.Timers;
using CoronaTracker.Utils;
using System;
using System.Timers;
using System.Windows.Forms;

namespace CoronaTracker.SubForms
{
    public partial class HomeSubForm : Form
    {

        HomeDateTimeTimer timer;

        /// <summary>
        /// Constuctor for home sub form
        /// </summary>
        public HomeSubForm()
        {

            LogClass.Log($"Start initialize sub sub form");
            InitializeComponent();
            timer = new HomeDateTimeTimer();

            label13.Text = $"{DateTime.Now.ToString("dd/MM/yyyy")}\n{DateTime.Now.ToString("HH:mm:ss")}";
            timerDel = new timerDelDelegate(timerDelMethod);
            LogClass.Log($"Sub sub form successfully initialized");
        }

        public delegate void timerDelDelegate();
        public timerDelDelegate timerDel;
        void timerDelMethod() { label13.Text = $"{DateTime.Now.ToString("dd/MM/yyyy")}\n{DateTime.Now.ToString("HH:mm:ss")}";}

        /// <summary>
        /// Function to load data on change visible
        /// </summary>
        /// <param name="sender"> variable for sender </param>
        /// <param name="e"> variable for event arguments </param>
        private void HomeSubForm_VisibleChanged(object sender, EventArgs e)
        {
            LogClass.Log($"HomeSubForm visible changed event handler start");

            timer.ChangeStatus(this.Visible);

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
            LogClass.Log($"HomeSubForm visible changed event handler end");
        }

        /// <summary>
        /// Function to open site
        /// </summary>
        /// <param name="sender"> variable for sender </param>
        /// <param name="e"> variable for event arguments </param>
        private void label5_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://rapidapi.com/Gramzivi/api/covid-19-data/");
        }

        /// <summary>
        /// Function to open site
        /// </summary>
        /// <param name="sender"> variable for sender </param>
        /// <param name="e"> variable for event arguments </param>
        private void label6_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.flaticon.com/free-icon/coronavirus_2760147");
        }

        /// <summary>
        /// Function to open site
        /// </summary>
        /// <param name="sender"> variable for sender </param>
        /// <param name="e"> variable for event arguments </param>
        private void label7_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com");
        }

        /// <summary>
        /// Function to open site
        /// </summary>
        /// <param name="sender"> variable for sender </param>
        /// <param name="e"> variable for event arguments </param>
        private void label8_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://stackoverflow.com");
        }

        /// <summary>
        /// Function to open site
        /// </summary>
        /// <param name="sender"> variable for sender </param>
        /// <param name="e"> variable for event arguments </param>
        private void label9_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.w3schools.com");
        }

        /// <summary>
        /// Function to open site
        /// </summary>
        /// <param name="sender"> variable for sender </param>
        /// <param name="e"> variable for event arguments </param>
        private void label11_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.mockaroo.com");
        }

        private void HomeSubForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer.ChangeStatus(false);
        }
    }
}
