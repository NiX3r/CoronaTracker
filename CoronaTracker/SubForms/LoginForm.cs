using CoronaTracker.Database;
using CoronaTracker.Enums;
using CoronaTracker.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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

        private LoginWindowStatus state;

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

        public void EndApp()
        {
            Application.Exit();
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

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);
        private void LoginForm_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

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
                        case 0: // email & password uncorrect
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

        private void button1_Click(object sender, EventArgs e)
        {
            logIn();   
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void textBox5_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
                logIn();
        }
    }
}
