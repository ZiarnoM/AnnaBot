using System.Data.SqlClient;

namespace Anna
{
    public class Db
    {
        private SqlConnection _con;
        private ConfigModel _config;

        public Db(ConfigModel configModel)
        {
            _config = configModel;
        }
        public static SqlConnection Connect(string conStr)
        {
            SqlConnection c = new SqlConnection(conStr);
            c.Open();
            return c;
        }

        public SqlConnection Connect()
        {
            SqlConnection c = new SqlConnection(_config.connectionString);
            c.Open();
            return c;
        }
    }
}