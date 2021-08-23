using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoronaTracker.Instances
{

    /// <summary>
    /// Instance for map rest api data
    /// </summary>

    class CovidInfo
    {
        public String country { get; set; }
        public String code { get; set; }
        public float latitude { get; set; }
        public float longitude { get; set; }
        public int confirmed { get; set; }
        public int deaths { get; set; }
        public int critical { get; set; }
        public int recovered { get; set; }
        public DateTime lastChange { get; set; }
        public DateTime lastUpdate { get; set; }
    }
}
