using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CoronaTracker.Utils
{
    class EmailWriter
    {

        public static bool Write(int ID, int firstPersonalNumber, int secondPersonalNumber, string mailTo)
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
                message.From = new MailAddress("info@coronatracker.ncodes.eu");
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
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }

        }

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
                message.From = new MailAddress("info@coronatracker.ncodes.eu");
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
