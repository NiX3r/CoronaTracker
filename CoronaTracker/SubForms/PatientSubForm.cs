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

    public partial class PatientSubForm : Form
    {

        private Form currentChildForm;
        private Button currentButton;

        public PatientSubForm()
        {
            InitializeComponent();

            OpenChildForm(new ListSubSubForm(), button1);

        }

        public Form GetCurrentForm()
        {
            return currentChildForm;
        }

        public void OpenChildForm(Form childForm, Button button)
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
        }

        private void button1_Click(object sender, EventArgs e)
        {
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
        }
    }
}
