using CoronaTracker.Instances;
using CoronaTracker.SubForms;
using CoronaTracker.Timers;
using CoronaTracker.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static CoronaTracker.Enums.EmployeePoseEnum;

namespace CoronaTracker
{

    /// <summary>
    /// 
    /// UI
    /// 
    /// GUI for showing main UI
    /// 
    /// </summary>

    public partial class UI : Form
    {

        // Instance for current child form
        private Form currentChildForm = null;
        // Instance for current pushed button
        private Button currentButton = null;

        public UI()
        {
            LogClass.Log($"Start initialize sub sub form");
            InitializeComponent();

            label2.Text = "version: " + ProgramVariables.Version;

            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddEllipse(0, 0, pictureBox2.Width, pictureBox2.Height);
            pictureBox2.Region = new Region(path);

            OpenChildForm(new HomeSubForm(), button6);
            LogClass.Log($"Main UI form successfully initialized");

        }
        
        /// <summary>
        /// Functions to allow move application by clicking Upper Panel
        /// </summary>
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);
        private void UpperPanel_MouseDown(object sender, MouseEventArgs e)
        {
            LogClass.Log($"UpperPanel mouse down event handler start");
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
            LogClass.Log($"UpperPanel mouse down event handler end");
        }

        /// <summary>
        /// Function to update profile picture
        /// </summary>
        public void UpdateProfilePicture()
        {
            pictureBox2.ImageLocation = ProgramVariables.ProfileURL;
        }

        /// <summary>
        /// Function to update application version
        /// </summary>
        public void UpdateVersion()
        {
            label2.Text = "version: " + ProgramVariables.Version;
        }

        /// <summary>
        /// Function to open child form
        /// </summary>
        /// <param name="childForm"> variable for child form </param>
        /// <param name="button"> variable for button </param>
        public void OpenChildForm(Form childForm, Button button)
        {
            LogClass.Log($"Start open child form");
            // Setup child form
            if (currentChildForm != null)
            {
                currentChildForm.Hide();
            }
            currentChildForm = childForm;
            childForm.TopLevel = false;
            childForm.Dock = DockStyle.Fill;
            SubFormPanel.Controls.Add(childForm);
            SubFormPanel.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();

            // Setup button visualation
            if(currentButton != null)
            {
                currentButton.TextAlign = ContentAlignment.MiddleCenter;
                currentButton.BackColor = Color.FromArgb(33, 33, 33);
            }
            currentButton = button;
            panel2.Location = currentButton.Location;
            currentButton.TextAlign = ContentAlignment.MiddleRight;
            currentButton.BackColor = Color.FromArgb(44, 44, 44);
            currentButton.TextAlign = ContentAlignment.MiddleRight;
            LogClass.Log($"Successfully opened child form");
        }

        /// <summary>
        /// Function to handle button 7 click event
        /// Try to delete all temp files and kill the app
        /// </summary>
        /// <param name="sender"> variable for sender </param>
        /// <param name="e"> variable for event arguments </param>
        private void button7_Click(object sender, EventArgs e)
        {
            LogClass.Log($"button7 click event handler start");
            try
            {
                File.Delete("temp.jpg");
                File.Delete("temp.pdf");
            }
            catch
            {

            }
            ProgramVariables.ProgramThread.KillTheApp();
        }

        /// <summary>
        /// Function to handle button 8 click event
        /// Change form window state to minimized
        /// </summary>
        /// <param name="sender"> variable for sender </param>
        /// <param name="e"> variable for event arguments </param>
        private void button8_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        /// <summary>
        /// Function to handle form visible changed event
        /// Load fullname and profile picture
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UI_VisibleChanged(object sender, EventArgs e)
        {
            LogClass.Log($"UI visible changed event handler start");
            if (ProgramVariables.Fullname != null)
            {
                label4.Text = ProgramVariables.Fullname;
                pictureBox2.ImageLocation = ProgramVariables.ProfileURL;
            }
            LogClass.Log($"UI visible changed event handler end");
        }

        /// <summary>
        /// Function to handle label 3 click event
        /// Log out current user
        /// </summary>
        /// <param name="sender"> variable for sender </param>
        /// <param name="e"> variable for event arguments </param>
        private void label3_Click(object sender, EventArgs e)
        {
            LogClass.Log($"label3 click event handler start");
            ProgramVariables.Fullname = null;
            ProgramVariables.ID = 0;
            ProgramVariables.ProfileURL = null;
            ProgramVariables.ProgramThread.GetLoginUI().Show();
            this.Hide();
            LogClass.Log($"label3 click event handler end");
        }

        /// <summary>
        /// Function to handle button 6 click event
        /// Open child form
        /// </summary>
        /// <param name="sender"> variable for sender </param>
        /// <param name="e"> variable for event arguments </param>
        private void button6_Click(object sender, EventArgs e)
        {
            LogClass.Log($"button6 click event handler start");

            if (((Button)sender).Text.Equals(currentButton.Text))
            {
                LogClass.Log("User tried to open same form");
                return;
            }

            if (((Button)sender).Text.Equals("Vaccine") ||
               ((Button)sender).Text.Equals("Patient") ||
               ((Button)sender).Text.Equals("Settings"))
            {
                if (HasAccess())
                {
                    OpenChildForm(ProgramVariables.FormCache[((Button)sender).Text], (Button)sender);
                }
                else
                {
                    MessageBox.Show("Unfortunately, you do not have access to this section.\n\nTip: Ask your leader to change you user status");
                }
            }
            else
                OpenChildForm(ProgramVariables.FormCache[((Button)sender).Text], (Button)sender);

            LogClass.Log($"button6 click event handler end");
        }

        /// <summary>
        /// Function to check if current user has access
        /// </summary>
        /// <returns>
        /// true : user has access
        /// false : user has not access
        /// </returns>
        private bool HasAccess()
        {
            if (ProgramVariables.Pose == EmployeePose.User)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Function to get current child form
        /// </summary>
        /// <returns>
        /// current child form
        /// </returns>
        public Form GetCurrentForm()
        {
            return currentChildForm;
        }

        /// <summary>
        /// Function to handle picture box 3 click event
        /// Open report form
        /// </summary>
        /// <param name="sender"> variable for sender </param>
        /// <param name="e"> variable for event arguments </param>
        private void pictureBox3_Click(object sender, EventArgs e)
        {
            if (!ProgramVariables.FormCache["REPORT"].Visible)
                ProgramVariables.FormCache["REPORT"].Show();
        }

    }
}
