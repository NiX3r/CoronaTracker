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
    public partial class CountriesSubForm : Form
    {

        // Instance for lastest selected country
        private CovidInfo lastestCountry;

        /// <summary>
        /// Constructor for countries sub form
        /// </summary>
        public CountriesSubForm()
        {
            InitializeComponent();

            lastestCountry = null;

        }

        /// <summary>
        /// Function to update country data
        /// </summary>
        private void SetCountryData()
        {

            lastestCountry = RestAPI.GetCovidDataAsync(textBox1.Text).Result;

            if (lastestCountry != null)
            {
                label1.Text = lastestCountry.recovered.ToString();
                label6.Text = lastestCountry.confirmed.ToString();
                label11.Text = lastestCountry.critical.ToString();
                label4.Text = lastestCountry.deaths.ToString();

                pictureBox4.ImageLocation = "https://www.countryflags.io/" + lastestCountry.code + "/flat/64.png";

                label8.Text = "Last update: " + lastestCountry.lastUpdate.ToString();
                label9.Text = "Last change: " + lastestCountry.lastChange.ToString();

                label7.Text = lastestCountry.country;
            }
            else
            {
                MessageBox.Show("Unfortunately country with name '" + textBox1.Text + "' does not exists!");
            }

        }

        /// <summary>
        /// Function to empty text box on click
        /// </summary>
        /// <param name="sender"> variable for sender </param>
        /// <param name="e"> variable for event arguments </param>
        private void textBox1_Enter(object sender, EventArgs e)
        {
            if(textBox1.Text.Equals("Type for search..."))
            {
                textBox1.Text = "";
            }
        }

        /// <summary>
        /// Function to catch button event and set data
        /// </summary>
        /// <param name="sender"> variable for sender </param>
        /// <param name="e"> variable for event arguments </param>
        private void button1_Click(object sender, EventArgs e)
        {

            SetCountryData();
            
        }
    }
}
