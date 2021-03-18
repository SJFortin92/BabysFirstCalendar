using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static BabysFirstCalendar.DataAccess.SQLDataAccess;
using BabysFirstCalendar.DatabaseModels;

namespace BabysFirstCalendar.EmailManagement
{
    public static class EmailLogic
    {
        //Side note: We do not pull accounts with NotificationSchedule = 0
        //Because those are accounts who signed up to "never" be reminded
        //List of accounts who signed up for daily reminders
        //NotificationSchedule = 1 is the integer form of weekly reminders
        //NotificationSchedule = 2 is the integer form of weekly reminders
        public static List<AccountRemindersDBModel> PullNotificationAccounts()
        {

            string SQL = @"SELECT FirstName, Email, DateLastUsed, NotificationScheduleID
                            FROM Account
                            WHERE NotificationScheduleID=1
                            OR
                            NotificationScheduleID=2";

            List<AccountRemindersDBModel> listOfAccounts = LoadData<AccountRemindersDBModel>(SQL);
            return listOfAccounts;
        }

        public static bool NeedsReminder(AccountRemindersDBModel Account)
        {
            //Set variables
            DateTime dateUsed = Account.DateLastUsed;
            int notifSched = Account.NotificationSchedule;
            DateTime current = DateTime.Today;

            //Find how many days since last used
            TimeSpan difference = current - dateUsed;
            int numDays = Convert.ToInt32(difference.Days);

            if (notifSched == 1 && numDays > 1)
            {
                return true;
            }

            else if (notifSched == 2 && (numDays % 7) == 0)
            {
                return true;
            }

            else
                return false;
        }
    }
}