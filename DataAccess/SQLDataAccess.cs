using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dapper;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace BabysFirstCalendar.DataAccess
{
    //Class that processes SQL connections

    public static class SQLDataAccess
    {
       
        //Default connection string to SQL

        public static string GetConnectionString(string connectionName = "NewbornCalendarEntities")
        {
            return ConfigurationManager.ConnectionStrings[connectionName].ConnectionString;
        }

        
        /// <summary>
        /// Takes a SQL string such as "Select X from Y"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="SQL"></param>
        /// <returns> A list of type T, generally some sort of Model </returns>

        public static List<T> LoadData<T> (string SQL)
        {
            using (IDbConnection cnn = new SqlConnection(GetConnectionString()))
            {
                return cnn.Query<T>(SQL).ToList();
            }

        }
    }
}