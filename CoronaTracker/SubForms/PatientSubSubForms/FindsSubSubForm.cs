﻿using AForge.Video;
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
    public partial class FindsSubSubForm : Form
    {

        // Instance for capture video from camera
        private FilterInfoCollection CaptureDevice;
        // Instance for lastest camera frame
        private VideoCaptureDevice FinalFrame;
        // Instance for selected patient
        private PatientInstance patient;
        // Instance for selected find
        private FindsInstance actualFind;
        // Instance for list of finds
        private List<FindsInstance> finds;
        // Instance for selected find button
        private Button actualFindButton;
        // Instance for help create finds buttons
        private int index2;

        /// <summary>
        /// Create finds buttons
        /// </summary>
        public delegate void addButtonDelegate();
        public addButtonDelegate addButton;
        void addButtonMethod() { index2 = 0; panel2.Controls.Clear(); finds.ForEach(x => addButton2(x)); }

        /// <summary>
        /// Load data into finds buttons
        /// </summary>
        public delegate void setDataDelegate();
        public setDataDelegate setData;
        void setDataMethod() { label2.Text = patient.Fullname; label3.Text = patient.PersonalNumberFirst; label6.Text = patient.PersonalNumberSecond; }

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
        /// Constuctor for finds sub sub form
        /// </summary>
        public FindsSubSubForm()
        {
            InitializeComponent();

            addButton = new addButtonDelegate(addButtonMethod);
            setData = new setDataDelegate(setDataMethod);

            IntPtr handle = CreateRoundRectRgn(0, 0, button1.Width, button1.Height, 30, 30);
            button1.Region = Region.FromHrgn(handle);
            button2.Region = Region.FromHrgn(handle);
            handle = CreateRoundRectRgn(0, 0, panel1.Width, panel1.Height, 30, 30);
            panel1.Region = Region.FromHrgn(handle);

            CaptureDevice = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            FinalFrame = null;
            finds = new List<FindsInstance>();
            buttonPattern2.Hide();
            index2 = 0;
            actualFind = null;

        }

        /// <summary>
        /// Function to add finds buttons 
        /// </summary>
        /// <param name="find"> variable for find info </param>
        private void addButton2(FindsInstance find)
        {
            String key = find.ID.ToString();
            Button bt = setDefaults2();
            bt.Name = key;
            bt.Text = "Found: " + find.Employee + "\nFound date: " + find.Found;
            this.panel2.Controls.Add(bt);
            ((Button)this.panel2.Controls[key]).Location = new Point(0, index2 * 56);
            //bt.BringToFront();
            ((Button)this.panel2.Controls[key]).Show();
            index2++;
        }

        /// <summary>
        /// Fucntion to set default style of button
        /// </summary>
        /// <returns>
        /// Return visual style of default finds button
        /// </returns>
        private Button setDefaults2()
        {
            Button output = new Button();
            output.Size = buttonPattern2.Size;
            output.BackColor = Color.FromArgb(55,55,55);
            output.FlatStyle = FlatStyle.Flat;
            output.FlatAppearance.BorderSize = 0;
            output.Font = new Font("MS Reference Sans Serif", 9.0f, FontStyle.Bold);
            output.ForeColor = Color.FromArgb(240, 240, 240);
            output.Click += button2_click;
            return output;
        }

        /// <summary>
        /// Function to load find info
        /// </summary>
        /// <param name="sender"> variable for sender </param>
        /// <param name="e"> variable for event arguments </param>
        private void button2_click(object sender, EventArgs e)
        {
            if(actualFindButton!=null) actualFindButton.BackColor = Color.FromArgb(55, 55, 55);
            actualFindButton = (Button)sender;
            actualFindButton.BackColor = Color.FromArgb(50,50,50);

            foreach (FindsInstance find in finds)
            {
                Debug.WriteLine($"Checking {((Button)sender).Name} equals to {find.ID}");
                if (((Button)sender).Name.Equals(find.ID.ToString()))
                {
                    actualFind = find;
                    return;
                }
            }
        }

        /// <summary>
        /// Function to add find to patient
        /// </summary>
        /// <param name="sender"> variable for sender </param>
        /// <param name="e"> variable for event arguments </param>
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if(patient != null)
            {
                if(MessageBox.Show("Are you sure to add finds to " + patient.Fullname + "?", "Action with database", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if(DatabaseMethods.AddFinds(patient.PersonalNumberFirst, patient.PersonalNumberSecond))
                    {
                        panel2.Controls.Clear();
                        index2 = 0;
                        finds = DatabaseMethods.GetFinds(patient.PersonalNumberFirst, patient.PersonalNumberSecond);
                        foreach (FindsInstance find in finds)
                        {
                            addButton2(find);
                        }
                        MessageBox.Show("Finds successfully added!");
                    }
                    else
                    {
                        MessageBox.Show("Sorry something really messed up!\nPlease, contact app developer!");
                    }
                }
            }
            else
            {
                MessageBox.Show("You must have picked patient to add finds.");
            }
        }

        /// <summary>
        /// Function to remove find from patient
        /// </summary>
        /// <param name="sender"> variable for sender </param>
        /// <param name="e"> variable for event arguments </param>
        private void pictureBox3_Click(object sender, EventArgs e)
        {

            if (actualFind != null)
            {
                if (MessageBox.Show("Are you sure to remove finds from " + patient.Fullname + "?", "Action with database", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (DatabaseMethods.RemoveFinds(actualFind.ID))
                    {
                        panel2.Controls.Clear();
                        index2 = 0;
                        finds = DatabaseMethods.GetFinds(patient.PersonalNumberFirst, patient.PersonalNumberSecond);
                        foreach (FindsInstance find in finds)
                        {
                            addButton2(find);
                        }
                        actualFind = null;
                        MessageBox.Show("Finds successfully removed!");
                    }
                    else
                    {
                        MessageBox.Show("Sorry something really messed up!\nPlease, contact app developer!");
                    }
                }
            }
            else
            {
                MessageBox.Show("You must have picked finds to remove it.");
            }
        }

        /// <summary>
        /// Function to load patient by ID
        /// </summary>
        /// <param name="sender"> variable for sender </param>
        /// <param name="e"> variable for event arguments </param>
        private void button2_Click_1(object sender, EventArgs e)
        {
            if (numericUpDown1.Value > 0)
            {
                patient = DatabaseMethods.GetPatient((int)numericUpDown1.Value);
                if (patient != null)
                {
                    label2.Text = patient.Fullname;
                    label3.Text = patient.PersonalNumberFirst.ToString();
                    label6.Text = patient.PersonalNumberSecond.ToString();
                    index2 = 0;
                    panel2.Controls.Clear();
                    finds = DatabaseMethods.GetFinds(patient.PersonalNumberFirst, patient.PersonalNumberSecond);
                    finds.ForEach(x => addButton2(x));
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
            if (FinalFrame == null)
            {
                FinalFrame = new VideoCaptureDevice(CaptureDevice[0].MonikerString);
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
            pictureBox4.Image = (Bitmap)eventArgs.Frame.Clone();
            try
            {
                Bitmap bitmap = new Bitmap((Bitmap)eventArgs.Frame.Clone());
                BarcodeReader reader = new BarcodeReader { AutoRotate = true };
                Result result = reader.Decode(bitmap);
                string decoded = result.ToString().Trim();

                Debug.WriteLine(decoded);
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
                        finds = DatabaseMethods.GetFinds(patient.PersonalNumberFirst, patient.PersonalNumberSecond);
                        ((FindsSubSubForm)((PatientSubForm)ProgramVariables.ProgramUI.GetCurrentForm()).GetCurrentForm()).Invoke(addButton);
                    }
                    else
                        MessageBox.Show("Patient does not exists!");
                }
            }
            catch (Exception ex)
            {
                label10.Text = ex.Source + "\n" + ex.Message;
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
        private void FindsSubSubForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (FinalFrame != null)
            {
                if (FinalFrame.IsRunning)
                {
                    exitcamera();
                }
            }
        }

        /// <summary>
        /// Function to create and open underwent PDF file
        /// </summary>
        /// <param name="sender"> variable for sender </param>
        /// <param name="e"> variable for event arguments </param>
        private void pictureBox1_Click(object sender, EventArgs e)
        {

            int id = DatabaseMethods.GetPatientIDByPersonalNumber(patient.PersonalNumberFirst, patient.PersonalNumberSecond);
            GeneratePdfClass.GenerateUnderwentPdf(finds, patient, id);
            Process.Start("temp.pdf");

        }
    }
}
