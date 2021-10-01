using CoronaTracker.Database;
using CoronaTracker.Instances;
using CoronaTracker.Utils;
using Microsoft.Win32;
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

namespace CoronaTracker.SubForms
{
    public partial class ReportBug : Form
    {
        public ReportBug()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            // Checks if all parameters are wrote
            if(!textBox1.Text.Equals("") && !richTextBox1.Text.Equals("") && listBox1.SelectedItem != null)
            {

                string topic = textBox1.Text;
                string type = listBox1.SelectedItem.ToString();
                DateTime create = DateTime.Now;
                string os = FriendlyName();
                string description = richTextBox1.Text;
                int priority = (int)numericUpDown1.Value;
                List<string> lLog = new List<string>();
                string log = "";

                LogClass.Save();
                StreamReader sr = new StreamReader("log.txt");
                while (!sr.EndOfStream)
                {

                    string line = sr.ReadLine();

                    if (lLog.Count == 10)
                        lLog.RemoveAt(0);

                    lLog.Add(line);

                }

                lLog.ForEach(x => { log += "\n" + x; });
                log = log.Substring(1);

                ProgramVariables.Webhook.SendMessage(topic, type, priority, create, os);
                DatabaseMethods.AddBugReport(topic, type, priority, create, os, description, log);
                this.Close();
                MessageBox.Show("Thanks for report! We'll respond as soon as we possible!");

            }

        }

        public string HKLM_GetString(string path, string key)
        {
            try
            {
                RegistryKey rk = Registry.LocalMachine.OpenSubKey(path);
                if (rk == null) return "";
                return (string)rk.GetValue(key);
            }
            catch { return ""; }
        }

        public string FriendlyName()
        {
            string ProductName = HKLM_GetString(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName");
            string CSDVersion = HKLM_GetString(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CSDVersion");
            if (ProductName != "")
            {
                return (ProductName.StartsWith("Microsoft") ? "" : "Microsoft ") + ProductName +
                            (CSDVersion != "" ? " " + CSDVersion : "");
            }
            return "";
        }

        private void button7_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);
        private void ReportBug_MouseDown(object sender, MouseEventArgs e)
        {
            LogClass.Log($"ReportBug mouse down event handler start");
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
            LogClass.Log($"ReportBug mouse down event handler end");
        }
    }
}
