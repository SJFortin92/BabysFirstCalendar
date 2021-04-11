using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BabysFirstCalendar.Models
{
    public class ContactModel
    {
        [Display(Name = "First name")]
        [Required(ErrorMessage = "Please enter your first name")]
        public string FirstName { get; set; }

        [Display(Name = "Email address")]
        //DataType.EmailAddress checks to make sure it's a valid email type
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Please enter your email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please add a subject to this message")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Please enter a message")]
        public string Message { get; set; }

    }
}