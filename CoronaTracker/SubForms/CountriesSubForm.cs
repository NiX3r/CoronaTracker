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

        private List<Button> buttons;
        private int index;

        public CountriesSubForm()
        {
            InitializeComponent();

            buttonPattern.Hide();
            buttons = new List<Button>();
            index = 0;

            foreach(CovidInfo covid in ProgramVariables.CovidData)
            {
                addButton(covid);
            }

        }

        private void addButton(CovidInfo covid)
        {
            Button bt = setDefaults();
            bt.Name = bt.Text = covid.location;
            this.panel1.Controls.Add(bt);
            ((Button)this.panel1.Controls[covid.location]).Location = new Point(0, index * 41);
            //bt.BringToFront();
            buttons.Add(((Button)this.panel1.Controls[covid.location]));
            ((Button)this.panel1.Controls[covid.location]).Show();
            index++;
        }

        private Button setDefaults()
        {
            Button output = new Button();
            output.Size = buttonPattern.Size;
            output.BackColor = Color.FromArgb(44, 44, 44);
            output.FlatStyle = FlatStyle.Flat;
            output.FlatAppearance.BorderSize = 0;
            output.Font = new Font("MS Reference Sans Serif", 11.0f, FontStyle.Bold);
            output.ForeColor = Color.FromArgb(240, 240, 240);
            output.Click += button_click;
            return output;
        }

        private void button_click(object sender, EventArgs e)
        {
            foreach(CovidInfo covid in ProgramVariables.CovidData)
            {
                if (covid.location.Equals((((Button)sender).Text)))
                {
                    pictureBox4.ImageLocation = "https://www.countryflags.io/" + covid.country_code + "/flat/64.png";
                    label7.Text = covid.location;
                    label1.Text = covid.recovered.ToString();
                    label6.Text = covid.confirmed.ToString();
                    label4.Text = covid.dead.ToString();
                }
            }
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            if(textBox1.Text.Equals("Type for search..."))
            {
                textBox1.Text = "";
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            panel1.Controls.Clear();
            buttons.Clear();
            index = 0;
            foreach (CovidInfo covid in ProgramVariables.CovidData)
            {
                if (textBox1.Text.Equals("") && panel1.Controls[covid.location] != null)
                    addButton(covid);
                if (covid.location.Contains(textBox1.Text))
                {
                    addButton(covid);
                }
            }
        }
    }
}
