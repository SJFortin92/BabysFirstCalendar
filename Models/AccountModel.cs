using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BabysFirstCalendar.Models
{
    //To enable a drop-down table for Notifications
    public enum Notification
    {
        Never,
        Daily,
        Weekly
    }
    public class AccountModel
    {
        [Display(Name = "First name")]
        [Required(ErrorMessage = "Please enter your first name")]
        public string FirstName { get; set; }

        [Display(Name = "Last name")]
        [Required(ErrorMessage = "Please enter your last name")]
        public string LastName { get; set; }

        [Display(Name = "Email address")]
        //DataType.EmailAddress checks to make sure it's a valid email type
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Please enter your email")]
        public string Email { get; set; }

        [Display(Name = "Confirm email address")]
        [Compare("Email", ErrorMessage = "Email addresses must match")]
        public string ConfirmEmail { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "Please enter your password")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Please enter a password at least 8 characters long")]
        public string Password { get; set; }

        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Your passwords must match")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Notification schedule")]
        public Notification NotificationSchedule { get; set; }

    }

}