using BabysFirstCalendar.DatabaseModels;
using BabysFirstCalendar.DataAccess;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Web;


namespace BabysFirstCalendar.DatabaseBusinessLogic
{
    public static class AccountProcessor
    {
        public static string HashPassword(string password)
        {
            //Salt and hash the password first

            //Create the salt
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

            //Get the hash value
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000);
            byte[] hash = pbkdf2.GetBytes(20);

            //Combine them to get the result
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);
            string passwordHash = Convert.ToBase64String(hashBytes);

            return passwordHash;
        }
        

        //This function returns the stored, hashed password from the database
        public static string FindPassword(string username)
        {
            string newConnectionString = SQLDataAccess.GetConnectionString();
            string SQL = @"SELECT Password.Password FROM Password
                        JOIN Account
                        ON Account.AccountID = Password.AccountID
                        WHERE Account.Email = '" + username +"'";
            using (SqlConnection cnn = new SqlConnection(newConnectionString))
            {
                SqlCommand command = new SqlCommand(SQL, cnn);
                cnn.Open();
                string hashedPassword = Convert.ToString(command.ExecuteScalar());
                cnn.Close();
                return hashedPassword;
            }
        }

        //This function compares the hashed password to the one given by the user
        public static bool ComparePassword(string userInputtedPassword, string username)
        {
            //Fetch the stored value
            string savedPasswordHash = FindPassword(username);
            //Extract the bytes
            byte[] hashBytes = Convert.FromBase64String(savedPasswordHash);
            //Get the salt
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);
           // Compute the hash on the password the user entered
            var pbkdf2 = new Rfc2898DeriveBytes(userInputtedPassword, salt, 100000);
            byte[] hash = pbkdf2.GetBytes(20);
            //Compare the results 
            for (int i = 0; i < 20; i++)
                if (hashBytes[i + 16] != hash[i])
                    return false;
            return true;
        }
    
        public static int CreateAccount(string firstName, string lastName,
            string email, int notificationSchedule, string password)
        {
            int notificationInt = Convert.ToInt32(notificationSchedule);
            string passwordToSave = HashPassword(password);

            AccountModel data = new AccountModel
            {
                FirstName = firstName,
                LastName = lastName,
                Password = passwordToSave,
                Email = email,
                NotificationSchedule = notificationInt
            };

            //Open a new SQL connection
            string newConnectionString = SQLDataAccess.GetConnectionString();
            SqlConnection cnn = new SqlConnection(newConnectionString);
            cnn.Open();
            
            SqlCommand cmd = new SqlCommand("AccountCreation", cnn);
            
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@FirstName", data.FirstName);
            cmd.Parameters.AddWithValue("@LastName", data.LastName);
            cmd.Parameters.AddWithValue("@Password", data.Password);
            cmd.Parameters.AddWithValue("@Email", data.Email);
            cmd.Parameters.AddWithValue("@NotificationSchedule", data.NotificationSchedule );
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

        public static int UpdateAccount(string FirstName, string LastName, string PhoneNumber,
            string CurrentEmail, string NewEmail, int Notification)
        {
            EditAccountDBModel data = new EditAccountDBModel
            {
                FirstName = FirstName,
                LastName = LastName,
                CurrentEmail = CurrentEmail,
                NewEmail =NewEmail,
                PhoneNumber = PhoneNumber,
                Notification = Notification
            };

 
            //Open a new SQL connection
            string newConnectionString = SQLDataAccess.GetConnectionString();
            SqlConnection cnn = new SqlConnection(newConnectionString);
            cnn.Open();

            SqlCommand cmd = new SqlCommand("AccountUpdate", cnn);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@FirstName", data.FirstName);
            cmd.Parameters.AddWithValue("@LastName", data.LastName);
            cmd.Parameters.AddWithValue("@CurrentEmail", data.CurrentEmail);
            cmd.Parameters.AddWithValue("@NewEmail", data.NewEmail);
            cmd.Parameters.AddWithValue("@Phone", data.PhoneNumber);
            cmd.Parameters.AddWithValue("@NotificationSchedule", data.Notification);
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



        //For future reference of the logic when we go to load notes
        //DO NOT USE THIS TO VIEW ALL ACCOUNTS - SECURITY HAZARD

        //To add into the home controller when viewing notes:
        /*public ActionResult ViewNotes()
         * {
         * ViewBag.Message = "Precious memories";
         * var data = ViewNotes();
         * List<NoteModel> notes = new List<NoteModel>();
         * 
         * foreach (var row in data)
         * {
         *         notes.Add(new NoteModel
         *         {
         *              Date = row.Text;
         *              Text = row.Text;
         *          });
         *  Return View(notes);
         * 
        
        
        The following to stay in the processor section. It is a list of all the account information

        public static List<NoteModel> LoadNotes()
        {
            string SQL = @"Select ChildID, FirstName, LastName from dbo.Note;";

            return SQLDataAccess.LoadData<NoteModel>(SQL);
        }

    */
    }
}