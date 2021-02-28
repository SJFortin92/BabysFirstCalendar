using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BabysFirstCalendar.DatabaseModels
{
    public class ChildDBModel
    {
        public int AccountID { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime DOB { get; set; }
    }
}