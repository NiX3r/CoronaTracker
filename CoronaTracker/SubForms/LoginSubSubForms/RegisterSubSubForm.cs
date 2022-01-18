﻿using CoronaTracker.Database;
using CoronaTracker.Instances;
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
    /// 
    /// Register Sub Sub Form
    /// 
    /// GUI child of Login Sub Form
    /// Shows UI for register into application
    /// 
    /// </summary>

    public partial class RegisterSubSubForm : Form
    {
        public RegisterSubSubForm()
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
        /// Function to handle register button click event
        /// </summary>
        /// <param name="sender"> variable for sender </param>
        /// <param name="e"> variable for event arguments </param>
        private void button1_Click(object sender, EventArgs e)
        {
            LogClass.Log($"button1 click event handler start");
            // Checks basic informations
            if (!textBox1.Text.Equals("") && !textBox2.Text.Equals("") && !textBox3.Text.Equals("") && !textBox4.Text.Equals("") && !textBox5.Text.Equals("") &&
                textBox3.Text.Length == 9 && textBox2.Text.Equals(textBox5.Text) && textBox1.Text.Contains(".") && textBox1.Text.Contains("@"))
            {
                addUser();
            }
            else
                MessageBox.Show("You type bad input! Please try again");
            LogClass.Log($"button1 click event handler end");
        }

        /// <summary>
        /// Function to handle phone text box key press event
        /// Allows only digits inputs
        /// </summary>
        /// <param name="sender"> variable for sender </param>
        /// <param name="e"> variable for key press event arguments </param>
        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            LogClass.Log($"textBox3 key press event handler start");
            if (!Char.IsNumber(e.KeyChar) && !Char.IsControl(e.KeyChar))
            {
                e.Handled = true;
                LogClass.Log($"textBox3 key press event handler end - handled");
                return;
            }
            LogClass.Log($"textBox3 key press event handler end");
        }

        /// <summary>
        /// Function to add user into database
        /// 
        /// Database Methods Add User return bool :
        /// true : user successfully created
        /// false : user unsuccessfully created
        /// </summary>
        private void addUser()
        {
            if (DatabaseMethods.AddUser(textBox4.Text, textBox1.Text, Convert.ToInt32(textBox3.Text), PasswordEncryption(textBox2.Text)))
            {
                button1.Text = "Account created! Please log in";
                button1.BackColor = Color.Lime;
                button1.Enabled = false;
            }
            else
                MessageBox.Show("Account cannot be create!");
        }

        /// <summary>
        /// Function to handle retype password text box key press event
        /// Try to add user after press enter
        /// </summary>
        /// <param name="sender"> variable for sender </param>
        /// <param name="e"> variable for key press event arguments </param>
        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == 13)
            {
                e.Handled= true;
                addUser();
            }
        }
    }
}
