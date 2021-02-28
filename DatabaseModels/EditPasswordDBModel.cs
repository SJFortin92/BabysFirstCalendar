using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BabysFirstCalendar.DatabaseModels
{
    public class EditPasswordDBModel
    {
        public string Email { get; set; }

        public string NewPassword { get; set; }
    }
}