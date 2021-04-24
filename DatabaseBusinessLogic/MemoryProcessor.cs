using BabysFirstCalendar.DataAccess;
using BabysFirstCalendar.DatabaseModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.Mvc;
using static BabysFirstCalendar.DatabaseBusinessLogic.RetrievalProcessor;
using static BabysFirstCalendar.DataAccess.SQLDataAccess;
using static BabysFirstCalendar.DatabaseBusinessLogic.InputProcessor;
using System.Web.Hosting;
using BabysFirstCalendar.Models;

namespace BabysFirstCalendar.DatabaseBusinessLogic
{

    //Class contains methods to load a list of memories, create a new memory in the DB, update a memory in the DB
    //and delete a memory from the DB

    public static class MemoryProcessor
    {
    
        //Struct to be used to create a new memory and edit one
        //Will be used to return when we save photos
        public struct SaveMemoryStruct
        {
            public string path { get; set; }
            public int fileSize { get; set; }
            public int hasPhoto { get; set; }
        }


        /// <summary>
        /// Loads up user (or demo) memories for display on the calendar
        /// Is called by the HomeController. 
        /// </summary>
        /// <returns>A list of memories</returns>
        
        public static List<MemoryRetrievalDBModel> ViewMemories()
        {
            //If the user is logged in..
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                int childID = RetrieveChildID();
                string SQL = @"SELECT N.NoteID, N.Text, N.Date, N.HasPhoto, P. PhotoLocationReference
                            FROM Note as N
                            LEFT JOIN Photo as P
                            ON N.NoteID = P.NoteID
                            WHERE ChildID = '" + childID + "'";

                return LoadData<MemoryRetrievalDBModel>(SQL);
            }

            //Otherwise, use the demo childID
            else
            {
                int childID = 10;
                string SQL = @"SELECT N.NoteID, N.Text, N.Date, N.HasPhoto, P. PhotoLocationReference
                            FROM Note as N
                            LEFT JOIN Photo as P
                            ON N.NoteID = P.NoteID
                            WHERE ChildID = '" + childID + "'";

                return LoadData<MemoryRetrievalDBModel>(SQL);
            }
        }


        /// <summary>
        /// The SaveMemory function in the HomeController processes
        /// if a note exists or not. If not, SaveMemory calls this method
        /// This method will save the new memory in SQL by calling
        /// a stored procedure.
        /// </summary>
        /// <param name="Date"></param>
        /// <param name="Text"></param>
        /// <param name="HasPhoto"></param>
        /// <param name="PhotoLocation"></param>
        /// <param name="PhotoSize"></param>
        /// <returns>A 1 if successful, 0 if not</returns>

        public static int CreateMemory(DateTime Date, string Text, int HasPhoto, string PhotoLocation, int PhotoSize)
        {
            //Calls RetrieveChildID from the RetrievalProcessor in DatabaseBusinessLogic
            int ChildID = RetrieveChildID();

            //If no child, return an error
            if (ChildID == 0)
            {
                return 0;
            }

            //Calls MemoryDBModel from DatabaseModels
            //Sets data that was given as a parameter to a DB Model
            MemoryDBModel data = new MemoryDBModel
            {
                ChildID = ChildID,
                DateOccurred = Date,
                Text = Text,
                HasPhoto = HasPhoto,
                PhotoLocation = PhotoLocation,
                PhotoSize = PhotoSize
            };

            //Opens the SQL connection
            string newConnectionString = SQLDataAccess.GetConnectionString();
            SqlConnection cnn = new SqlConnection(newConnectionString);
            cnn.Open();

            //Calls the NoteCreation stored procedure in SQL
            SqlCommand cmd = new SqlCommand("NoteCreation", cnn);

            cmd.CommandType = CommandType.StoredProcedure;
            //Add parameters
            cmd.Parameters.AddWithValue("@ChildID", data.ChildID);
            cmd.Parameters.AddWithValue("@DateOccurred", data.DateOccurred);
            cmd.Parameters.AddWithValue("@Text", data.Text);
            cmd.Parameters.AddWithValue("@HasPhoto", data.HasPhoto);
            
            //This one is special since the path may be null. We must account for possibly passing a null value
            //Use ToDBNull to return a value if not null, or a SQL approved null if null
            cmd.Parameters.Add(new SqlParameter("@PhotoLocation", InputProcessor.ToDBNull(data.PhotoLocation)));

            cmd.Parameters.AddWithValue("@PhotoSize", data.PhotoSize);
            cmd.Parameters.Add("@Success", SqlDbType.Int, 1);
            cmd.Parameters.Add("@ErrorStatus", SqlDbType.Char, 50);
            cmd.Parameters["@ErrorStatus"].Direction = ParameterDirection.Output;
            cmd.Parameters["@Success"].Direction = ParameterDirection.Output;
            cmd.ExecuteNonQuery();
            
            //Set success = @Success output from the procedure
            int success = (int)cmd.Parameters["@Success"].Value;
            string message = (string)cmd.Parameters["@ErrorStatus"].Value;
            cnn.Close();

            //Returns 1 if successful, 0 if not
            return success;
        }


        /// <summary>
        /// This is also called by SaveMemory in the HomeController.
        /// But this takes a NoteID as a parameter, allowing this method
        /// to update an already existing note. 
        /// Similar to CreateMemory, it uses a stored procedure to update
        /// in SQL
        /// </summary>
        /// <param name="NoteID"></param>
        /// <param name="Date"></param>
        /// <param name="Text"></param>
        /// <param name="HasPhoto"></param>
        /// <param name="PhotoLocation"></param>
        /// <param name="PhotoSize"></param>
        /// <returns>A 1 if successful, 0 if not</returns>

