using CoronaTracker.Instances;
using CoronaTracker.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoronaTracker.SubForms
{

    /// <summary>
    /// 
    /// Countries Sub Form
    /// 
    /// GUI child of UI
    /// Shows UI for get actual data from targeting country
    /// 
    /// </summary>

    public partial class CountriesSubForm : Form
    {

        // Instance for lastest selected country
        private Button TEMPLATE;
        private Dictionary<string, Bitmap> COUNTRIES;

        /// <summary>
        /// Constructor for countries sub form
        /// </summary>
        public CountriesSubForm()
        {
            LogClass.Log($"Start initialize sub sub form");
            InitializeComponent();
            TEMPLATE = button2;
            COUNTRIES = new Dictionary<string, Bitmap>();
            LoadCountriesDefault();
            LogClass.Log($"Sub sub form successfully initialized");
        }

        /// <summary>
        /// Function to load default countries buttons
        /// </summary>
        private void LoadCountriesDefault()
        {
            foreach(RegionInfo info in CultureInfo.GetCultures(CultureTypes.SpecificCultures).Select(x => new RegionInfo(x.LCID)))
            {
                if (!COUNTRIES.ContainsKey(info.Name))
                {
                    try
                    {
                        WebClient client = new WebClient();
                        Stream stream = client.OpenRead("https://www.countryflagicons.com/FLAT/32/" + info.TwoLetterISORegionName + ".png");
                        Bitmap bitmap = new Bitmap(stream);
                        COUNTRIES.Add(info.DisplayName, bitmap);
                    }
                    catch { }
                }
            }
        }

        /// <summary>
        /// Function to update country data
        /// </summary>
        private void SetCountryData(CovidInfo lastestCountry)
        {
            LogClass.Log($"Start set country data");

            if (lastestCountry != null)
            {
                label1.Text = lastestCountry.recovered.ToString();
                label6.Text = lastestCountry.confirmed.ToString();
                label11.Text = lastestCountry.critical.ToString();
                label4.Text = lastestCountry.deaths.ToString();

                //pictureBox4.ImageLocation = "https://www.countryflagicons.com/FLAT/64/" + lastestCountry.code + ".png";

                label8.Text = "Last update: " + lastestCountry.lastUpdate.ToString();
                label9.Text = "Last change: " + lastestCountry.lastChange.ToString();
            }
            else
            {
                MessageBox.Show("Unfortunately country with name '" + textBox1.Text + "' does not exists!");
            }
            LogClass.Log($"Successfully set country data");

        }

        /// <summary>
        /// Function to empty text box on click
        /// </summary>
        /// <param name="sender"> variable for sender </param>
        /// <param name="e"> variable for event arguments </param>
        private void textBox1_Enter(object sender, EventArgs e)
        {
            LogClass.Log($"textBox1 enter event handler start");
            if (textBox1.Text.Equals("Type for search..."))
            {
                textBox1.Text = "";
            }
            LogClass.Log($"textBox1 enter event handler end");
        }

        /// <summary>
        /// Function to return Button with it defaults
        /// </summary>
        /// <returns>
        /// Default Button
        /// </returns>
        private Button CopyDefault()
        {
            Button button = new Button();
            button.Dock = TEMPLATE.Dock;
            button.FlatAppearance.BorderSize = TEMPLATE.FlatAppearance.BorderSize;
            button.FlatStyle = TEMPLATE.FlatStyle;
            button.Font = TEMPLATE.Font;
            button.ForeColor = TEMPLATE.ForeColor;
            button.BackColor = TEMPLATE.BackColor;
            button.ImageAlign = TEMPLATE.ImageAlign;
            button.Width = 47;
            button.Click += onClick;
            return button;
        }

        /// <summary>
        /// Function to refresh list of countries by specific parameters
        /// </summary>
        /// <param name="regex"> variable for parametres </param>
        public void LoadList(string regex = "")
        {
            LogClass.Log("Loading countries list" + (regex.Equals("") ? "" : " with parameters '" + regex + "'"));

            panel5.Controls.Clear();
            foreach(string country in COUNTRIES.Keys)
            {
                if (country.ToLower().Contains(regex.ToLower()))
                {
                    Button p = CopyDefault();
                    p.Text = country;
                    p.Image = COUNTRIES[country];
                    panel5.Controls.Add(p);
                }
            }

            LogClass.Log("Successfully loaded");
        }

        /// <summary>
        /// Function to handle form load event
        /// Load list with no parameters
        /// </summary>
        /// <param name="sender"> variable for sender </param>
        /// <param name="e"> variable for event arguments </param>
        private void CountriesSubForm_Load(object sender, EventArgs e)
        {
            LoadList();
        }

        /// <summary>
        /// Function to handle country click event
        /// Load country data in UI
        /// </summary>
        /// <param name="sender"> variable for sender </param>
        /// <param name="e"> variable for event arguments </param>
        private void onClick(object sender, EventArgs e)
        {
            if (!ProgramVariables.CovidCache.ContainsKey(((Button)sender).Text))
            {
                CovidInfo info = RestAPI.GetCovidDataAsync(((Button)sender).Text).Result;
                if(info != null)
                {
                    ProgramVariables.CovidCache.Add(((Button)sender).Text, info);
                    pictureBox4.Image = COUNTRIES[((Button)sender).Text];
                    label7.Text = ((Button)sender).Text;
                    SetCountryData(info);
                }
            }
            else
            {
                CovidInfo info = ProgramVariables.CovidCache[((Button)sender).Text];
                pictureBox4.Image = COUNTRIES[((Button)sender).Text];
                label7.Text = ((Button)sender).Text;
                SetCountryData(info);
            }
        }

        /// <summary>
        /// Function to handle parameters text box text change event
        /// Refresh countries list with specific parameters
        /// </summary>
        /// <param name="sender"> variable for sender </param>
        /// <param name="e"> variable for event arguments </param>
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (!textBox1.Text.Equals(""))
            {
                LoadList(textBox1.Text);
            }
        }
    }
}
