using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DataAccessLibrary.Models;

namespace TemplateTest.Models
{
    public class UserViewModel
    {

        public int UserId { get; set; }

        [DisplayName("FirsName")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "FirstName is required and empty strings are not allowed!")]
        public string Firstname { get; set; }

        [DisplayName("LastName")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "LastName is required and empty strings are not allowed!")]
        public string Lastname { get; set; }

        [DisplayName("UserRank")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "UserRank is required and empty strings are not allowed!")]
        public string Userrank { get; set; }

        [DisplayName("UserFunction")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "UserFunction is required and empty strings are not allowed!")]
        public string Userfunction { get; set; }

        [DisplayName("Email")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "The email is required and empty strings are not allowed!")]
        public string Email { get; set; }

      //  [RegularExpression("^.*(?=.{8,})(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[@#!$%^&+=]).*$", ErrorMessage = "Password must be at least 8 characters long, must contain at least one one lower case letter, one upper case letter, one digit and one special character. Valid special characters are -   @#!$%^&+=")]
        public string OldPassword { get; set; }

        [DisplayName("Password")]
      //  [RegularExpression("^.*(?=.{8,})(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[@#!$%^&+=]).*$", ErrorMessage = "Password must be at least 8 characters long, must contain at least one one lower case letter, one upper case letter, one digit and one special character. Valid special characters are -   @#!$%^&+=")]
        public string Password { get; set; }

        public byte[] Salt { get; set; }

        [DisplayName("Department")]
        public int DepartmentId { get; set; }


        [DisplayName("Supervisor")]
        public int SupervisorId { get; set; }

      

        [DisplayName("User Role")]
        public int RoleId { get; set; }

    }
}
