using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoronaTracker.Database.DatabaseInstances
{

    /// <summary>
    /// Instance for employee info from database
    /// </summary>

    class EmployeeInstance
    {
        public String Fullname { get; set; }
        public String ProfilePictureURL { get; set; }
        public int Phone { get; set; }

        public EmployeeInstance (String Fullname, String ProfilePictureURL, int Phone)
        {
            this.Fullname = Fullname;
            this.ProfilePictureURL = ProfilePictureURL;
            this.Phone = Phone;
        }
    }
}
