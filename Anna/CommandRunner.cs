using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Anna
{
    public class CommandRunner
    {
        public static Dictionary<string, Action<string, object[]>> Commands =
            new Dictionary<string, Action<string, object[]>>()
            {
                {"Print", PrintSql}
            };


        public static void DetectAndRunComamandFunction(Message msg)
        {
            try
            {
                DataRow commandRow = Db.FindCommand(msg.command, msg.parameters.Length);
                if (commandRow == null)
                {
                    throw new Exception("AOW");
                }

                string action = commandRow["Action"].ToString();
                string sqlCmd = commandRow["Sql"].ToString();


                Commands[action](sqlCmd, msg.parameters);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static void PrintSql(string sql, object[] args)
        {
            DataRow result = Db.ExecSql(sql, args);
            foreach (var row in result.ItemArray)
            {
                Console.Write(row + " ");
            }
        }
    }
}