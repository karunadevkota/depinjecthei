using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Models
{
    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string CurrentPassword { get; set; }
        [Required]
        [DataType (DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set;}
        [Required]
        [DataType (DataType.Password)]
        [Display(Name = "Confirm New Password")]
        [Compare("NewPassword" , ErrorMessage = " Your New Password and Confirm PAssword do not match")]
        public string ConfirmPassword { get; set;}
    }
}
