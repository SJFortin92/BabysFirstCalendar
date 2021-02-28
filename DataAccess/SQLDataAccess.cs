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
    public static class SQLDataAccess
    {
        public static object ToDBNull(object value)
        {
            if (null != value)
                return value;
            return DBNull.Value;
        }
        public static string GetConnectionString(string connectionName = "NewbornCalendarEntities")
        {
            return ConfigurationManager.ConnectionStrings[connectionName].ConnectionString;
        }

        public static List<T> LoadData<T> (string SQL)
        {
            using (IDbConnection cnn = new SqlConnection(GetConnectionString()))
            {
                return cnn.Query<T>(SQL).ToList();
            }

        }
        /*
        public static int SaveData<T>(string SQL, T data)
        {
            using (IDbConnection cnn = new SqlConnection(GetConnectionString()))
            {
                return cnn.Execute(SQL, data);
            }
        }
        */
    }
}