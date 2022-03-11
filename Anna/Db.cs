using System.Data.SqlClient;
using System;
using System.Data;
using System.Text;

namespace Anna
{
    public class Db
    {
        private static SqlConnection _con;
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

        public void Connect()
        {
            SqlConnection c = new SqlConnection(_config.connectionString);
            c.Open();
            _con = c;
        }

        public static DataSet selectSQL(SqlCommand sqlCmd)
        {
            DataSet ds;
            try
            {
                SqlDataAdapter da = new SqlDataAdapter(sqlCmd);
                ds = new DataSet();
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                Console.Write(ex);
                throw;
            }

            return ds;
        }

        public static SqlCommand commandSQL(SqlConnection con, string sql, params object[] values) // @p0, @p1 ...
        {
            SqlCommand cmd = new SqlCommand(sql, con);
            if (values != null)
                for (int i = 0; i < values.Length; i++)
                    if (values[i] == null)
                        cmd.Parameters.AddWithValue("p" + i.ToString(), DBNull.Value);
                    else
                        cmd.Parameters.AddWithValue("p" + i.ToString(), values[i]);
            return cmd;
        }

        public static DataRow rowSQL(SqlConnection con, string sql, params object[] values)
        {
            SqlCommand cmd = commandSQL(con, sql, values);
            DataSet ds = selectSQL(cmd);
            return Db.getRow(ds);
        }

        public static DataRow getRow(DataSet ds)
        {
            DataRowCollection drc = getRows(ds);
            if (drc.Count > 0)
                return drc[0];
            return null;
        }

        public static DataRowCollection getRows(DataSet ds)
        {
            return ds.Tables[0].Rows;
        }

        public static DataRow FindCommand(string name, int numArgs)
        {
            object[] args = {name, numArgs};
            return rowSQL(_con, "select * from Commands WHERE Command=@p0 AND NumberOfArguments=@p1", args);
        }

        public static DataRow FindService(string name)
        {
            object[] args = {name};
            return rowSQL(_con, "select * from Services WHERE Name=@p0", args);
        }
    }
}