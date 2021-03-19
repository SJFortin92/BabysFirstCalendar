using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

//We will use this to load a List from SQL to
//This is for reference when we go to send email
//reminders in the system

namespace BabysFirstCalendar.DatabaseModels
{
    public class AccountRemindersDBModel
    {
        public string FirstName { get; set; }
        public string Email { get; set; }
        public DateTime DateLastUsed { get; set; }
        public int NotificationScheduleID { get; set; }
        
    }
}