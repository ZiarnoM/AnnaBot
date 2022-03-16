﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
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
                Dictionary<string, string> Data = new Dictionary<string, string>()
                {
                    {"Name", result.ItemArray[0].ToString()},
                    {"Repository", result.ItemArray[1].ToString()},
                    {"Branch", result.ItemArray[2].ToString()},
                    {"Destination", result.ItemArray[3].ToString()},
                };
                string destinationFolderName = getDestinationFolderName(Data["Destination"]);

                string[] cmdCommands = new string[]
                {
                    ("mkdir " + addQuotes(Data["Destination"])), // if destination doesn't exist
                    ("cd " + addQuotes(Data["Destination"] + "../") ),
                };

                execCmdCommands(cmdCommands);

            } catch(Exception e)
            {
                Console.Write(e);
                return "Error";
            }

            return "OK";
        }
        public static void SqlInsertLog(object[] args)
        {
            string sql = "IF EXISTS (SELECT * FROM LogChannel WHERE Channel=@p2) begin INSERT INTO Log (UserNick,Message,Channel) VALUES(@p0,@p1,@p2) end";
            Db.ExecNonQuerySql(sql,args);

        }
        public static string SqlChangeLogChannel(string sql, object[] args)
        {
            Db.ExecNonQuerySql(sql,args);
            return "大丈夫(daijoubu)";
        }

        public static void execCmdCommands(string[] commands)
        {
            Process p = new Process();
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = "cmd.exe";
            info.RedirectStandardInput = true;
            info.UseShellExecute = false;

            p.StartInfo = info;
            p.Start();

            using (StreamWriter sw = p.StandardInput)
            {
                if (sw.BaseStream.CanWrite)
                {
                    foreach (var cmd in commands)
                    {
                        sw.WriteLine(cmd);
                    }
                }
            }
            
        }
        
        public static string Reverse( string s )
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse( charArray );
            return new string( charArray );
        }
        
        /*
         * Return folder name from whole path
         *
         * Example:
         * C:\Users\Public\test -> Return: test
         *
         * If there is no backslash (\), returns path
         * 
         */
        public static string getDestinationFolderName(string destination)
        {
            if (destination.IndexOf("/") < 0)
            {
                return destination;
            }
            
            int indexFromEnd = Reverse(destination).IndexOf("/");
            return destination.Substring(destination.Length - indexFromEnd);
        }

       /* Return string with qoutes
        * Example: test -> "test"
        *
        * Useful while using strings containing path since:
        * cd C:/Users/Username With Space/ won't work, but
        * cd "C:/Users/Username With Space/" will
        * 
        */
        public static string addQuotes(string str)
        {
            return "\"" + str + "\"";
        }
    }
}