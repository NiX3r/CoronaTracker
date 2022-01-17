using CoronaTracker.Database.DatabaseInstances;
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
        /// Function to generate underwent PDF file
        /// </summary>
        /// <param name="finds"> variable for finds list </param>
        /// <param name="patient"> variable for patient instance </param>
        /// <param name="patientID"> variable for patient ID </param>
        /// <returns>
        /// true : generate was success
        /// false : generate was unsuccess
        /// </returns>
        public static bool GenerateUnderwentPdf(List<FindsInstance> finds, PatientInstance patient, int patientID)
        {

            try
            {
                LogClass.Log($"Starting generating underwent PDF");
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
                LogClass.Log($"PDF successfully generated");
                return true;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                LogClass.Log($"PDF unsuccessfully generated. Error: {ex.Message}");
                return false;
            }

        }

        /// <summary>
        /// Function to generate vaccinate PDF file
        /// </summary>
        /// <param name="vaccine"> variable for vaccine instance </param>
        /// <param name="patient"> variable for patient instance </param>
        /// <param name="patientID"> variable for patient ID </param>
        /// <returns>
        /// true : generate was success
        /// false : generate was unsuccess
        /// </returns>
        public static bool GenerateVaccinatePdf(PatientVaccineAction vaccine, PatientInstance patient, int patientID)
        {
            try
            {
                LogClass.Log($"Starting generate vaccinate PDF");
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
                LogClass.Log($"PDF successfully generated");
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                LogClass.Log($"PDF unsuccessfully generated. Error: {ex.Message}");
                return false;
            }
        }

    }
}
