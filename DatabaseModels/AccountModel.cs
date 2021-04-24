using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BabysFirstCalendar.DatabaseModels
{
    public class AccountModel
    {
        public int AccountID { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public int NotificationSchedule { get; set; }

        public DateTime DateLastUsed { get; set; }

        public int DataUsed { get; set; }

        public int AccountType { get; set; }

        public string Password { get; set; }

    }
}