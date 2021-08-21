using CoronaTracker.Database;
using CoronaTracker.Database.DatabaseInstances;
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
    public partial class SettingsSubForm : Form
    {
        public SettingsSubForm()
        {
            InitializeComponent();

            EmployeeInstance employee = DatabaseMethods.GetEmployeeByID(ProgramVariables.ID);
            textBox1.Text = employee.ProfilePictureURL;
            textBox2.Text = employee.Phone.ToString();
            pictureBox1.ImageLocation = employee.ProfilePictureURL;

            // Checks if employee is permitted to edit others poses
            if (!DatabaseMethods.HasEmployeePermitChangePose())
            {
                button4.Hide();
                button5.Hide();
            }
            else
                notPermited.Hide();

        }

        private String PasswordEncryption(String password)
        {
            string salt1 = "6&eL#YwFJFqD";
            string salt2 = "zyQ@^cVX9H67";
            string saltedPassword = salt1 + password + salt2;

            var sha1 = new System.Security.Cryptography.SHA1Managed();
            var plaintextBytes = Encoding.UTF8.GetBytes(saltedPassword);
            var hashBytes = sha1.ComputeHash(plaintextBytes);

            var sb = new StringBuilder();
            foreach (var hashByte in hashBytes)
            {
                sb.AppendFormat("{0:x2}", hashByte);
            }
            var hashString = sb.ToString();
            return hashString;
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (MessageBox.Show("Are you sure to edit your profile information?", "Action with database", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                DatabaseMethods.EditEmployeeInfo(textBox1.Text, Convert.ToInt32(textBox2.Text));
                ProgramVariables.ProfileURL = textBox1.Text;
                ProgramVariables.ProgramUI.UpdateProfilePicture();
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                pictureBox1.ImageLocation = textBox1.Text;
            }
            catch
            {
                MessageBox.Show("Could not load image");
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

            if (MessageBox.Show("Are you sure to change your password?", "Action with database", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (textBox4.Text == textBox5.Text && textBox4.Text != "" && textBox3.Text != "")
                {
                    DatabaseMethods.UpdatePassword(PasswordEncryption(textBox3.Text), PasswordEncryption(textBox4.Text));
                    if (checkBox1.Checked)
                        if (!DatabaseMethods.AddAutoLoginSession())
                            MessageBox.Show("Auto login already exists");
                    MessageBox.Show("Password successfully changed!");
                    return;
                }
                if (checkBox1.Checked)
                    if (!DatabaseMethods.AddAutoLoginSession())
                        MessageBox.Show("Auto login already exists");
            }
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if(textBox6.Text != "" && textBox6.Text.Contains("@") && textBox6.Text.Contains("."))
            {
                String pose = DatabaseMethods.GetEmployeeRoleByEmail(textBox6.Text);
                switch (pose)
                {
                    case "guest":
                        radioButton1.Select();
                        break;
                    case "employee":
                        radioButton2.Select();
                        break;
                    case "leader":
                        radioButton3.Select();
                        break;
                    case "developer":
                        radioButton4.Select();
                        break;
                    case "":
                        MessageBox.Show("Employee with that name does not exists!");
                        break;
                    default:
                        MessageBox.Show("Something wen't wront!");
                        break;
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure to change pose?", "Action with database", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (textBox6.Text != "" && textBox6.Text.Contains("@") && textBox6.Text.Contains("."))
                {
                    String pose = DatabaseMethods.GetEmployeeRoleByEmail(textBox6.Text);
                    if (radioButton1.Checked)
                        pose = "guest";
                    else if (radioButton2.Checked)
                        pose = "employee";
                    else if (radioButton3.Checked)
                        pose = "leader";
                    else
                        pose = "developer";

                    DatabaseMethods.UpdatePoseByEmail(textBox6.Text, pose);

                }
            }
        }
    }
}
