using CoronaTracker.Database;
using CoronaTracker.Database.DatabaseInstances;
using CoronaTracker.Enums;
using CoronaTracker.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static CoronaTracker.Enums.LoginWindowStatusEnum;

namespace CoronaTracker.SubForms
{
    public partial class LoginForm : Form
    {

        // Enum for sign in / sign up state
        private LoginWindowStatus state;

        /// <summary>
        /// Constuctor for login form
        /// </summary>
        public LoginForm()
        {

            InitializeComponent();

            state = LoginWindowStatus.SignIn;
            label8.Hide();
            textBox5.Hide();
            label7.Hide();
            textBox4.Hide();
            label6.Hide();
            textBox3.Hide();

        }

        /// <summary>
        /// Function to end the app
        /// </summary>
        public void EndApp()
        {
            Application.Exit();
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

        /// <summary>
        /// Allow move form while click and move on form
        /// </summary>
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);
        private void LoginForm_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        /// <summary>
        /// Function to change status sign in / sign up
        /// </summary>
        /// <param name="sender"> variable for sender </param>
        /// <param name="e"> variable for event arguments </param>
        private void label3_Click(object sender, EventArgs e)
        {
            if(state == LoginWindowStatus.SignIn)
            {
                state = LoginWindowStatus.SignUp;
                label3.Text = "sign in";
                button1.Text = "Sign up";
                label8.Show();
                textBox5.Show();
                label7.Show();
                textBox4.Show();
                label6.Show();
                textBox3.Show();
            }
            else
            {
                state = LoginWindowStatus.SignIn;
                label3.Text = "sign up";
                button1.Text = "Sign in";
                label8.Hide();
                textBox5.Hide();
                label7.Hide();
                textBox4.Hide();
                label6.Hide();
                textBox3.Hide();
            }
        }

        /// <summary>
        /// Function to log in
        /// </summary>
        private void logIn()
        {
            if (state == LoginWindowStatus.SignIn)
            {
                if (!textBox1.Text.Equals("") && !textBox2.Text.Equals(""))
                {
                    switch (DatabaseMethods.LogIn(textBox1.Text, PasswordEncryption(textBox2.Text)))
                    {
                        case -2:
                            MessageBox.Show("Your account is temporary ban!");
                            textBox1.Text = textBox2.Text = textBox3.Text = textBox4.Text = textBox5.Text = "";
                            break;
                        case -1: // email correct | password uncorrect
                            MessageBox.Show("Email or password is uncorrect!");
                            textBox1.Text = textBox2.Text = textBox3.Text = textBox4.Text = textBox5.Text = "";
                            break;
                        case 0: // email uncorrect
                            MessageBox.Show("Email or password is uncorrect!");
                            textBox1.Text = textBox2.Text = textBox3.Text = textBox4.Text = textBox5.Text = "";
                            break;
                        case 1: // successfully login
                            ProgramVariables.ProgramUI.Show();
                            textBox1.Text = textBox2.Text = textBox3.Text = textBox4.Text = textBox5.Text = "";
                            this.Hide();
                            break;
                    }
                }
                else
                {
                    MessageBox.Show("Please type password and email!");
                }
            }
            else
            {
                if (!textBox1.Text.Equals("") && !textBox2.Text.Equals("") &&
                    !textBox3.Text.Equals("") && !textBox4.Text.Equals("") &&
                    !textBox5.Text.Equals(""))
                {
                    if (textBox2.Text.Equals(textBox5.Text))
                    {
                        if (DatabaseMethods.AddUser(textBox4.Text, textBox1.Text, Convert.ToInt32(textBox3.Text), PasswordEncryption(textBox2.Text)))
                        {
                            MessageBox.Show("Account successfully created! Please sign in.");
                            textBox1.Text = textBox2.Text = textBox3.Text = textBox4.Text = textBox5.Text = "";
                        }
                        else
                        {
                            MessageBox.Show("Account with that email already exists!");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please type all parameters for sign up!");
                    }
                }
                else
                {
                    MessageBox.Show("Please type all parameters for sign up!");
                }
            }
        }

        /// <summary>
        /// Function to log in while click button
        /// </summary>
        /// <param name="sender"> variable for sender </param>
        /// <param name="e"> variable for event arguments </param>
        private void button1_Click(object sender, EventArgs e)
        {
            logIn();   
        }

        /// <summary>
        /// Function to exit app while click button
        /// </summary>
        /// <param name="sender"> variable for sender </param>
        /// <param name="e"> variable for event arguments </param>
        private void button7_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// Function to log in while press enter
        /// Does not work yet
        /// </summary>
        /// <param name="sender"> variable for sender </param>
        /// <param name="e"> variable for event arguments </param>
        private void textBox5_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
                logIn();
        }
    }
}
