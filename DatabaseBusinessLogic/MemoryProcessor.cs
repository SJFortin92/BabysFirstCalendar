﻿using BabysFirstCalendar.DataAccess;
using BabysFirstCalendar.DatabaseModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using static BabysFirstCalendar.DatabaseBusinessLogic.RetrievalProcessor;
using static BabysFirstCalendar.DataAccess.SQLDataAccess;

namespace BabysFirstCalendar.DatabaseBusinessLogic
{
    //Need to add Update and Delete in here
    public static class MemoryProcessor
    {
        //Loads up memories for display. It uses the demo Child notes
        //if the user is not logged in
        public static List<MemoryRetrievalDBModel> ViewMemories()
        {

            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                int childID = RetrieveChildID();
                string SQL = @"SELECT NoteID, Text, Date, HasPhoto
                            FROM Note
                            WHERE ChildID = '" + childID + "'";

                return LoadData<MemoryRetrievalDBModel>(SQL);
            }

            else
            {
                int childID = 10;
                string SQL = @"SELECT NoteID, Text, Date, HasPhoto
                            FROM Note
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