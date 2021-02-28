using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BabysFirstCalendar.Models
{
    public class DeleteAccountModel
    {
        [Display(Name = "Password")]
        [Required(ErrorMessage = "Please enter your password")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Please enter a password at least 8 characters long")]
        public string Password { get; set; }

        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Your passwords must match")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Please confirm deleting your account")]
        [Display(Name = "Confirm Deletion")]
        public bool ConfirmDelete { get; set; }
     }
}