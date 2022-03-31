using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Anna
{
    public class Scheduler
    {
        public static Dictionary<string, Func<string, object[], string>> Commands =
            new Dictionary<string, Func<string, object[], string>>()
            {
                {"Rss", (sql, args) => { return RssXmlReader(sql, args); }}
            };

        public static string RssXmlReader(string sql, object[] args)
        {
            List<string[]> xmlColection = new List<string[]> { };
            DataRowCollection rssSubscriptionRows = Db.ExecSqlCollection(sql, args);
            foreach (DataRow rssRow in rssSubscriptionRows)
            {
                CookieContainer cookies = new CookieContainer();
                HttpWebRequest webRequest =
                    (HttpWebRequest) WebRequest.Create(rssRow.ItemArray[1].ToString());
                webRequest.UserAgent =
                    "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
                webRequest.Method = "GET";
                webRequest.CookieContainer = cookies;
                using (HttpWebResponse webResponse = (HttpWebResponse) webRequest.GetResponse())
                {
                    using (StreamReader streamReader = new StreamReader(webResponse.GetResponseStream()))
                    {
                        XmlDocument doc = new XmlDocument();
                        string xml = streamReader.ReadToEnd();
                        doc.LoadXml(xml);
                        XmlNodeList nodeList;
                        XmlNode root = doc.DocumentElement;

                        nodeList = root.SelectNodes("descendant::item");
                        foreach (XmlNode item in nodeList)
                        {
                            xmlColection.Add(new string[]
                            {
                                rssRow.ItemArray[0].ToString(),
                                rssRow.ItemArray[2].ToString(),
                                item["title"]?.InnerText,
                                item["description"]?.InnerText,
                                item["link"]?.InnerText,
                                item["pubDate"]?.InnerText
                            });
                        }
                    }
                }
            }

            return "1";
        }

        public static void checkScheduler()
        {
            Thread aoe = new Thread(new ThreadStart(() =>
            {
                while (true)
                {
                    try
                    {
                        Thread.Sleep(3000);
                        DataRowCollection schedulerTasksToRun = Db.ExecSqlCollection(
                            "select SchedulerId from ReportsScheduler where ActiveP = 1 and NextStart < GETDATE()",
                            new object[] { });
                        foreach (DataRow schedulerTaskRow in schedulerTasksToRun)
                        {
                            DataRowCollection schedulerTasksSql = Db.ExecSqlCollection(
                                "select SQl,SendMethod from Scheduler where Id = @p0",
                                new object[] {schedulerTaskRow.ItemArray[0]});
                            foreach (DataRow schedulerRow in schedulerTasksSql)
                            {
                                string action = schedulerRow.ItemArray[1].ToString();
                                string sqlCmd = schedulerRow.ItemArray[0].ToString();
                                Commands[action](sqlCmd, new object[] { });
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        object[] errorArgs = {IrcBot._config.nick,e.GetMergedErrors() , 1, 1};
                        object[] stackArgs = {IrcBot._config.nick, e.StackTrace, 1, 1};
                        CommandRunner.SqlInsertSystemLog(errorArgs);
                        CommandRunner.SqlInsertSystemLog(stackArgs);
                    }
                }
            }));

            aoe.Start();
        }
    }
}