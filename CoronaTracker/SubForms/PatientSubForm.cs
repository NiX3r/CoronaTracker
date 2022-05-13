using CoronaTracker.Instances;
using CoronaTracker.SubForms.PatientSubSubForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoronaTracker.SubForms
{

    /// <summary>
    /// 
    /// Patient Sub Form
    /// 
    /// GUI child of UI
    /// Shows UI for manipulate with patient's data
    /// 
    /// </summary>

    public partial class PatientSubForm : Form
    {

        // Variable for current child form
        private Form currentChildForm;
        // Variable for current button
        private Button currentButton;

        /// <summary>
        /// Constuctor for patient sub form
        /// </summary>
        public PatientSubForm()
        {
            LogClass.Log($"Start initialize sub sub form");
            InitializeComponent();

            LogClass.Log($"Sub sub form successfully initialized");
        }

        /// <summary>
        /// Function to get current child form
        /// </summary>
        /// <returns>
        /// Return current child form
        /// </returns>
        public Form GetCurrentForm()
        {
            return currentChildForm;
        }

        /// <summary>
        /// Function to open child form
        /// </summary>
        /// <param name="childForm"> variable for child form to set </param>
        /// <param name="button"> variable for button to set </param>
        public void OpenChildForm(Form childForm, Button button)
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

            // Setup button visualation
            if (currentButton != null)
            {
                currentButton.TextAlign = ContentAlignment.MiddleCenter;
                currentButton.BackColor = Color.FromArgb(44,44,44);
            }
            currentButton = button;
            SubFormPanel.Location = currentButton.Location;
            currentButton.TextAlign = ContentAlignment.BottomCenter;
            currentButton.BackColor = Color.FromArgb(55,55,55);
            currentButton.TextAlign = ContentAlignment.BottomCenter;
            LogClass.Log($"Open child form end");
        }

        /// <summary>
        /// Function to load sub sub form
        /// </summary>
        /// <param name="sender"> variable for sender </param>
        /// <param name="e"> variable for event arguments </param>
        private void button1_Click(object sender, EventArgs e)
        {
            LogClass.Log($"button1 click event handler start");
            switch (((Button)sender).Text)
            {
                case "List":
                    OpenChildForm(new ListSubSubForm(), (Button)sender);
                    break;
                case "Finds":
                    OpenChildForm(new FindsSubSubForm(), (Button)sender);
                    break;
                case "Vaccinacions":
                    OpenChildForm(new VaccinationsSubSubForm(), (Button)sender);
                    break;
            }
            LogClass.Log($"button1 click event handler end");
        }

        public void killTheCam()
        {
            try
            {
                ((ListSubSubForm)currentChildForm).exitcamera();
                ((FindsSubSubForm)currentChildForm).exitcamera();
                ((VaccinationsSubSubForm)currentChildForm).exitcamera();
            }
            catch (Exception ex) { }
        }

        /// <summary>
        /// Function to handle form load event
        /// Open default child form - List Sub Sub Form
        /// </summary>
        /// <param name="sender"> variable for sender </param>
        /// <param name="e"> variable for event arguments </param>
        private void PatientSubForm_Load(object sender, EventArgs e)
        {
            OpenChildForm(new ListSubSubForm(), button1);
        }
    }
}
