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
        //Function to hash passwords
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
            //Connect to SQL and select the Password where the account email = the user's email
            //Join the two tables on AccountID
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
        //Returns true if the passwords match
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
        
        //Function called by AccountController in the Controller folder
        public static int CreateAccount(string firstName, string lastName,
            string email, int notificationSchedule, string password)
        {
            //Ensure the notificationInt is actually an integer and not enum
            int notificationInt = Convert.ToInt32(notificationSchedule);

            //Hash the password given
            string passwordToSave = HashPassword(password);

            //This calls the AccountModel in the DatabaseModels, NOT the regular models
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
            
            //Call the AccountCreation stored procedure
            SqlCommand cmd = new SqlCommand("AccountCreation", cnn);
            
            cmd.CommandType = CommandType.StoredProcedure;

            //Add the parameters
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

            //Set success equal to the @Success outputted by the stored procedure
            int success = (int)cmd.Parameters["@Success"].Value;

            string message = (string)cmd.Parameters["@ErrorStatus"].Value;

            //Close the connection
            cnn.Close();

            //Return success (0 for an error, and 1 for it being ok)
            return success;
        }

        //Function called by AccountController in the Controller folder
        //Updates an account
        public static int UpdateAccount(string FirstName, string LastName, string PhoneNumber,
            string CurrentEmail, string NewEmail, int Notification)
        {
            //Uses EditAccountDBModel in the DatabaseModels folder
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

            //Call the AccountUpdate stored procedure
            SqlCommand cmd = new SqlCommand("AccountUpdate", cnn);

            cmd.CommandType = CommandType.StoredProcedure;

            //Add the parameters
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

            //Set success equal to the @Success output
            int success = (int)cmd.Parameters["@Success"].Value;
            string message = (string)cmd.Parameters["@ErrorStatus"].Value;

            //Close connection
            cnn.Close();

            //Returns 1 if successful, 0 if not
            return success;
        }

        //Function called by AccountController in the Controller folder
        //Updates a user's password
        public static int UpdatePassword(string CurrentEmail, string CurrentPassword, string NewPassword)
        {
            //Make sure that the current password matches the user entered one, if so, continue
            if (ComparePassword(CurrentPassword, CurrentEmail))
            {
                //Hash the new password
                NewPassword = HashPassword(NewPassword);

                //Calls EditPasswordDBModel in the DatabaseModel folder
                EditPasswordDBModel data = new EditPasswordDBModel
                {
                    Email = CurrentEmail,
                    NewPassword = NewPassword
                };


                //Open a new SQL connection
                string newConnectionString = SQLDataAccess.GetConnectionString();
                SqlConnection cnn = new SqlConnection(newConnectionString);
                cnn.Open();

                //Calls the PasswordUpdate stored procedure in SQL
                SqlCommand cmd = new SqlCommand("PasswordUpdate", cnn);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CurrentEmail", data.Email);
                cmd.Parameters.AddWithValue("@NewPassword", data.NewPassword);
                cmd.Parameters.Add("@Success", SqlDbType.Int, 1);
                cmd.Parameters.Add("@ErrorStatus", SqlDbType.Char, 50);
                cmd.Parameters["@ErrorStatus"].Direction = ParameterDirection.Output;
                cmd.Parameters["@Success"].Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();

                //Sets success equal to @Success output
                int success = (int)cmd.Parameters["@Success"].Value;
                string message = (string)cmd.Parameters["@ErrorStatus"].Value;

                //Close the connection
                cnn.Close();

                //Returns 1 if successful, 0 if not
                return success;
            }

            else
                return 0;
        }

        //Is called by AccountController in the Controller folder
        //Deletes an account entirely, included associated children, memories, and photos
        public static int AccountDelete(string CurrentEmail, string Password)
        {
            //Ensure that the given password matches the stored password
            if (ComparePassword(Password, CurrentEmail))
            {
                //Calls DeleteAccountDBModel from the DatabaseModels folder
                DeleteAccountDBModel data = new DeleteAccountDBModel
                {
                    CurrentEmail = CurrentEmail,
                };


                //Open a new SQL connection
                string newConnectionString = SQLDataAccess.GetConnectionString();
                SqlConnection cnn = new SqlConnection(newConnectionString);
                cnn.Open();

                //Calls the AccountDeletion stored procedure
                SqlCommand cmd = new SqlCommand("AccountDeletion", cnn);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CurrentEmail", data.CurrentEmail);
                cmd.Parameters.Add("@Success", SqlDbType.Int, 1);
                cmd.Parameters.Add("@ErrorStatus", SqlDbType.Char, 50);
                cmd.Parameters["@ErrorStatus"].Direction = ParameterDirection.Output;
                cmd.Parameters["@Success"].Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();

                //Set success equal to the @Success output
                int success = (int)cmd.Parameters["@Success"].Value;
                string message = (string)cmd.Parameters["@ErrorStatus"].Value;

                cnn.Close();

                //Return 1 if successful, 0 if not
                return success;
            }

            else
                //If the password doesn't match, return 0 for a failure
                return 0;
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