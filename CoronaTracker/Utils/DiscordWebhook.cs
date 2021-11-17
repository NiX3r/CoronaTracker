using CoronaTracker.Instances;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace CoronaTracker.Utils
{
    class DiscordWebhook
    {

        private readonly WebClient dWebClient;
        private static NameValueCollection discordValues = new NameValueCollection();
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

        public string WebHook { get; set; }
        public string UserName { get; set; }
        public string ProfilePicture { get; set; }

        public DiscordWebhook()
        {
            dWebClient = new WebClient();
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

            discordValues.Add("username", UserName);
            discordValues.Add("avatar_url", ProfilePicture);
            discordValues.Add("content", msg);

            dWebClient.UploadValues(WebHook, discordValues);

            LogClass.Log("Discord webhook sent");

        }

        public void Dispose()
        {
            dWebClient.Dispose();
        }

    }
}
