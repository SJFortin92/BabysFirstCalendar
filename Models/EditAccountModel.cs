﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static BabysFirstCalendar.DatabaseBusinessLogic.EnumProcessor;

namespace BabysFirstCalendar.Models
{
    public class EditAccountModel
    {
        [Display(Name = "First name")]
        [Required(ErrorMessage = "Please enter your first name")]
        public string FirstName { get; set; }

        [Display(Name = "Last name")]
        [Required(ErrorMessage = "Please enter your last name")]
        public string LastName { get; set; }

        [Display(Name = "Phone number")]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        [Display(Name = "Email address")]
        //DataType.EmailAddress checks to make sure it's a valid email type
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Please enter your email")]
        public string Email { get; set; }

        [Display(Name = "Confirm email address")]
        [Compare("Email", ErrorMessage = "Email addresses must match")]
        public string ConfirmEmail { get; set; }

        [Display(Name = "Notification schedule")]
        public Notification NotificationSchedule { get; set; }
    }
}