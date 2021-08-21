using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoronaTracker.Database.DatabaseInstances
{

    /// <summary>
    /// Instance for patient vaccine action info from database
    /// </summary>

    class PatientVaccineAction
    {
        public DateTime FirstDate { get; set; }
        public DateTime SecondDate { get; set; }
        public int VaccineType { get; set; }
        public string VaccineTypeString { get; set; }
        public int Employee { get; set; }
        public string EmployeeString { get; set; }

        public PatientVaccineAction(DateTime first, DateTime second, int vaccine, string vaccines, int employee, string employees)
        {
            this.FirstDate = first;
            this.SecondDate = second;
            this.VaccineType = vaccine;
            this.VaccineTypeString = vaccines;
            this.Employee = employee;
            this.EmployeeString = employees;
        }
    }
}
