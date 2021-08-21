using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoronaTracker.Database.DatabaseInstances
{

    /// <summary>
    /// Instance for finds of patient from database
    /// </summary>

    class FindsInstance
    {
        public int ID { get; }
        public DateTime Found { get; }
        public String Employee { get; }

        public FindsInstance(int id, DateTime found, String employee)
        {
            ID = id;
            Found = found;
            Employee = employee;
        }
    }
}
