using CoronaTracker.Database;
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

namespace CoronaTracker.SubForms.LoginSubSubForms
{

    /// <summary>
    /// Login Sub Sub Form
    /// 
    /// GUI child of Login Form
    /// Shows UI for login into application
    /// 
    /// </summary>

    public partial class LoginSubSubForm : Form
    {
        public LoginSubSubForm()
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
        /// Function to handle login button click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            LogClass.Log($"button1 click event handler start");
            if (!textBox1.Text.Equals("") && !textBox2.Text.Equals(""))
            {
                login();
            }
            else
            {
                MessageBox.Show("Please type password and email!");
            }
            LogClass.Log($"button1 click event handler end");
        }

        /// <summary>
        /// Function to login via input credentials
        /// 
        /// Database Methods Log In returned int :
        /// -2 : User's account is temporary banned
        /// -1 : Email or password is incorrect
        /// 0 : Email is inccorect
        /// 1 : Successfully logged in
        /// 
        /// </summary>
        private void login()
        {
            switch (DatabaseMethods.LogIn(textBox1.Text, PasswordEncryption(textBox2.Text)))
            {
                case -2: // temporary ban
                    MessageBox.Show("Your account is temporary ban!");
                    LogClass.Log("User's account is temporary banned");
                    textBox1.Text = textBox2.Text = "";
                    break;
                case -1: // email incorrect | password incorrect
                    MessageBox.Show("Email or password is incorrect!");
                    LogClass.Log("Email or password is incorrect");
                    textBox1.Text = textBox2.Text = "";
                    break;
                case 0: // email incorrect
                    MessageBox.Show("Email or password is incorrect!");
                    LogClass.Log("Email is incorrect");
                    textBox1.Text = textBox2.Text = "";
                    break;
                case 1: // successfully login
                    ProgramVariables.ProgramThread.ShowMainUI();
                    ProgramVariables.ProgramThread.HideLoginUI();
                    DatabaseMethods.SetActiveEmployeeById(ProgramVariables.ID, true);
                    textBox1.Text = textBox2.Text = "";
                    break;
            }
        }

        /// <summary>
        /// Function to handle password text box key press event
        /// Try to login after press enter
        /// </summary>
        /// <param name="sender"> variable for sender </param>
        /// <param name="e"> variable for key press event arguments </param>
        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                e.Handled = true;
                login();
            }
        }
    }
}
