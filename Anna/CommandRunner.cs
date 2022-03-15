using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using TFunc =
    System.Func<System.Collections.Generic.IDictionary<string, object>,
        System.Collections.Generic.IDictionary<string, object>>;

namespace Anna
{
    public class CommandRunner
    {
        public static Dictionary<string, Func<string, object[], string>> Commands =
            new Dictionary<string, Func<string, object[], string>>()
            {
                {"Print", (sql, args) => { return SqlResponseToString(sql, args); }},
                {"Log", (sql, args) => { return SqlChangeLogChannel(sql, args); }},
                {"Deploy", (sql, args) => { return Deploy(sql, args); }},
                
            };
        public static string DetectAndRunComamandFunction(Message msg)
        {
            try
            {
                DataRow commandRow = Db.FindCommand(msg.command, msg.parameters.Length);
                if (commandRow == null)
                {
                    return "Invalid command. See !help";
                }

                string action = commandRow["Action"].ToString();
                string sqlCmd = commandRow["Sql"].ToString();


                return Commands[action](sqlCmd, msg.parameters);
            }
            catch (Exception e)
            {
                return "Invalid command. See !help";
            }
        }

        public static string SqlResponseToString(string sql, object[] args)
        {
            var resultStr = "";
            DataRow result = Db.ExecSql(sql, args);
            foreach (var row in result.ItemArray)
            {
                resultStr += row + " ";
            }

            return resultStr;
        }

        //TODO
        public static string Deploy(string sql, object[] args)
        {
            try
            {
                DataRow result = Db.ExecSql(sql, args);
                foreach (var row in result.ItemArray)
                {
                    Console.Write(row + " ");
                }

            } catch(Exception e)
            {
                Console.Write(e);
            }

            return "OK";
        }
        public static void SqlInsertLog(object[] args)
        {
            string sql = "INSERT INTO Log (UserNick,Message,Channel) VALUES(@p0,@p1,@p2)";
            Db.ExecNonQuerySql(sql,args);

        }
        public static string SqlChangeLogChannel(string sql, object[] args)
        {
            Db.ExecNonQuerySql(sql,args);
            return "大丈夫(daijoubu)";
        }
    }
}