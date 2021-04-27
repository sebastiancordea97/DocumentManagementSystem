using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TemplateTest.Models
{
    public class DepartmentViewModel
    {
        public int DepartmentId { get; set; }

        [DisplayName("Department name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "The department name can not be an empty string!")]
        [MaxLength(200, ErrorMessage = "The department name can not exceed 200 characters!")]
        public string DepartmentName { get; set; }
        [DisplayName("Supervisor ID")]
        public int Supervisor { get; set; }


    }
}
