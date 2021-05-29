using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoronaTracker.Instances
{
    class CovidInfo
    {
        public String location { get; set; }
        public String country_code { get; set; }
        public float latitude { get; set; }
        public float longitude { get; set; }
        public int confirmed { get; set; }
        public int dead { get; set; }
        public int recovered { get; set; }
        public DateTime updated { get; set; }
    }
}
