using System;
using System.Collections.Generic;

namespace DataAccessLibrary.Models
{
    public partial class UserRole
    {
        public UserRole()
        {
            Users = new HashSet<Users>();
        }

        public int UserRoleId { get; set; }
        public string Role { get; set; }

        public virtual ICollection<Users> Users { get; set; }
    }
}
