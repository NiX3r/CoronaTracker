using CoronaTracker.Database;
using CoronaTracker.Instances;
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
    public partial class ForgetSubSubForm : Form
    {

        private string code;
        private int id;

        public ForgetSubSubForm()
        {
            InitializeComponent();

            label1.Hide();
            label2.Hide();
            label5.Hide();
            textBox2.Hide();
            textBox3.Hide();
            textBox4.Hide();

            code = PasswordEncryption(MacAddress() + DateTime.Now.ToString("yyyyMMddHHmmss"));
            code = code.Substring(0, 8);

        }

        private string MacAddress()
        {
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

            return macadress;
        }

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

        private void button1_Click(object sender, EventArgs e)
        {

            if(button1.Text.Equals("Send Mail"))
            {
                if (!textBox1.Text.Equals("") && textBox1.Text.Contains("@") && textBox1.Text.Contains("."))
                {
                    id = DatabaseMethods.GetEmployeeIdByEmail(textBox1.Text);
                    if (id >= 0)
                    {
                        if (DatabaseMethods.CheckCodeValidity(id))
                        {
                            if (EmailWriterClass.WriteCodeEmail(textBox1.Text, code))
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

        }
    }
}
