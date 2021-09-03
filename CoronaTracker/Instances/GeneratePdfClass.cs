﻿using CoronaTracker.Database.DatabaseInstances;
using CoronaTracker.Utils;
using OpenHtmlToPdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CoronaTracker.Instances
{
    class GeneratePdfClass
    {

        /// <summary>
        /// Function to 
        /// </summary>
        /// <param name="finds"></param>
        /// <param name="patient"></param>
        /// <param name="patientID"></param>
        /// <returns></returns>
        public static bool GenerateUnderwentPdf(List<FindsInstance> finds, PatientInstance patient, int patientID)
        {

            try
            {
                WebClient client = new WebClient();
                string url = "http://coronatracker.ncodes.eu/underwent.html";
                string qr = "https://api.qrserver.com/v1/create-qr-code/?size=128x128&data=" + "CoronaTracker-by-nCodes.eu_" + patientID + "_" + patient.PersonalNumberFirst + "_" + patient.PersonalNumberSecond;
                string loopModel = "<tr><td>%index%</td><td>%date%</td><td>%doctor%</td></tr>";
                string loop = "";
                string html = client.DownloadString(url);
                int index = 1;
                foreach(FindsInstance find in finds)
                {
                    string t = loopModel;
                    t = t.Replace("%index%", index.ToString())
                         .Replace("%date%", find.Found.ToString("dd.MM. yyyy HH:mm"))
                         .Replace("%doctor%", find.Employee);
                    loop += t;
                    index++;
                }

                html = html.Replace("%fullname%", patient.Fullname)
                           .Replace("%personalno1%", patient.PersonalNumberFirst)
                           .Replace("%personalno2%", patient.PersonalNumberSecond)
                           .Replace("%insurance%", patient.InsuranceCode.ToString())
                           .Replace("%loop%", loop)
                           .Replace("%time%", DateTime.Now.ToString("dd.MM. yyyy HH:mm"))
                           .Replace("%doctor%", ProgramVariables.Fullname)
                           .Replace("%img_link%", qr);

                var pdf = Pdf.From(html)
                         .WithoutOutline()
                         .WithMargins(0.01.Centimeters())
                         .WithObjectSetting("web.*", "true")
                         .Content();

                File.WriteAllBytes("temp.pdf", pdf);

                return true;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }

        }

        public static bool GenerateVaccinatePdf(PatientVaccineAction vaccine, PatientInstance patient, int patientID)
        {
            try
            {
                WebClient client = new WebClient();
                string url = "http://coronatracker.ncodes.eu/vaccinate.html";
                string qr = "https://api.qrserver.com/v1/create-qr-code/?size=128x128&data=" + "CoronaTracker-by-nCodes.eu_" + patientID + "_" + patient.PersonalNumberFirst + "_" + patient.PersonalNumberSecond;
                string html = client.DownloadString(url);

                html = html.Replace("%fullname%", patient.Fullname)
                           .Replace("%personalno1%", patient.PersonalNumberFirst)
                           .Replace("%personalno2%", patient.PersonalNumberSecond)
                           .Replace("%insurance%", patient.InsuranceCode.ToString())
                           .Replace("%vaccine%", vaccine.VaccineTypeString)
                           .Replace("%date1%", vaccine.FirstDate.ToString("dd.MM. yyyy HH:mm"))
                           .Replace("%doctor%", vaccine.EmployeeString)
                           .Replace("%date2%", vaccine.SecondDate.ToString("dd.MM. yyyy HH:mm"))
                           .Replace("%time%", DateTime.Now.ToString("dd.MM. yyyy HH:mm"))
                           .Replace("%doctor%", ProgramVariables.Fullname)
                           .Replace("%img_link%", qr);

                var pdf = Pdf.From(html)
                         .WithoutOutline()
                         .WithMargins(0.01.Centimeters())
                         .WithObjectSetting("web.*", "true")
                         .Content();

                File.WriteAllBytes("temp.pdf", pdf);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

    }
}
