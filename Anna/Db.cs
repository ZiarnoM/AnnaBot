using System;
using System.Data.SqlClient;
using System.IO;
using System.Text.Json;

namespace Anna
{
    public class Db
    {
        private static string _conStr;
        private static SqlConnection _con;
        public static void Config()
        {
            const string fileName = "../../../DbConString.txt";
            _conStr = File.ReadAllText(fileName);
            _con = Connect(_conStr);
            
        }
        public static SqlConnection Connect(string conStr)
        {
            SqlConnection c = new SqlConnection(conStr);
            c.Open();
            return c;
        }
    }
}