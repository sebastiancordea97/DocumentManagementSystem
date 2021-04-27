using System;
using System.Collections.Generic;

namespace DataAccessLibrary.Models
{
    public partial class Department
    {
        public Department()
        {
            Users = new HashSet<Users>();
        }

        public int DepId { get; set; }
        public string DepName { get; set; }
        public int? Supervisor { get; set; }

        public virtual Users SupervisorNavigation { get; set; }
        public virtual ICollection<Users> Users { get; set; }
    }
}
