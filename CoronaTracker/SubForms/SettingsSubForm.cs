using CoronaTracker.Database;
using CoronaTracker.Database.DatabaseInstances;
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
    public partial class SettingsSubForm : Form
    {

        /// <summary>
        /// Constuctor for finds sub sub form
        /// </summary>
        public SettingsSubForm()
        {
            LogClass.Log($"Start initialize sub sub form");
            InitializeComponent();

            LogClass.Log($"Sub sub form successfully initialized");
        }

        /// <summary>
        /// Function to encrypt password
        /// Use 2 salt & SHA256 encryption
        /// </summary>
        /// <param name="password"> variable for password to encrypt </param>
        /// <returns>
        /// Return encrypted password
        /// </returns>
        private String PasswordEncryption(String password)
        {
            string salt1 = "6&eL#YwFJFqD";
            string salt2 = "zyQ@^cVX9H67";
            string saltedPassword = salt1 + password + salt2;

            var sha1 = new System.Security.Cryptography.SHA256Managed();
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

        /// <summary>
        /// Function to edit profile info
        /// </summary>
        /// <param name="sender"> variable for sender </param>
        /// <param name="e"> variable for event arguments </param>
        private void button1_Click(object sender, EventArgs e)
        {
            LogClass.Log($"button1 click event handler start");
            if (MessageBox.Show("Are you sure to edit your profile information?", "Action with database", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                DatabaseMethods.EditEmployeeInfo(textBox1.Text, Convert.ToInt32(textBox2.Text));
                ProgramVariables.ProfileURL = textBox1.Text;
                ProgramVariables.ProgramUI.UpdateProfilePicture();
            }
            LogClass.Log($"button1 click event handler end");
        }

        /// <summary>
        /// Function to temporary load image
        /// </summary>
        /// <param name="sender"> variable for sender </param>
        /// <param name="e"> variable for event arguments </param>
        private void button2_Click(object sender, EventArgs e)
        {
            LogClass.Log($"button2 click event handler start");
            try
            {
                pictureBox1.ImageLocation = textBox1.Text;
            }
            catch
            {
                MessageBox.Show("Could not load image");
            }
            LogClass.Log($"button2 click event handler end");
        }

        /// <summary>
        /// Function to allow only numbers
        /// </summary>
        /// <param name="sender"> variable for sender </param>
        /// <param name="e"> variable for event arguments </param>
        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            LogClass.Log($"textBox2 key press event handler start");
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
            LogClass.Log($"textBox2 key press event handler end");
        }

        /// <summary>
        /// Function to change the password
        /// </summary>
        /// <param name="sender"> variable for sender </param>
        /// <param name="e"> variable for event arguments </param>
        private void button3_Click(object sender, EventArgs e)
        {
            LogClass.Log($"button3 click event handler start");
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
            LogClass.Log($"button3 click event handler end");
        }

        /// <summary>
        /// Function to load role by email
        /// </summary>
        /// <param name="sender"> variable for sender </param>
        /// <param name="e"> variable for event arguments </param>
        private void button4_Click(object sender, EventArgs e)
        {
            LogClass.Log($"button4 event handler start");
            if (textBox6.Text != "" && textBox6.Text.Contains("@") && textBox6.Text.Contains("."))
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
            LogClass.Log($"button4 event handler end");
        }

        /// <summary>
        /// Function to change pose
        /// </summary>
        /// <param name="sender"> variable for sender </param>
        /// <param name="e"> variable for event arguments </param>
        private void button5_Click(object sender, EventArgs e)
        {
            LogClass.Log($"button5 click event handler start");
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
            LogClass.Log($"button5 click event handler end");
        }

        private void SettingsSubForm_Load(object sender, EventArgs e)
        {
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
    }
}
