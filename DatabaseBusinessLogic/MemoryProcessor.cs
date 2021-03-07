using BabysFirstCalendar.DataAccess;
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

namespace BabysFirstCalendar.DatabaseBusinessLogic
{
    //Class for elements of the memories that we will pass to display on the calendar
    public class MemoriesToLoad
    {
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public int HasPhoto { get; set; }
    }

    //Need to add Update and Delete in here
    public static class MemoryProcessor
    {
        //Loads up memories for display
        public static List<MemoriesToLoad> ViewMemories()
        {
            //Open a new SQL connection
            string newConnectionString = SQLDataAccess.GetConnectionString();
            SqlConnection cnn = new SqlConnection(newConnectionString);

            List<MemoriesToLoad> memories = null;

            try
            {

                SqlCommand command = new SqlCommand("SELECT Text, Date, HasPhoto FROM Note;", cnn);

                using ( command )
                {
                    cnn.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            memories.Add(new MemoriesToLoad
                            {
                                Text = Convert.ToString(reader["Text"]),
                                Date = Convert.ToDateTime(reader["Date"]),
                                HasPhoto = Convert.ToInt16(reader["HasPhoto"])
                            });
                        }
                    }
                }
                return memories;
            }

            //Throw exceptions if need be
            catch (Exception)
            {
                throw;
            }

            //Close the connection
            finally
            {
                cnn.Close();
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
    }
}