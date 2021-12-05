using CoronaTracker.Database;
using CoronaTracker.Database.DatabaseInstances;
using CoronaTracker.Enums;
using CoronaTracker.Instances;
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

            LogClass.Log($"Start initialize sub sub form");
            InitializeComponent();
            LogClass.Log($"Sub sub form successfully initialized");

            state = LoginWindowStatus.SignIn;
            OpenChildForm(new LoginSubSubForm());

        }

        public void OpenChildForm(Form childForm)
        {
            LogClass.Log($"Start open child form");
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
            LogClass.Log($"Open child form end");

        }

        /// <summary>
        /// Function to end the app
        /// </summary>
        public void EndApp()
        {
            ProgramVariables.ProgramThread.KillTheApp();
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
            LogClass.Log($"loginForm mouse down event handler start");
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
            LogClass.Log($"loginForm mouse down event handler end");
        }

        /// <summary>
        /// Function to change status sign in / sign up
        /// </summary>
        /// <param name="sender"> variable for sender </param>
        /// <param name="e"> variable for event arguments </param>
        private void label3_Click(object sender, EventArgs e)
        {
            LogClass.Log($"label3 click event handler start");
            if (state != LoginWindowStatus.SignUp)
            {
                state = LoginWindowStatus.SignUp;
                OpenChildForm(new RegisterSubSubForm());
            }
            LogClass.Log($"label3 click event handler end");
        }

        /// <summary>
        /// Function to exit app while click button
        /// </summary>
        /// <param name="sender"> variable for sender </param>
        /// <param name="e"> variable for event arguments </param>
        private void button7_Click(object sender, EventArgs e)
        {
            LogClass.Log($"Ending program");
            LogClass.Save();
            Application.Exit();
        }

        private void label10_Click(object sender, EventArgs e)
        {
            LogClass.Log($"label10 click event handler start");
            if (state != LoginWindowStatus.SignIn)
            {
                state = LoginWindowStatus.SignIn;
                OpenChildForm(new LoginSubSubForm());
            }
            LogClass.Log($"label10 click event handler end");
        }

        private void label9_Click(object sender, EventArgs e)
        {
            LogClass.Log($"label9 click event handler start");
            if (state != LoginWindowStatus.ForgetPassword)
            {
                state = LoginWindowStatus.ForgetPassword;
                OpenChildForm(new ForgetSubSubForm());
            }
            LogClass.Log($"label9 click event handler end");
        }
    }
}
