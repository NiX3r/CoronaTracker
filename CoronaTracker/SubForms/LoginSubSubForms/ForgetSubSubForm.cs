using CoronaTracker.Database;
using CoronaTracker.Instances;
using CoronaTracker.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoronaTracker.SubForms.LoginSubSubForms
{

    /// <summary>
    /// 
    /// Forget Sub Sub Form
    /// 
    /// GUI child of Login Form
    /// Shows UI for reseting password via email code confirmation
    /// 
    /// </summary>

    public partial class ForgetSubSubForm : Form
    {

        private string code;
        private int id;

        public ForgetSubSubForm()
        {
            LogClass.Log($"Start initialize sub sub form");
            InitializeComponent();

            label1.Hide();
            label2.Hide();
            label5.Hide();
            textBox2.Hide();
            textBox3.Hide();
            textBox4.Hide();

            code = PasswordEncryption(MacAddress() + DateTime.Now.ToString("yyyyMMddHHmmss"));
            code = code.Substring(0, 8);
            LogClass.Log($"Sub sub form successfully initialized");
        }

        /// <summary>
        /// Function to get current device MAC address
        /// </summary>
        /// <returns>
        /// empty string : can't find any network device
        /// else : mac address 
        /// </returns>
        private string MacAddress()
        {
            LogClass.Log($"Getting mac accress");
            String macadress = string.Empty;

            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                OperationalStatus ot = nic.OperationalStatus;
                if (nic.OperationalStatus == OperationalStatus.Up)
                {
                    macadress = nic.GetPhysicalAddress().ToString();
                    break;
                }
            }
            LogClass.Log($"Got mac address: {macadress}");
            return macadress;
        }

        /// <summary>
        /// Function to encrypt password
        /// Use 2 salt & SHA256 encryption
        /// </summary>
        /// <param name="password"> variable for password to encrypt </param>
        /// <returns>
        /// Return encrypted password
        /// </returns>
        private string PasswordEncryption(string password)
        {
            string salt1 = "6&eL#YwFJFqD";
            string salt2 = "zyQ@^cVX9H67";
            string saltedPassword = salt1 + password + salt2;

            var sha1 = new SHA256Managed();
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
        /// Function to handle action button click event
        /// 
        /// Send Mail : Send mail to targeting email with specific code & get user ID
        /// Send Code : Check input code with cache code and database code
        /// Send Pass : Send new password into database
        /// 
        /// </summary>
        /// <param name="sender"> variable for sender </param>
        /// <param name="e"> variable for event arguments </param>
        private void button1_Click(object sender, EventArgs e)
        {
            LogClass.Log($"button1 click event handler start");
            if (button1.Text.Equals("Send Mail"))
            {
                if (!textBox1.Text.Equals("") && textBox1.Text.Contains("@") && textBox1.Text.Contains("."))
                {
                    id = DatabaseMethods.GetEmployeeIdByEmail(textBox1.Text);
                    if (id >= 0)
                    {
                        if (DatabaseMethods.CheckCodeValidity(id))
                        {
                            if (EmailWriter.WriteCodeEmail(textBox1.Text, code))
                            {
                                DatabaseMethods.LogResetPassword(textBox1.Text, code, id);
                                textBox1.ReadOnly = true;
                                button1.Text = "Send Code";
                                textBox2.Show();
                                label5.Show();
                            }
                            else
                                MessageBox.Show("Unfortunately email cannot be sent!");
                        }
                        else
                            MessageBox.Show("You already tried to reset password!");
                    }
                    else
                        MessageBox.Show("Unfortunately email cannot be sent!");
                }
            }
            else if (button1.Text.Equals("Send Code"))
            {
                if (textBox2.Text.Equals(code))
                {
                    if (DatabaseMethods.IsCodeValid(id, textBox2.Text))
                    {
                        DatabaseMethods.UpdateResetPasswordStatus(id, textBox2.Text, "SUCCESS");
                        textBox2.ReadOnly = true;
                        button1.Text = "Send Pass";
                        label1.Show();
                        label2.Show();
                        textBox3.Show();
                        textBox4.Show();
                    }
                }
            }
            else if (button1.Text.Equals("Send Pass"))
            {
                if(!textBox3.Text.Equals("") && textBox3.Text.Equals(textBox4.Text))
                {
                    if (DatabaseMethods.IsCodeValid(id, textBox2.Text))
                    {
                        DatabaseMethods.UpdateResetPasswordStatus(id, textBox2.Text, "RESET");
                        DatabaseMethods.UpdatePassword(id, PasswordEncryption(textBox3.Text));
                        textBox2.ReadOnly = true;
                        button1.Enabled = false;
                        button1.BackColor = Color.Lime;
                        button1.Text = "All done! Please log in";
                    }
                }
            }
            LogClass.Log($"button1 click event handler end");
        }
    }
}
