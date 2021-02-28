using BabysFirstCalendar.DataAccess;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace BabysFirstCalendar.DatabaseBusinessLogic
{
    public static class LayoutProcessor
    {
        public static string RetrieveName()
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                var username = HttpContext.Current.User.Identity.Name;

                string newConnectionString = SQLDataAccess.GetConnectionString();
                string SQL = @"SELECT FirstName FROM Account
                        WHERE Account.Email = '" + username + "'";
                using (SqlConnection cnn = new SqlConnection(newConnectionString))
                {
                    SqlCommand command = new SqlCommand(SQL, cnn);
                    cnn.Open();
                    string FirstName = Convert.ToString(command.ExecuteScalar());
                    cnn.Close();
                    return FirstName;
                }

            }
            else
                return null;
        }

        public static string RetrieveLastName()
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                var username = HttpContext.Current.User.Identity.Name;

                string newConnectionString = SQLDataAccess.GetConnectionString();
                string SQL = @"SELECT LastName FROM Account
                        WHERE Account.Email = '" + username + "'";
                using (SqlConnection cnn = new SqlConnection(newConnectionString))
                {
                    SqlCommand command = new SqlCommand(SQL, cnn);
                    cnn.Open();
                    string LastName = Convert.ToString(command.ExecuteScalar());
                    cnn.Close();
                    return LastName;
                }
            }
            else
                return null;
        }

        public static string RetrieveEmail()
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                var username = HttpContext.Current.User.Identity.Name;
                return username;
            }
            else
                return null;
        }

        public static string RetrievePhone()
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                var username = HttpContext.Current.User.Identity.Name;

                string newConnectionString = SQLDataAccess.GetConnectionString();
                string SQL = @"SELECT PhoneNumber FROM Account
                        WHERE Account.Email = '" + username + "'";
                using (SqlConnection cnn = new SqlConnection(newConnectionString))
                {
                    SqlCommand command = new SqlCommand(SQL, cnn);
                    cnn.Open();
                    string PhoneNumber = Convert.ToString(command.ExecuteScalar());
                    cnn.Close();
                    return PhoneNumber;
                }
            }
            else
                return null;
        }

        public static int RetrieveNotification()
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                var username = HttpContext.Current.User.Identity.Name;

                string newConnectionString = SQLDataAccess.GetConnectionString();
                string SQL = @"SELECT NotificationScheduleID FROM Account
                        WHERE Account.Email = '" + username + "'";
                using (SqlConnection cnn = new SqlConnection(newConnectionString))
                {
                    SqlCommand command = new SqlCommand(SQL, cnn);
                    cnn.Open();
                    int Notification = Convert.ToInt16(command.ExecuteScalar());
                    cnn.Close();
                    return Notification;
                }
            }
            else
                return 0;
        }

            
    }
}