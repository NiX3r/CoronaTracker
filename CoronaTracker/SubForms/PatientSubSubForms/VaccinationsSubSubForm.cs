using AForge.Video;
using AForge.Video.DirectShow;
using CoronaTracker.Database;
using CoronaTracker.Database.DatabaseInstances;
using CoronaTracker.Instances;
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
using ZXing;

namespace CoronaTracker.SubForms.PatientSubSubForms
{
    public partial class VaccinationsSubSubForm : Form
    {

        // Instance for capture video from camera
        private FilterInfoCollection CaptureDevice;
        // Instance for lastest camera frame
        private VideoCaptureDevice FinalFrame;
        // Instance for selected patient
        private PatientInstance patient;
        // Instance for patient vaccinate
        private PatientVaccineAction vaccine;
        // Instance for vaccine types
        private Dictionary<string, int> vaccineTypes;

        /// <summary>
        /// Load data from patient
        /// </summary>
        public delegate void setDataDelegate();
        public setDataDelegate setData;
        void setDataMethod()
        {
            label2.Text = patient.Fullname;
            label3.Text = patient.PersonalNumberFirst;
            label6.Text = patient.PersonalNumberSecond;
            vaccine = DatabaseMethods.GetPatientVaccine(DatabaseMethods.GetPatientIDByPersonalNumber(patient.PersonalNumberFirst, patient.PersonalNumberSecond));
            if(vaccine != null)
            {
                dateTimePicker1.Value = vaccine.FirstDate;
                dateTimePicker2.Value = vaccine.SecondDate;
                textBox1.Text = vaccine.EmployeeString;
                listBox1.SetSelected(listBox1.Items.IndexOf(vaccine.VaccineTypeString), true);
            }
        }

        /// <summary>
        /// Create instance for round corners
        /// </summary>
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect, // x-coordinate of upper-left corner
            int nTopRect, // y-coordinate of upper-left corner
            int nRightRect, // x-coordinate of lower-right corner
            int nBottomRect, // y-coordinate of lower-right corner
            int nWidthEllipse, // height of ellipse
            int nHeightEllipse // width of ellipse
        );

        /// <summary>
        /// Constructor for vaccinations sub sub form
        /// </summary>
        public VaccinationsSubSubForm()
        {
            InitializeComponent();

            vaccineTypes = new Dictionary<string, int>();
            dateTimePicker1.Value = DateTime.Now;
            dateTimePicker2.Value = DateTime.Now.AddDays(40);

            IntPtr handle = CreateRoundRectRgn(0, 0, button1.Width, button1.Height, 30, 30);
            button1.Region = Region.FromHrgn(handle);
            button2.Region = Region.FromHrgn(handle);
            handle = CreateRoundRectRgn(0, 0, panel1.Width, panel1.Height, 30, 30);
            panel1.Region = Region.FromHrgn(handle);

            CaptureDevice = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            FinalFrame = new VideoCaptureDevice();
            listBox2.Items.Clear();
            foreach (FilterInfo fi in CaptureDevice)
            {
                listBox2.Items.Add(fi.Name);
            }
            if (CaptureDevice.Count == 0)
                button1.Enabled = false;
            else
                listBox2.SelectedIndex = 0;

            DatabaseMethods.GetVaccineTypes().ForEach(x => 
            {
                listBox1.Items.Add(x.Name);
                vaccineTypes.Add(x.Name, x.ID);
            });

        }

