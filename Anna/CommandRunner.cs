﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
                {"Deploy", (sql, args) => { return Deploy(sql, args); }},
                {"AnnaDeploy", (sql, args) => { return AnnaDeploy(sql, args); }}

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
            var resultList = new List<string>();
            DataRowCollection result = Db.ExecSqlCollection(sql, args);
            if (result.Count == 1)
            {
                return string.Join(" ", result[0].ItemArray);
            }

            foreach (DataRow row in result)
            {
                resultList.Add(string.Join(" ", row.ItemArray));
            }
            return string.Join("\n", resultList);
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
                    {"PublishConfiguration", result.ItemArray[4].ToString()},
                };
                
                string [] commands = new string[]
                {
                    "rmdir /s /q tempPublish",
                    "mkdir tempPublish",
                    "cd tempPublish",
                    $"git clone {Data["Repository"]} .",
                    $"git checkout {Data["Branch"]}",
                    $"dotnet clean -v q -c Release {Data["Name"]}",
                    $"dotnet test {Data["Name"]}",
                    $"dotnet publish -o publish -c Release {Data["Name"]}",
                    $"mkdir {addQuotes(Data["Destination"])}",
                    $"move {Data["Destination"]}\\_app_offline.htm {Data["Destination"]}\\app_offline.htm",
                    $"robocopy publish {addQuotes(Data["Destination"])} /S",
                    $"move {Data["Destination"]}\\app_offline.htm {Data["Destination"]}\\_app_offline.htm",
                    "cd ../",
                    "timeout 1", // in case any process is using tempPublish
                    "rmdir /s /q tempPublish",
                    "timeout 3",
                    "rmdir /s /q tempPublish"
                };

                string publishFolderName = getPublishFolderName(Data["PublishConfiguration"]);
                string destinationFolderName = getDestinationFolderName(Data["Destination"]);

                execCmdCommands(commands, false);
                    
            }
            catch (Exception e)
            {
                object[] errorArgs = {IrcBot._config.nick,e.GetMergedErrors() , 1, 1};
                object[] stackArgs = {IrcBot._config.nick, e.StackTrace, 1, 1};
                SqlInsertSystemLog(errorArgs);
                SqlInsertSystemLog(stackArgs);
                return "Error";
            }

            return GetCodesValue("CHANGE-SUCCESS-MESSAGE");
        }

        public static void SqlInsertLog(object[] args)
        {
            string sql =
                "IF EXISTS (SELECT * FROM LogChannel WHERE Channel=@p2) begin INSERT INTO Log (UserNick,Message,Channel) VALUES(@p0,@p1,@p2) end";
            Db.ExecNonQuerySql(sql, args);
        }

        public static void SqlInsertSystemLog(object[] args)
        {

            string sql = "INSERT INTO SystemLog (UserNick,Message,Event,Type) VALUES(@p0,@p1,@p2,@p3)";

            Db.ExecNonQuerySql(sql,args);

        }

        public static string ExecSqlOperation(string sql, object[] args)
        {
            Db.ExecNonQuerySql(sql, args);
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
            catch (Exception e)
            {
                return "OK";
            }
        }

        public static void execCmdCommands(string[] commands, Boolean waitForExit)
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "cmd.exe";
            startInfo.RedirectStandardError = false;
            startInfo.RedirectStandardOutput = false;
            startInfo.RedirectStandardInput = true;
            process.StartInfo = startInfo;
            process.Start();
            
            using (StreamWriter sw = process.StandardInput)
            {
                if (sw.BaseStream.CanWrite)
                {
                    foreach (string command in commands)
                    {
                        sw.WriteLine(command);
                    }
                }
            }

            if (waitForExit)
                process.WaitForExit();
            //string stdout_str = process.StandardOutput.ReadToEnd();
            //string stderr_str = process.StandardError.ReadToEnd();
        }

        public static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
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

        /* Return publish folder name extracted from publish configuration (flags)
         *
         * Example:
         * pubConf: -o publishFolderName ... -> return publishFolderName
         * 
         */
        public static string getPublishFolderName(string publishFlags)
        {
            if (!publishFlags.Contains("-o ") && !publishFlags.Contains("--output "))
                return null;
            string[] splitted = publishFlags.Split(' ');
            int i = 0;
            foreach (string elem in splitted)
            {
                if (elem == "-o" || elem == "--output")
                {
                    try
                    {
                        return splitted[i + 1];
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }

                i++;
            }

            return null;
        }

        public static string AnnaDeploy(string sql, object[] args)
        {
            string[] cloneCommands = 
            {
                "mkdir tempRedeploy",
                "cd tempRedeploy",
                "git clone git@github.com:ZiarnoM/AnnaBot.git .",
                $"dotnet publish -o publish -c Release Anna",

            };
            
            string[] commands = new string[]
            {
                
                "taskkill/im Anna.exe /F",

            };
            
            execCmdCommands(cloneCommands, true);
            execCmdCommands(new string[]{"redeploy.bat"}, false);
            execCmdCommands(commands, false);
            return "Startuje deploy Anny";
        }
    }
}