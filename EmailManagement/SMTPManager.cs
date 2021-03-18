using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using BabysFirstCalendar.DatabaseModels;
using static BabysFirstCalendar.EmailManagement.EmailLogic;

//Please note that SMTP emails are not recommended for security purposes
//Do not use this method in live production

namespace BabysFirstCalendar.EmailManagement
{
    public class SMTPManager
    {
        public void ComposeMessage()
        {

        }

        //Loops through each of the accounts and sends an email if 
        //the account needs a reminder
        public void IterateAccounts()
        {
            List<AccountRemindersDBModel> listOfAccounts = PullNotificationAccounts();
            foreach (var account in listOfAccounts)
            {
                if (NeedsReminder(account))
                {
                    //Insert information to send an email here
                }
            }
        }
    }
}