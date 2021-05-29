using CoronaTracker.Instances;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CoronaTracker.Utils
{
    class RestAPI
    {

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

    }
}