        /// <summary>
        /// Function to start loading QR code
        /// </summary>
        /// <param name="sender"> variable for sender </param>
        /// <param name="e"> variable for event arguments </param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (FinalFrame != null)
            {
                label10.Text = "Reading QR";
                FinalFrame = new VideoCaptureDevice(CaptureDevice[listBox2.SelectedIndex].MonikerString);
                FinalFrame.NewFrame += new NewFrameEventHandler(FinalFrame_NewFrame);
                FinalFrame.Start();
            }
            else
                MessageBox.Show("I'm sorry, but you didn't have any input camera");
        }

        /// <summary>
        /// Function to load and save lastest QR code
        /// </summary>
        /// <param name="sender"> variable for sender </param>
        /// <param name="eventArgs"> variable for event arguments </param>
        private void FinalFrame_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            pictureBox2.Image = (Bitmap)eventArgs.Frame.Clone();

            try
            {
                Bitmap bitmap = new Bitmap(pictureBox2.Image);
                BarcodeReader reader = new BarcodeReader { AutoRotate = true};
                Result result = reader.Decode(bitmap);
                string decoded = result.ToString().Trim();        
                //capture a snapshot if there is a match
                if (FinalFrame.IsRunning == true)
                {
                    exitcamera();
                }
                string[] content = decoded.Split('_');
                if (content[0].Equals("CoronaTracker-by-nCodes.eu") && DatabaseMethods.IsPatientExist(Convert.ToInt32(content[1]), Convert.ToInt32(content[2]), Convert.ToInt32(content[3])))
                {
                    patient = DatabaseMethods.GetPatient(Convert.ToInt32(content[1]));
                    if (patient != null)
                    {
                        ((FindsSubSubForm)((PatientSubForm)ProgramVariables.ProgramUI.GetCurrentForm()).GetCurrentForm()).Invoke(setData);
                    }
                    else
                        MessageBox.Show("Patient does not exists!");
                }
            }
            catch (Exception ex)
            {
                label10.Text = ex.Message;
            }
        }

        /// <summary>
        /// Function to stop recording camera on form closing
        /// </summary>
        /// <param name="sender"> variable for sender </param>
        /// <param name="e"> variable for event arguments </param>
        private void VaccinationsSubSubForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (FinalFrame != null)
            {
                exitcamera();
            }
        }

        /// <summary>
        /// Function to stop recording camera
        /// </summary>
        private void exitcamera()
        {
            FinalFrame.SignalToStop();
            // FinalVideo.WaitForStop();  << marking out that one solved it
            FinalFrame.NewFrame -= new NewFrameEventHandler(FinalFrame_NewFrame); // as sugested
            FinalFrame = null;
        }

        /// <summary>
        /// Function to load patient by ID
        /// </summary>
        /// <param name="sender"> variable for sender </param>
        /// <param name="e"> variable for event arguments </param>
        private void button2_Click_1(object sender, EventArgs e)
        {
            if(numericUpDown1.Value > 0)
            {
                patient = DatabaseMethods.GetPatient((int)numericUpDown1.Value);
                if (patient != null)
                {
                    setDataMethod();
                }
                else
                    MessageBox.Show("Patient does not exists!");
            }
        }

        /// <summary>
        /// Function to add vaccinate
        /// </summary>
        /// <param name="sender"> variable for sender </param>
        /// <param name="e"> variable for event arguments </param>
        private void pictureBox6_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                if (MessageBox.Show("Are you sure to archive vaccinate?", "Database Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (!DatabaseMethods.IsPatientVaccinate(DatabaseMethods.GetPatientIDByPersonalNumber(patient.PersonalNumberFirst, patient.PersonalNumberSecond)))
                    {
                        int selectedType = vaccineTypes[listBox1.SelectedItem.ToString()];
                        DatabaseMethods.AddPatientVaccine(dateTimePicker1.Value, dateTimePicker2.Value, selectedType, DatabaseMethods.GetPatientIDByPersonalNumber(patient.PersonalNumberFirst, patient.PersonalNumberSecond), ProgramVariables.ID);
                        MessageBox.Show("Vaccine successfully archived!");
                    }
                    else
                    {
                        MessageBox.Show("Patient has already vaccinated!");
                    }
                }
            }
        }

        /// <summary>
        /// Function to create and open vaccinate PDF file
        /// </summary>
        /// <param name="sender"> variable for sender </param>
        /// <param name="e"> variable for event arguments </param>
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            int id = DatabaseMethods.GetPatientIDByPersonalNumber(patient.PersonalNumberFirst, patient.PersonalNumberSecond);
            GeneratePdfClass.GenerateVaccinatePdf(vaccine, patient, id);
            Process.Start("temp.pdf");
        }

        /// <summary>
        /// Function to automatically add days to second date
        /// </summary>
        /// <param name="sender"> variable for sender </param>
        /// <param name="e"> variable for event arguments </param>
        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            dateTimePicker2.Value = dateTimePicker1.Value.AddDays(40);
        }
    }
}
