﻿using CoronaTracker.Database;
using CoronaTracker.Database.DatabaseInstances;
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
    public partial class VaccineTypeSubForm : Form
    {

        // Variable for button index 
        private int index;
        // Variable for vaccine types list
        private List<VaccineTypeInstance> vaccineTypes;
        // Variable for buttons lsit
        private List<Button> buttons;

        /// <summary>
        /// Constuctor for vaccine type sub form
        /// </summary>
        public VaccineTypeSubForm()
        {
            InitializeComponent();

            vaccineTypes = DatabaseMethods.GetVaccineTypes();
            buttonPattern.Hide();

            index = 0;
            buttons = new List<Button>();

            foreach(VaccineTypeInstance type in vaccineTypes)
            {
                addButton(type);
            }

        }

        /// <summary>
        /// Function to add vaccine type button
        /// </summary>
        /// <param name="type"> variable for vaccine type </param>
        private void addButton(VaccineTypeInstance type)
        {
            Button bt = setDefaults();
            bt.Name = bt.Text = type.Name;
            this.panel1.Controls.Add(bt);
            ((Button)this.panel1.Controls[type.Name]).Location = new Point(0, index * 41);
            //bt.BringToFront();
            buttons.Add(((Button)this.panel1.Controls[type.Name]));
            ((Button)this.panel1.Controls[type.Name]).Show();
            index++;
        }

        /// <summary>
        /// Function to set button defaults
        /// </summary>
        /// <returns>
        /// Return default buttons
        /// </returns>
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

        /// <summary>
        /// Function to load vaccine type data 
        /// </summary>
        /// <param name="sender"> variable for sender </param>
        /// <param name="e"> variable for event arguments </param>
        private void button_click(object sender, EventArgs e)
        {
            foreach(VaccineTypeInstance type in vaccineTypes)
            {
                if (((Button)sender).Text.Equals(type.Name))
                {
                    textBox2.Text = type.Name;
                    richTextBox1.Text = type.Description;
                }
            }
        }

        /// <summary>
        /// Function to add vaccine type
        /// </summary>
        /// <param name="sender"> variable for sender </param>
        /// <param name="e"> variable for event arguments </param>
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if (!textBox2.Text.Equals(""))
            {
                if(MessageBox.Show("Are you sure to add vaccine type?", "Database Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (DatabaseMethods.AddVaccineType(textBox2.Text, richTextBox1.Text))
                    {
                        VaccineTypeInstance type = DatabaseMethods.GetVaccineType(textBox2.Text);
                        vaccineTypes.Add(type);
                        addButton(type);
                        MessageBox.Show("Vaccine type successfully created!");
                    }
                    else
                    {
                        MessageBox.Show("Vaccine type with this name already exists!");
                    }
                }
            }
            else
            {
                MessageBox.Show("Name is needed!");
            }
        }

        /// <summary>
        /// Function to edit vaccine type
        /// </summary>
        /// <param name="sender"> variable for sender </param>
        /// <param name="e"> variable for event arguments </param>
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (!textBox2.Text.Equals(""))
            {
                if (MessageBox.Show("Are you sure to edit vaccine type?", "Database Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (DatabaseMethods.EditVaccineType(textBox2.Text, richTextBox1.Text))
                    {
                        foreach(VaccineTypeInstance type in vaccineTypes)
                        {
                            if (type.Name.Equals(textBox2.Text))
                            {
                                type.Description = richTextBox1.Text;
                            }
                        }
                        MessageBox.Show("Vaccine type successfully edit!");
                    }
                    else
                    {
                        MessageBox.Show("Vaccine type with this name doesnt exists!");
                    }
                }
            }
            else
            {
                MessageBox.Show("Name is needed!");
            }
        }

        /// <summary>
        /// Function to remove vaccine type
        /// </summary>
        /// <param name="sender"> variable for sender </param>
        /// <param name="e"> variable for event arguments </param>
        private void pictureBox3_Click(object sender, EventArgs e)
        {
            if (!textBox2.Text.Equals(""))
            {
                if (MessageBox.Show("Are you sure to remove vaccine type?", "Database Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (DatabaseMethods.RemoveVaccineType(textBox2.Text))
                    {
                        foreach(VaccineTypeInstance type in vaccineTypes)
                        {
                            if (type.Name.Equals(textBox2.Text))
                            {
                                vaccineTypes.Remove(type);
                                break;
                            }
                        }
                        panel1.Controls.Clear();
                        buttons.Clear();
                        index = 0;
                        foreach (VaccineTypeInstance type in vaccineTypes)
                        {
                            addButton(type);
                        }
                        MessageBox.Show("Vaccine type successfully deleted!");
                    }
                    else
                    {
                        MessageBox.Show("Vaccine type with this name doesnt exists!");
                    }
                }
            }
            else
            {
                MessageBox.Show("Name is needed!");
            }
        }

        /// <summary>
        /// Function to clear text box while enter
        /// </summary>
        /// <param name="sender"> variable for sender </param>
        /// <param name="e"> variable for event arguments </param>
        private void textBox1_Enter(object sender, EventArgs e)
        {
            if (textBox1.Text.Equals("Type for search..."))
            {
                textBox1.Text = "";
            }
        }

        /// <summary>
        /// Function to filter vaccine types
        /// </summary>
        /// <param name="sender"> variable for sender </param>
        /// <param name="e"> variable for event arguments </param>
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            panel1.Controls.Clear();
            buttons.Clear();
            index = 0;
            foreach (VaccineTypeInstance type in vaccineTypes)
            {
                if (textBox1.Text.Equals("") && panel1.Controls[type.Name] != null)
                    addButton(type);
                if (type.Name.Contains(textBox1.Text))
                {
                    addButton(type);
                }
            }
        }
    }
}
