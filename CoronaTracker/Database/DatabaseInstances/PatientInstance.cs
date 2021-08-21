using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoronaTracker.Database.DatabaseInstances
{

    /// <summary>
    /// Instance for patient info from database
    /// </summary>

    class PatientInstance
    {
        public String PersonalNumberFirst { get; set; }
        public String PersonalNumberSecond { get; set; }
        public String Fullname { get; set; }
        public String Email { get; set; }
        public int Phone { get; set; }
        public int InsuranceCode { get; set; }
        public String Description { get; set; }

        public PatientInstance(String numberFirst, String numberSecond, String fullname, String email, int phone, int incuranceCode, String description)
        {
            PersonalNumberFirst = numberFirst;
            PersonalNumberSecond = numberSecond;
            Fullname = fullname;
            Email = email;
            Phone = phone;
            InsuranceCode = incuranceCode;
            Description = description;
        }

    }
}
