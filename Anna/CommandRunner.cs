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
                {"Operation", (sql, args) => { return ExecSqlOperation(sql, args); }},
                {"Deploy", (sql, args) => { return Deploy(sql, args); }}
                
                
                
            };
        public static string DetectAndRunComamandFunction(Message msg)
        {
            try
            {
                DataRow commandRow = Db.FindCommand(msg.command, msg.parameters.Length);
                if (commandRow == null)
                {
                    return "Invalid command. See !help(null)";
                }

                string action = commandRow["Action"].ToString();
                string sqlCmd = commandRow["Sql"].ToString();


                return Commands[action](sqlCmd, msg.parameters);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "Invalid command. See !help";
            }
        }

        public static string SqlResponseToString(string sql, object[] args)
        {
            var resultStr = "";
            DataRowCollection result = Db.ExecSqlCollection(sql, args);
            if (result.Count == 1)
            {
                foreach (var cell in result[0].ItemArray)
                {
                    resultStr += cell + " ";
                }

                return resultStr;
            }
            foreach (DataRow row in result)
            {
                foreach (var cell in row.ItemArray)
                {
                    resultStr += cell + " ";
                }
                resultStr += ", ";
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
            string sql = "IF EXISTS (SELECT * FROM LogChannel WHERE Channel=@p2) begin INSERT INTO Log (UserNick,Message,Channel) VALUES(@p0,@p1,@p2) end";
            Db.ExecNonQuerySql(sql,args);

        }
        public static string ExecSqlOperation(string sql, object[] args)
        {
            Db.ExecNonQuerySql(sql,args);
            return GetCodesValue("CHANGE-SUCCESS-MESSAGE");
        }
        public static string GetCodesValue(string name)
        {
            string[] args = {name};
                string sql = "SELECT * FROM Codes where Name = @p0";
                try
                {
                    DataRow row = Db.ExecSql(sql, args);
                    return row.ItemArray[2].ToString();
                }
                catch(Exception e)
                {
                    return "OK";
                }

        }
    }
}