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

        //Loads up memories for display. It uses the demo Child notes
        //if the user is not logged in
        public static List<MemoryRetrievalDBModel> ViewMemories()
        {

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

        //This is called by the MemoryController in the Controller folder
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
            MemoryDBModel data = new MemoryDBModel
            {
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

            //Calls the NoteCreation stored procedure in SQL
            SqlCommand cmd = new SqlCommand("NoteCreation", cnn);

            cmd.CommandType = CommandType.StoredProcedure;
            //Add parameters
            cmd.Parameters.AddWithValue("@ChildID", data.ChildID);
            cmd.Parameters.AddWithValue("@DateOccurred", data.DateOccurred);
            cmd.Parameters.AddWithValue("@Text", data.Text);
            cmd.Parameters.AddWithValue("@HasPhoto", data.HasPhoto);
            
            //This one is special since the path may be null. We must account for possibly passing a null value
            cmd.Parameters.Add(new SqlParameter("@PhotoLocation", SQLDataAccess.ToDBNull(data.PhotoLocation)));

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

        //Data to update a memory
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

            //Calls the NoteCreation stored procedure in SQL
            SqlCommand cmd = new SqlCommand("NoteUpdate", cnn);

            cmd.CommandType = CommandType.StoredProcedure;
            //Add parameters
            cmd.Parameters.AddWithValue("NoteID", data.NoteID);
            cmd.Parameters.AddWithValue("@ChildID", data.ChildID);
            cmd.Parameters.AddWithValue("@DateOccurred", data.DateOccurred);
            cmd.Parameters.AddWithValue("@Text", data.Text);
            cmd.Parameters.AddWithValue("@HasPhoto", data.HasPhoto);

            //This one is special since the path may be null. We must account for possibly passing a null value
            cmd.Parameters.Add(new SqlParameter("@PhotoLocation", SQLDataAccess.ToDBNull(data.PhotoLocation)));

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

        //Function to be used with SaveMemory in the HomeController
        //Saves the user inputted photo and can be adapted to save more than
        //one photo in the future
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

        //Function to delete selected note
        public static int DeleteNote(int NoteID)
        {
            MemoryDBModel data = new MemoryDBModel
            {
                NoteID = NoteID
            };

            string newConnectionString = SQLDataAccess.GetConnectionString();
            SqlConnection cnn = new SqlConnection(newConnectionString);
            cnn.Open();

            //Calls the NoteCreation stored procedure in SQL
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