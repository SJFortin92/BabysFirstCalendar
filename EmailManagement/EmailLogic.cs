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

        /// <summary>
        /// Pulls a list of users who have a NotificationSchedule int of
        /// 1 = Daily
        /// 2 = Weekly
        /// We do not pull those with a NotificationSchedule int of 0
        /// 0 = Never
        /// </summary>
        /// <returns></returns>

        public static List<AccountRemindersDBModel> PullNotificationAccounts()
        {

            string SQL = @"SELECT FirstName, Email, DateLastUsed, NotificationScheduleID
                            FROM Account
                            WHERE NotificationScheduleID=1
                            OR
                            NotificationScheduleID=2";

            return LoadData<AccountRemindersDBModel>(SQL);
        }


        /// <summary>
        /// This function scans the AccountRemindersDBModel to see if 
        /// A. The user is a daily or weekly reminder and
        /// B. If the user has not responded in 1 day (daily) or in 1 week (weekly)
        /// </summary>
        /// <param name="Account"></param>
        /// <returns> Returns true if account needs a reminder, false if not </returns>

        public static bool NeedsReminder(AccountRemindersDBModel Account)
        {

            //Set variables
            DateTime dateUsed = Account.DateLastUsed;
            int notifSched = Account.NotificationScheduleID;
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