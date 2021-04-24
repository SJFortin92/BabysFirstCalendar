using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BabysFirstCalendar.DatabaseBusinessLogic
{
    public static class EnumProcessor
    {
        //To enable a drop-down table for Notifications in Account
        //Is called by the AccountModel and AccountController
        public enum Notification
        {
            Never,
            Daily,
            Weekly
        }
    }
}