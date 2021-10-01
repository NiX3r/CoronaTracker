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
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZXing;

namespace CoronaTracker.SubForms.PatientSubSubForms
{
    public partial class ListSubSubForm : Form
    {

        // Instance for capture video from camera
        private FilterInfoCollection CaptureDevice;
        // Instance for lastest camera frame
        private VideoCaptureDevice FinalFrame;
        // Instance for selected patient
        private PatientInstance patient;

        /// <summary>
        /// Load info about patient
        /// </summary>
        public delegate void setDataDelegate();
        public setDataDelegate setData;
        void setDataMethod() { label11.Text = "Catch QR"; textBox2.Text = patient.PersonalNumberFirst; textBox3.Text = patient.PersonalNumberSecond; textBox4.Text = patient.Fullname; textBox5.Text = patient.Email; textBox6.Text = patient.Phone.ToString(); textBox7.Text = patient.InsuranceCode.ToString(); richTextBox1.Text = patient.Description; }

        /// <summary>
        /// Constructor for list sub sub form
        /// </summary>
        public ListSubSubForm()
        {
            InitializeComponent();
            
            setData = new setDataDelegate(setDataMethod);

            IntPtr handle = CreateRoundRectRgn(0, 0, button1.Width, button1.Height, 30, 30);
            button1.Region = Region.FromHrgn(handle);
            button2.Region = Region.FromHrgn(handle);
            handle = CreateRoundRectRgn(0, 0, panel1.Width, panel1.Height, 30, 30);
            panel1.Region = Region.FromHrgn(handle);

            CaptureDevice = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            FinalFrame = new VideoCaptureDevice();
            listBox1.Items.Clear();
            foreach (FilterInfo fi in CaptureDevice)
            {
                listBox1.Items.Add(fi.Name);
            }
            if (CaptureDevice.Count == 0)
                button1.Enabled = false;
            else
                listBox1.SelectedIndex = 0;

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
        /// Function to add patient
        /// </summary>
        /// <param name="sender"> variable for sender </param>
        /// <param name="e"> variable for event arguments </param>
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if (!textBox2.Text.Equals("") && !textBox3.Text.Equals("") && !textBox4.Text.Equals("")
                 && !textBox5.Text.Equals("") && !textBox6.Text.Equals("") && !textBox7.Text.Equals(""))
            {
                if (MessageBox.Show("Are you sure to add patient?", "Database Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (DatabaseMethods.AddPatient(textBox2.Text, textBox3.Text, textBox4.Text, textBox5.Text, Convert.ToInt32(textBox6.Text), Convert.ToInt32(textBox7.Text), richTextBox1.Text))
                    {
                        PatientInstance type = new PatientInstance(textBox2.Text, textBox3.Text, textBox4.Text, textBox5.Text, Convert.ToInt32(textBox6.Text), Convert.ToInt32(textBox7.Text), richTextBox1.Text);
                        MessageBox.Show("Patient successfully added!");
                    }
                    else
                    {
                        MessageBox.Show("Patient with this person number already exists!");
                    }
                }
            }
            else
            {
                MessageBox.Show("All parameters with * needed!");
            }
        }

        /// <summary>
        /// Function to edit patient
        /// </summary>
        /// <param name="sender"> variable for sender </param>
        /// <param name="e"> variable for event arguments </param>
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (!textBox2.Text.Equals("") && !textBox3.Text.Equals("") && !textBox4.Text.Equals("")
                 && !textBox5.Text.Equals("") && !textBox6.Text.Equals("") && !textBox7.Text.Equals(""))
            {
                if (MessageBox.Show("Are you sure to edit patient?", "Database Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (DatabaseMethods.EditPatient(textBox2.Text, textBox3.Text, textBox4.Text, textBox5.Text,  Convert.ToInt32(textBox6.Text), Convert.ToInt32(textBox7.Text), richTextBox1.Text))
                    {
                        MessageBox.Show("Patient successfully edit!");
                    }
                    else
                    {
                        MessageBox.Show("Patient with this name doesnt exists!");
                    }
                }
            }
            else
            {
                MessageBox.Show("All parameters with * needed!");
            }
        }

        /// <summary>
        /// Function to remove patient
        /// </summary>
        /// <param name="sender"> variable for sender </param>
        /// <param name="e"> variable for event arguments </param>
        private void pictureBox3_Click(object sender, EventArgs e)
        {
            if (!textBox2.Text.Equals(""))
            {
                if (MessageBox.Show("Are you sure to remove patient?", "Database Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (DatabaseMethods.RemovePatient(textBox2.Text, textBox3.Text))
                    {
                        MessageBox.Show("Vaccine type successfully deleted!");
                    }
                    else
                    {
                        MessageBox.Show("Vaccine type with this name doesnt exists!");
                    }
                }
            }
            else
            {
                MessageBox.Show("Name is needed!");
            }
        }

        /// <summary>
        /// Function to allow only number
        /// </summary>
        /// <param name="sender"> variable for sender </param>
        /// <param name="e"> variable for event arguments </param>
        private void textBox6_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Function to load patient by ID
        /// </summary>
        /// <param name="sender"> variable for sender </param>
        /// <param name="e"> variable for event arguments </param>
        private void button2_Click(object sender, EventArgs e)
        {
            if (numericUpDown1.Value > 0)
            {
                patient = DatabaseMethods.GetPatient((int)numericUpDown1.Value);
                if (patient != null)
                {
                    textBox4.Text = patient.Fullname;
                    textBox2.Text = patient.PersonalNumberFirst;
                    textBox3.Text = patient.PersonalNumberSecond;
                    textBox5.Text = patient.Email;
                    textBox6.Text = patient.Phone.ToString();
                    textBox7.Text = patient.InsuranceCode.ToString();
                    richTextBox1.Text = patient.Description;
                }
                else
                    MessageBox.Show("Patient does not exists!");
            }
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
                label11.Text = "Reading QR";
                FinalFrame = new VideoCaptureDevice(CaptureDevice[listBox1.SelectedIndex].MonikerString);
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
            pictureBox5.Image = (Bitmap)eventArgs.Frame.Clone();

            try
            {
                Bitmap bitmap = new Bitmap(pictureBox5.Image);
                BarcodeReader reader = new BarcodeReader { AutoRotate = true };
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
                        ((ListSubSubForm)((PatientSubForm)ProgramVariables.ProgramUI.GetCurrentForm()).GetCurrentForm()).Invoke(setData);
                    }
                    else
                        MessageBox.Show("Patient does not exists!");
                }
            }
            catch (Exception ex)
            {
                label11.Text = ex.Source + "\n" + ex.Message;
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
        /// Function to stop recording camera on form closing
        /// </summary>
        /// <param name="sender"> variable for sender </param>
        /// <param name="e"> variable for event arguments </param>
        private void ListSubSubForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (FinalFrame != null)
                if (FinalFrame.IsRunning)
                    exitcamera();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            if(EmailWriter.Write(DatabaseMethods.GetPatientIDByPersonalNumber(textBox2.Text, textBox3.Text), Convert.ToInt32(textBox2.Text), Convert.ToInt32(textBox3.Text), textBox5.Text))
            {
                MessageBox.Show("Email has been successfully sent!");
            }
        }
    }
}
