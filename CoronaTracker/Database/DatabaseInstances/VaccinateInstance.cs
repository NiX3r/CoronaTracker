using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoronaTracker.Database.DatabaseInstances
{

    /// <summary>
    /// Instance for vaccinate instance from database
    /// Shorter version of PatientVaccineAction
    /// </summary>

    class VaccinateInstance
    {
        public DateTime FirstDate { get; set; }
        public DateTime SecondDate { get; set; }
        public string DoctorFullname { get; set; }

        public VaccinateInstance(DateTime first, DateTime second, string doctor)
        {
            FirstDate = first;
            SecondDate = second;
            DoctorFullname = doctor;
        }
    }
}
