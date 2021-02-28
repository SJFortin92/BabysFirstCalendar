using BabysFirstCalendar.DataAccess;
using BabysFirstCalendar.DatabaseModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace BabysFirstCalendar.DatabaseBusinessLogic
{
    public static class ChildProcessor
    {
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

        //We call the RetrieveAccountID so we can get the child ID later
        public static int RetrieveChildID()
        {
            int AccountID = RetrieveAccountID();

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
        public static int CreateChild(string firstName, string lastName, DateTime DOB)
        {
            int AccountID = RetrieveAccountID();

            if (AccountID == 0)
            {
                return 0;
            }

            ChildDBModel data = new ChildDBModel
            {
                AccountID = AccountID,
                FirstName = firstName,
                LastName = lastName,
                DOB = DOB
            };

            string newConnectionString = SQLDataAccess.GetConnectionString();
            SqlConnection cnn = new SqlConnection(newConnectionString);
            cnn.Open();

            SqlCommand cmd = new SqlCommand("ChildCreation", cnn);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@AccountID", data.AccountID);
            cmd.Parameters.AddWithValue("@FirstName", data.FirstName);
            cmd.Parameters.AddWithValue("@LastName", data.LastName);
            cmd.Parameters.AddWithValue("@DOB", data.DOB);
            cmd.Parameters.Add("@Success", SqlDbType.Int, 1);
            cmd.Parameters.Add("@ErrorStatus", SqlDbType.Char, 50);
            cmd.Parameters["@ErrorStatus"].Direction = ParameterDirection.Output;
            cmd.Parameters["@Success"].Direction = ParameterDirection.Output;
            cmd.ExecuteNonQuery();
            int success = (int)cmd.Parameters["@Success"].Value;
            string message = (string)cmd.Parameters["@ErrorStatus"].Value;
            cnn.Close();

            return success;
        }
    }
}