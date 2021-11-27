using CoronaTracker.Instances;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CoronaTracker.Utils
{
    class DiscordWebhook
    {

        //private readonly WebClient dWebClient;
        //private static NameValueCollection discordValues = new NameValueCollection();
        private const string example = "__**BUG REPORT**__\n" +
                                       "**Topic:** `%topic%`\n" +
                                       "**Type:** `%type%`\n" +
                                       "**Priority:** `%priority%`\n" +
                                       "**Create:** `%create%`\n" +
                                       "**OS:** `%os%`\n" +
                                       "**MAC:** ||`%mac%`||\n" +
                                       "**IP:** ||`%ip%`||\n" +
                                       "**Description:** ```%description%```\n" +
                                       "**Log:** ```%log%```";

        //public string WebHook { get; set; }

        public DiscordWebhook()
        {
            //dWebClient = new WebClient();
        }


        public void SendMessage(string topic, string type, int priority, DateTime create, string os, string description)
        {

            LogClass.Log("Sending discord webhook");

            var macAddr =
                (
                from nic in NetworkInterface.GetAllNetworkInterfaces()
                where nic.OperationalStatus == OperationalStatus.Up
                select nic.GetPhysicalAddress().ToString()
            ).FirstOrDefault();
            string externalIpString = new WebClient().DownloadString("http://icanhazip.com").Replace("\\r\\n", "").Replace("\\n", "").Trim();

            string msg = example;
            msg = msg.Replace("%topic%", topic)
                        .Replace("%type%", type)
                        .Replace("%priority%", priority.ToString())
                        .Replace("%create%", create.ToString("dd.MM.yyyy HH:mm"))
                        .Replace("%os%", os)
                        .Replace("%mac%", macAddr)
                        .Replace("%ip%", externalIpString)
                        .Replace("%description%", description)
                        .Replace("%log%", LogClass.GetLog());
            msg = msg.Replace("\"", "");

            MessageBox.Show(msg);

            /*discordValues.Add("content", msg);
            dWebClient.UploadValues(WebHook, discordValues);*/

            var httpRequest = (HttpWebRequest)WebRequest.Create(SecretClass.GetWebhookLink());
            httpRequest.Method = "POST";

            httpRequest.ContentType = "application/json";

            using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
            {
                streamWriter.Write(msg);
            }

            var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
            }

            LogClass.Log("Discord webhook sent");

        }

        /*public void Dispose()
        {
            dWebClient.Dispose();
        }*/

    }
}
