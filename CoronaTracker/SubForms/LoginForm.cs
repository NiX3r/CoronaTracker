using CoronaTracker.Database;
using CoronaTracker.Database.DatabaseInstances;
using CoronaTracker.Enums;
using CoronaTracker.SubForms.LoginSubSubForms;
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
        // Instance for actual sub sub form
        private Form currentChildForm;

        /// <summary>
        /// Constuctor for login form
        /// </summary>
        public LoginForm()
        {

            InitializeComponent();

            state = LoginWindowStatus.SignIn;
            OpenChildForm(new LoginSubSubForm());

        }

        public void OpenChildForm(Form childForm)
        {

            // Setup child form
            if (currentChildForm != null)
            {
                currentChildForm.Close();
            }
            currentChildForm = childForm;
            childForm.TopLevel = false;
            childForm.Dock = DockStyle.Fill;
            SubFormPanel.Controls.Add(childForm);
            SubFormPanel.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();

        }

        /// <summary>
        /// Function to end the app
        /// </summary>
        public void EndApp()
        {
            Application.Exit();
        }

        public void CloseForm()
        {
            this.Hide();
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
            if(state != LoginWindowStatus.SignUp)
            {
                state = LoginWindowStatus.SignUp;
                OpenChildForm(new RegisterSubSubForm());
            }
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

        private void label10_Click(object sender, EventArgs e)
        {
            if (state != LoginWindowStatus.SignIn)
            {
                state = LoginWindowStatus.SignIn;
                OpenChildForm(new LoginSubSubForm());
            }
        }

        private void label9_Click(object sender, EventArgs e)
        {
            if (state != LoginWindowStatus.ForgetPassword)
            {
                state = LoginWindowStatus.ForgetPassword;
                OpenChildForm(new ForgetSubSubForm());
            }
        }
    }
}
