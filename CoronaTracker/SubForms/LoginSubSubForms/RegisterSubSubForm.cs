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
    public partial class RegisterSubSubForm : Form
    {
        public RegisterSubSubForm()
        {
            InitializeComponent();
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

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
