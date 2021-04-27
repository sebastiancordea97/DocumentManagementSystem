using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace TemplateTest.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email Address is Mandatory!",AllowEmptyStrings = false)]
        [DisplayName("Email Address")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Please enter password", AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        [DisplayName("Password")]
        public string Password { get; set; }

        [DisplayName("Remember me?")]
        public bool IsPersistent { get; set; }



    }
}
