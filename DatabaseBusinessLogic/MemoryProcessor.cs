using BabysFirstCalendar.DataAccess;
using BabysFirstCalendar.DatabaseModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using static BabysFirstCalendar.DatabaseBusinessLogic.ChildProcessor;

namespace BabysFirstCalendar.DatabaseBusinessLogic
{
    public static class MemoryProcessor
    {
        public static bool CheckFileType(string fileName)
        {
            string ext = Path.GetExtension(fileName);
            switch (ext.ToLower())
            {
                case ".jpg":
                    return true;
                case ".jpeg":
                    return true;
                case ".png":
                    return true;
                default:
                    return false;
            }
        }
        public static int CreateMemory(DateTime Date, string Text, int HasPhoto, string PhotoLocation, int PhotoSize)
        {
            int ChildID = RetrieveChildID();

            if (ChildID == 0)
            {
                return 0;
            }

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

            SqlCommand cmd = new SqlCommand("NoteCreation", cnn);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ChildID", data.ChildID);
            cmd.Parameters.AddWithValue("@DateOccurred", data.DateOccurred);
            cmd.Parameters.AddWithValue("@Text", data.Text);
            cmd.Parameters.AddWithValue("@HasPhoto", data.HasPhoto);
            cmd.Parameters.Add(new SqlParameter("@PhotoLocation", SQLDataAccess.ToDBNull(data.PhotoLocation)));
            cmd.Parameters.AddWithValue("@PhotoSize", data.PhotoSize);
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