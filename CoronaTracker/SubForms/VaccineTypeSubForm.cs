using CoronaTracker.Database;
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

        private int index;
        private List<VaccineTypeInstance> vaccineTypes;
        private List<Button> buttons;

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
            foreach(VaccineTypeInstance type in vaccineTypes)
            {
                if (((Button)sender).Text.Equals(type.Name))
                {
                    textBox2.Text = type.Name;
                    richTextBox1.Text = type.Description;
                }
            }
        }

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

        private void textBox1_Enter(object sender, EventArgs e)
        {
            if (textBox1.Text.Equals("Type for search..."))
            {
                textBox1.Text = "";
            }
        }

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
