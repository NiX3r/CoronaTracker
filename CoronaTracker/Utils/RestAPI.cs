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

namespace CoronaTracker.Utils
{
    class RestAPI
    {

        /// <summary>
        /// Function for load Covid data from trackcorona
        /// Does not work yet!
        /// </summary>
        /// <returns>
        /// return nothing
        /// </returns>
        public static async Task GetCovidDataAsync()
        {
            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(new HttpMethod("GET"), "https://www.trackcorona.live/api/countries"))
                {
                    var response = await httpClient.SendAsync(request);
                    ProgramVariables.CovidData = JsonConvert.DeserializeObject<List<CovidInfo>>(response.Content.ReadAsStringAsync().Result.Replace("{\"code\": 200, \"data\": ", "").Replace("}]}", "}]"));
                }
            }
        }

        /// <summary>
        /// Function for load Covid data from rapidapi
        /// </summary>
        /// <param name="country"> variable for country name </param>
        /// <returns>
        /// return covid info variable
        /// </returns>
        public static async Task<CovidInfo> GetCovidDataAsync(String country)
        {
            var client = new HttpClient();
            var request = SecretClass.GetRestInfo(country);
            using (var response = client.SendAsync(request))
            {
                String body = await response.Result.Content.ReadAsStringAsync();
                if (!body.Contains("message"))
                {
                    body = body.Replace("[", "").Replace("]", "");
                    //Debug.WriteLine(body);
                    return JsonConvert.DeserializeObject<CovidInfo>(body);
                }
                return null;
            }
        }

    }
}
