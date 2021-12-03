using CoronaTracker.Instances;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoronaTracker.Utils
{
    class RestAPI
    {

        private static string URL = "https://covid-19-data.p.rapidapi.com/";

        /// <summary>
        /// Function for load Covid data from rapidapi
        /// </summary>
        /// <param name="country"> variable for country name </param>
        /// <returns>
        /// return covid info variable
        /// </returns>
        public static async Task<CovidInfo> GetCovidDataAsync(String country)
        {
            LogClass.Log($"Start getting covid data by name {country}");
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(URL + "country?name=" + country),
                Headers =
                {
                    { "x-rapidapi-key", SecretClass.GetRestApiKey() },
                    { "x-rapidapi-host", SecretClass.GetRestApiHost()}
                }
            };
            using (var response = client.SendAsync(request))
            {
                String body = await response.Result.Content.ReadAsStringAsync();
                if (!body.Contains("message"))
                {
                    body = body.Replace("[", "").Replace("]", "");
                    //Debug.WriteLine(body);
                    LogClass.Log($"Successfully get covid data");
                    return JsonConvert.DeserializeObject<CovidInfo>(body);
                }
                LogClass.Log($"Unsuccessfully get covid data");
                return null;
            }
        }

    }
}
