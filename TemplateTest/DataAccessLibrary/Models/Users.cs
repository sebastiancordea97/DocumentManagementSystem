using System;
using System.Collections.Generic;

namespace DataAccessLibrary.Models
{
    public partial class Users
    {
        public Users()
        {
            Department = new HashSet<Department>();
            Documents = new HashSet<Documents>();
            InverseSupervisor = new HashSet<Users>();
        }

        public int UserId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Userrank { get; set; }
        public string UserFunction { get; set; }
        public byte[] PasswordHash { get; set; }
        public int? DepartmentId { get; set; }
        public int? SupervisorId { get; set; }
        public int? RoleId { get; set; }
        public string Email { get; set; }
        public byte[] Salt { get; set; }

        public virtual Department DepartmentNavigation { get; set; }
        public virtual UserRole Role { get; set; }
        public virtual Users Supervisor { get; set; }
        public virtual ICollection<Department> Department { get; set; }
        public virtual ICollection<Documents> Documents { get; set; }
        public virtual ICollection<Users> InverseSupervisor { get; set; }
    }
}
