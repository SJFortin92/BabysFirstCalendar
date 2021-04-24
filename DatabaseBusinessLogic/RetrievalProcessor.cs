using BabysFirstCalendar.DataAccess;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;

namespace BabysFirstCalendar.DatabaseBusinessLogic
{
    public static class RetrievalProcessor
    {

        //Get the Javascript-friendly path for a user uploaded photo from SQL
        
        public static string RetrieveRelativeImagePath(string originalPath)
        {
            //Split up the directories
            string[] directories = originalPath.Split(Path.DirectorySeparatorChar);
            //Get the number of directoires
            int numDirectories = directories.Length;

            //We only want the file name, since we know it's in the upload folder. That would
            //be the last directory in the path.
            string pathToReturn = Path.Combine(@"../upload/", directories[numDirectories-1]);
            return pathToReturn;
        }


        //Gets the current user's email
        
        public static string RetrieveEmail()
        {
            //If the user is logged in
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                var username = HttpContext.Current.User.Identity.Name;
                return username;
            }
            else
                return null;
        }


        //Gets the current user's first name

        public static string RetrieveName()
        {
            //If the user is logged in
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                //Retrieves the username associated with the account
                var username = RetrieveEmail();

                string newConnectionString = SQLDataAccess.GetConnectionString();

                //Selects the FirstName where the AccountEmail equals the current user's email
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


        //RetrieveName function overload that takes the username as a parameter
        
        public static string RetrieveName(string username)
        {
            string newConnectionString = SQLDataAccess.GetConnectionString();

            //Selects the FirstName where the AccountEmail equals the current user's email
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


        // Gets the current user's last name
        
        public static string RetrieveLastName()
        {
            //If the user is logged in
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                //Get the current user's email
                var username = RetrieveEmail();

                //Select the last name where the Account.Email equals the user's current email
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


        //Gets the current user's phone number
       
        public static string RetrievePhone()
        {
            //If the user is currently logged in
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                //Get the user's current email
                var username = RetrieveEmail();

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


        //Gets the current user's notification schedule as an int

        public static int RetrieveNotification()
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                //Get the user's current email
                var username = RetrieveEmail();

                //Retrieve the NotificationScheduleID (int) where the emails match
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


        //Gets the current user's account ID
        //If RetrieveAccountID returns 0, we will throw an error
        
        public static int RetrieveAccountID()
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                var username = HttpContext.Current.User.Identity.Name;

                string newConnectionString = SQLDataAccess.GetConnectionString();
                string SQL = @"SELECT AccountID FROM Account
                        WHERE Account.Email = '" + username + "'";
                using (SqlConnection cnn = new SqlConnection(newConnectionString))
                {
                    SqlCommand command = new SqlCommand(SQL, cnn);
                    cnn.Open();
                    int AccountID = Convert.ToInt32(command.ExecuteScalar());
                    cnn.Close();
                    return AccountID;
                }

            }
            else
                return 0;
        }


        //Returns the ChildID associated with the account
        //Please note, this WILL pull all child IDs associated with the account
        //Will need to eventually be able to filter children based on user selection
        
        public static int RetrieveChildID()
        {
            //Get the associated AccountID
            int AccountID = RetrieveAccountID();

            //If the AccountID is not 0, which means there is an account
            if (AccountID != 0)
            {
                string newConnectionString = SQLDataAccess.GetConnectionString();
                string SQL = @"SELECT ChildID FROM Child
                        WHERE AccountID = '" + AccountID + "'";
                using (SqlConnection cnn = new SqlConnection(newConnectionString))
                {
                    SqlCommand command = new SqlCommand(SQL, cnn);
                    cnn.Open();
                    int ChildID = Convert.ToInt32(command.ExecuteScalar());
                    cnn.Close();
                    return ChildID;
                }

            }
            else
                return 0;
        }
    }
}