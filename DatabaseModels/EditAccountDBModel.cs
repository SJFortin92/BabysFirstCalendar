using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BabysFirstCalendar.DatabaseModels
{
    public class EditAccountDBModel
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }
        
        public string PhoneNumber { get; set; }

        public string CurrentEmail { get; set; }
        
        public string NewEmail { get; set; }
        
        public int Notification { get; set; }

    }
}