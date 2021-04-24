using BabysFirstCalendar.DataAccess;
using BabysFirstCalendar.DatabaseModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using static BabysFirstCalendar.DatabaseBusinessLogic.RetrievalProcessor;

namespace BabysFirstCalendar.DatabaseBusinessLogic
{
    //Will need to add an edit and delete function in here
    //Will also need to start supporting multiple children for one account
    public static class ChildProcessor
    {
        //This function is called by the ChildController in the Controller folder
        //Creates a child associated with the account
        public static int CreateChild(string firstName, string lastName, DateTime DOB)
        {
            //Calls RetrieveAccountID from the RetrievalProcessor in DatabaseBusinessLogic folder
            int AccountID = RetrieveAccountID();

            //If no AccountID exists, return an error
            if (AccountID == 0)
            {
                return 0;
            }

            //Calls the ChildDBModel from DatabaseModels folder
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

            //Calls the ChildCreation stored procedure
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

            //Sets success equal to @Success output
            int success = (int)cmd.Parameters["@Success"].Value;
            string message = (string)cmd.Parameters["@ErrorStatus"].Value;
            cnn.Close();

            //Returns 1 if successful, 0 if not
            return success;
        }
    }
}