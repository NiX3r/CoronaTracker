using CoronaTracker.Instances;
using Discord;
using Discord.Webhook;
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

        private DiscordWebhookClient DCW;

        public DiscordWebhook()
        {
            DCW = new DiscordWebhookClient(SecretClass.GetWebhookLink());
        }

        public async void SendMessage(string topic, string type, int priority, DateTime create, string os, string description)
        {

            LogClass.Log("Sending discord webhook");

            var macAddr =
                (
                from nic in NetworkInterface.GetAllNetworkInterfaces()
                where nic.OperationalStatus == OperationalStatus.Up
                select nic.GetPhysicalAddress().ToString()
            ).FirstOrDefault();
            string externalIpString = new WebClient().DownloadString("http://icanhazip.com").Replace("\\r\\n", "").Replace("\\n", "").Trim();

            var eb = new EmbedBuilder();
            eb.AddField("Topic", "`" + topic + "`");
            eb.AddField("Type", "`" + type + "`", true);
            eb.AddField("Priority", "`" + priority.ToString() + "`", true);
            eb.AddField("Create", "`" + create.ToString("dd.MM.yyyy HH:mm") + "`", false);
            eb.AddField("OS", "`" + os + "`", false);
            eb.AddField("MAC", "`" + macAddr + "`", true);
            eb.AddField("IP", "`" + externalIpString + "`", true);
            eb.AddField("Description", "`" + description + "`", false);
            eb.WithColor(121, 0, 255);

            // Here you make an array with 1 entry, which is the embed ( from EmbedBuilder.Build() )
            Embed[] embedArray = new Embed[] { eb.Build() };

            // Now you pass it into the method like this: 'embeds: embedArray'
            await DCW.SendFileAsync(filePath: "./logger.log", text: "", false, embeds: embedArray, $"({ProgramVariables.ID}) {ProgramVariables.Fullname}", ProgramVariables.ProfileURL);

        }

    }
}
