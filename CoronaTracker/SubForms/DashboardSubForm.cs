using CoronaTracker.Database;
using LiveCharts;
using LiveCharts.Wpf;
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

namespace CoronaTracker.SubForms
{
    public partial class DashboardSubForm : Form
    {

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

        public DashboardSubForm()
        {
            InitializeComponent();

            IntPtr handle = CreateRoundRectRgn(0, 0, panel1.Width, panel1.Height, 30, 30);
            panel1.Region = Region.FromHrgn(handle);
            panel2.Region = Region.FromHrgn(handle);
            panel6.Region = Region.FromHrgn(handle);
            panel8.Region = Region.FromHrgn(handle);

            handle = CreateRoundRectRgn(0, 0, panel5.Width, panel5.Height, 30, 30);
            panel5.Region = Region.FromHrgn(handle);

            label2.Text = DatabaseMethods.GetPatientsCount().ToString();
            label3.Text = DatabaseMethods.GetVacinnatePatientsCount().ToString();
            label8.Text = DatabaseMethods.GetConfirmedPatientsCount().ToString();
            label7.Text = DatabaseMethods.GetInfectionCount().ToString();
            
        }

        private void DashboardSubForm_Load(object sender, EventArgs e)
        {
            DateTime dt = DateTime.Now.AddMonths(-5);
            DateTime now = DateTime.Now;
            List<string> labels = new List<string>();

            while (true)
            {
                labels.Add(dt.ToString("MMM yyyy"));
                Console.Write(dt.ToString("yyyy-MM-dd HH:mm:ss") + " >> " + dt.AddMonths(1).ToString("yyyy-MM-dd HH:mm:ss"));
                dt = dt.AddMonths(1);
                if (DateTime.Compare(dt, now) > 0)
                {
                    break;
                }
            }

            cartesianChart1.AxisX.Add(new Axis
            {
                Title = "Months",
                Labels = labels
            });
            
            ChartValues<int> values = new ChartValues<int>();
            DatabaseMethods.GetInfections().ForEach(x =>
            {
                values.Add(x);
            });
            cartesianChart1.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Values",
                    Values = values
                }
            };
        }
    }
}
