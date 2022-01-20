using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Windows;

namespace CoronaTracker.Utils
{

    /// <summary>
    /// 
    /// Utils class for sending emails
    /// 
    /// </summary>

    class EmailWriter
    {

        private static int ID, firstPersonalNumber, secondPersonalNumber;
        private static string mailTo;

        /// <summary>
        /// Function to write an email async
        /// </summary>
        /// <param name="aID"> variable for user's ID </param>
        /// <param name="afirstPersonalNumber"> variable for first personal number </param>
        /// <param name="asecondPersonalNumber"> variable for second personal number </param>
        /// <param name="amailTo"> variable for email write to </param>
        public static void AsyncWrite(int aID, int afirstPersonalNumber, int asecondPersonalNumber, string amailTo)
        {

            ID = aID;
            firstPersonalNumber = afirstPersonalNumber;
            secondPersonalNumber = asecondPersonalNumber;
            mailTo = amailTo;

            Thread mailer = new Thread(new ThreadStart(Write));
            mailer.Start();
        }

        /// <summary>
        /// Function to write an email
        /// </summary>
        static void Write()
        {

            string pictureUrl = "https://api.qrserver.com/v1/create-qr-code/?size=512x512&data=" + "CoronaTracker-by-nCodes.eu_" + ID + "_" + firstPersonalNumber + "_" + secondPersonalNumber;
            string head = "Your CoronaTracker QR Code!";
            string body = "Good day,<br>" +
                          "Your CoronaTracker QR code is here! Please don't lose it. Your QR code is in the attachments.<br>" +
                          $"<br>" +
                          $"Please do not answer to this email. It's automatically generated!<br>" +
                          $"If you have any problems, please notify us via email <i>support@coronatracker.ncodes.eu</i><br>" +
                          $"<br>" +
                          $"Best wishes,<br>" +
                          $"Your CoronaTracker Team!";

            using (WebClient client = new WebClient())
            {
                client.DownloadFile(new Uri(pictureUrl), "temp.jpg");
            }

            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                Attachment attachment = new Attachment("temp.jpg");
                message.From = new MailAddress(SecretClass.GetEmailEmail());
                message.To.Add(new MailAddress(mailTo));
                message.Subject = head;
                message.Attachments.Add(attachment);
                message.IsBodyHtml = true;
                message.Body = body;
                smtp.Port = SecretClass.GetEmailPort();
                smtp.Host = SecretClass.GetEmailServer();
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(SecretClass.GetEmailEmail(), SecretClass.GetEmailPassword());
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(message);
                attachment.Dispose();
                MessageBox.Show("You successfully sent an email!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        /// <summary>
        /// Function to write an forget code email async
        /// </summary>
        /// <param name="email"> variable for email write to </param>
        /// <param name="code"> variable for forget code </param>
        /// <returns></returns>
        public static bool WriteCodeEmail(string email, string code)
        {

            string head = "Your CoronaTracker Reset Password Code!";
            string body = "Good day,<br>" +
                          "Your CoronaTracker reset password code is here! Type it in app.<br>" +
                          $"Code: <i>" + code + "</i><br>" +
                          $"Please do not answer to this email. It's automatically generated!<br>" +
                          $"If you have any problems, please notify us via email <i>support@coronatracker.ncodes.eu</i><br>" +
                          $"<br>" +
                          $"Best wishes,<br>" +
                          $"Your CoronaTracker Team!";

            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress(SecretClass.GetEmailEmail());
                message.To.Add(new MailAddress(email));
                message.Subject = head;
                message.IsBodyHtml = true;
                message.Body = body;
                smtp.Port = SecretClass.GetEmailPort();
                smtp.Host = SecretClass.GetEmailServer();
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(SecretClass.GetEmailEmail(), SecretClass.GetEmailPassword());
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(message);
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
