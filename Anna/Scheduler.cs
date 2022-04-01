using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Threading;
using System.Xml;

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
            // init list which will contain all rss items in arrays
            List<string[]> xmlColection = new List<string[]> { };
            // get list of every rss which is sub
            DataRowCollection rssSubscriptionRows = Db.ExecSqlCollection(sql, args);
            foreach (DataRow rssRow in rssSubscriptionRows)
            {
                // create http request
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
                        // split xml to nodes by item tag
                        nodeList = root.SelectNodes("descendant::item");
                        // foreach item in xml push content to list as array
                        foreach (XmlNode item in nodeList)
                        {
                            xmlColection.Add(new string[]
                            {
                                //id,subscriber,title,description,link,pubdate
                                //if not present in xml push null
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
                    //sleep for 5 min
                    Thread.Sleep(300000);
                    //get list of every scheduler where next start date is smalles than now
                    DataRowCollection schedulerTasksToRun = Db.ExecSqlCollection(
                        "select SchedulerId from ReportsScheduler where ActiveP = 1 and NextStart < GETDATE()",
                        new object[] { });
                    foreach (DataRow schedulerTaskRow in schedulerTasksToRun)
                    {
                        //foreach item in list get sql and send method
                        DataRowCollection schedulerTasksSql = Db.ExecSqlCollection(
                            "select SQl,SendMethod from Scheduler where Id = @p0",
                            new object[] {schedulerTaskRow.ItemArray[0]});
                        foreach (DataRow schedulerRow in schedulerTasksSql)
                        {
                            //pass args to disctionary present on top
                            string action = schedulerRow.ItemArray[1].ToString();
                            string sqlCmd = schedulerRow.ItemArray[0].ToString();
                            Commands[action](sqlCmd, new object[] { });
                        }

                    }
                    //update next start date
                    Db.ExecNonQuerySql("update rs set NextSTart = dbo.GetNextStart(rs.DateStart, rs.IntervalType, rs.Interval) from ReportsScheduler rs where rs.NextStart < GETDATE()",new string[]{});


                }

            }));

            aoe.Start();
        }

    }
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
}
