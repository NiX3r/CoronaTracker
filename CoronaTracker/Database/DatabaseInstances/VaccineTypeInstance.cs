using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoronaTracker.Database.DatabaseInstances
{

    /// <summary>
    /// Instance for vaccine type info from database
    /// </summary>

    class VaccineTypeInstance
    {
        public int ID { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }

        public VaccineTypeInstance(int id, String name, String description)
        {
            this.ID = id;
            this.Name = name;
            this.Description = description;
        }
    }
}