        public static int UpdateMemory(int NoteID, DateTime Date, string Text, int HasPhoto, string PhotoLocation, int PhotoSize)
        {
            //Calls RetrieveChildID from the RetrievalProcessor in DatabaseBusinessLogic
            int ChildID = RetrieveChildID();

            //If no child, return an error
            if (ChildID == 0)
            {
                return 0;
            }

            //Calls MemoryDBModel from DatabaseModels
            MemoryDBModel data = new MemoryDBModel
            {
                NoteID = NoteID,
                ChildID = ChildID,
                DateOccurred = Date,
                Text = Text,
                HasPhoto = HasPhoto,
                PhotoLocation = PhotoLocation,
                PhotoSize = PhotoSize
            };

            string newConnectionString = SQLDataAccess.GetConnectionString();
            SqlConnection cnn = new SqlConnection(newConnectionString);
            cnn.Open();

            //Calls the NoteUpdate stored procedure in SQL
            SqlCommand cmd = new SqlCommand("NoteUpdate", cnn);

            cmd.CommandType = CommandType.StoredProcedure;
            //Add parameters
            cmd.Parameters.AddWithValue("NoteID", data.NoteID);
            cmd.Parameters.AddWithValue("@ChildID", data.ChildID);
            cmd.Parameters.AddWithValue("@DateOccurred", data.DateOccurred);
            cmd.Parameters.AddWithValue("@Text", data.Text);
            cmd.Parameters.AddWithValue("@HasPhoto", data.HasPhoto);

            //This one is special since the path may be null. We must account for possibly passing a null value
            cmd.Parameters.Add(new SqlParameter("@PhotoLocation", InputProcessor.ToDBNull(data.PhotoLocation)));

            cmd.Parameters.AddWithValue("@PhotoSize", data.PhotoSize);
            cmd.Parameters.Add("@Success", SqlDbType.Int, 1);
            cmd.Parameters.Add("@ErrorStatus", SqlDbType.Char, 50);
            cmd.Parameters["@ErrorStatus"].Direction = ParameterDirection.Output;
            cmd.Parameters["@Success"].Direction = ParameterDirection.Output;
            cmd.ExecuteNonQuery();

            //Set success = @Success output from the procedure
            int success = (int)cmd.Parameters["@Success"].Value;
            string message = (string)cmd.Parameters["@ErrorStatus"].Value;
            cnn.Close();

            //Returns 1 if successful, 0 if not
            return success;
        }


        /// <summary>
        /// Saves the user inputted photo and can be adapted, eventually,
        /// to save more than one photo in the future.
        /// Is called by the SaveMemory function in the HomeController
        /// This does NOT validate the photo itself, as that is called before
        /// this function. Remember to validate the user upload BEFORE calling this
        /// </summary>
        /// <param name="userFiles">User uploaded files - AFTER VALIDATION</param>
        /// <returns>The SaveMemoryStruct (i.e. filepath, filesize, and hasPhoto)</returns>
        
        public static SaveMemoryStruct SavePhoto (HttpFileCollectionBase userFiles)
        {
            SaveMemoryStruct memory = new SaveMemoryStruct();

            HttpPostedFileBase photoUpload = userFiles[0];

            //hasPhoto is true, set it to 1
            memory.hasPhoto = 1;

            //Get the file size
            memory.fileSize = photoUpload.ContentLength / 1024;

            //Set a random filename and make a path
            string fileName = photoUpload.FileName;

            //Use HostingEnvironment.MapPath instead of Server.MapPath because we are in a static class
            memory.path = Path.Combine(HostingEnvironment.MapPath("~/upload/" + fileName));

            //Save the file
            photoUpload.SaveAs(memory.path);

            return memory;
        }


        /// <summary>
        /// This function is called by the HomeController.
        /// Will delete the selected note from SQL using
        /// a stored procedure
        /// </summary>
        /// <param name="NoteID"></param>
        /// <returns>A 1 if successful, 0 if not</returns>
        
        public static int DeleteNote(int NoteID)
        {
            MemoryDBModel data = new MemoryDBModel
            {
                NoteID = NoteID
            };

            //Connect to the database
            string newConnectionString = SQLDataAccess.GetConnectionString();
            SqlConnection cnn = new SqlConnection(newConnectionString);
            cnn.Open();

            //Calls the NoteDeletion stored procedure in SQL
            SqlCommand cmd = new SqlCommand("NoteDeletion", cnn);
            cmd.CommandType = CommandType.StoredProcedure;

            //Add parameters
            cmd.Parameters.AddWithValue("NoteID", data.NoteID);
            cmd.Parameters.Add("@Success", SqlDbType.Int, 1);
            cmd.Parameters.Add("@ErrorStatus", SqlDbType.Char, 50);
            cmd.Parameters["@ErrorStatus"].Direction = ParameterDirection.Output;
            cmd.Parameters["@Success"].Direction = ParameterDirection.Output;
            cmd.ExecuteNonQuery();

            //Set success = @Success output from the procedure
            int success = (int)cmd.Parameters["@Success"].Value;
            string message = (string)cmd.Parameters["@ErrorStatus"].Value;
            cnn.Close();

            //Returns 1 if successful, 0 if not
            return success;
        }
    }
}