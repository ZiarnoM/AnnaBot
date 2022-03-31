using AnnaLogs.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Anna;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System;

namespace AnnaLogs.Controllers
{
    public class ALogsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ChannelLogs(string asd)
        {
            _ = asd;
            string testing = asd;
            DataRowCollection AllChannels = Db.ExecSqlCollection("Select Channel from Log", new object[] { });
            foreach(DataRow row in AllChannels)
            {
                if (testing == row.ItemArray[0].ToString())
                {
                    return ViewLog(testing, "");
                }
            }
            DataRow a = Db.ExecSql("Select Top 1 Channel from Log", new object[] { });
            testing = a.ItemArray[0].ToString();
            return ViewLog(testing,"");
        }

        public IActionResult ViewLog(string channLog,string datLog)
        {
            string DateTimeLog = datLog;
            return All(channLog, DateTimeLog);
        }
        public IActionResult All(string channLog, string date)
        {
            string YesterdayHelp = "";
            string TommorowHelp = "";
            string tempVar = channLog;
            DataRow x = Db.ExecSql("Select top 1 convert(varchar(10), Date, 120) from Log where (Channel = @p0) Order By Date DESC;", new object[] { tempVar });
            string nn = @x.ItemArray[0].ToString();
            string[] n = nn.Split(' ');
            if (date == "")
            {
                date = n[0];
            }
            DataRow YesterdayDate = Db.ExecSql("Select top 1 convert(varchar(10), Date, 120) from Log where ((Channel = '#Annabot')) and (Left(convert(varchar(10), Date, 120), 10)<@p1)  Order By Date DESC", new object[] { tempVar, date });
            DataRowCollection Values = Db.ExecSqlCollection("Select Id,UserNick,Message,Date,Channel from Log where (Channel = @p0) and (Left(convert(varchar(10), Date, 120), 10) =@p1);", new object[] { tempVar, date });
            DataRowCollection Dates = Db.ExecSqlCollection("Select Distinct convert(varchar(10), Date, 120) from Log;", new object[] { });
            if (YesterdayDate is null)
            {
                YesterdayHelp = date;
            }
            else
            {
                YesterdayHelp = YesterdayDate.ItemArray[0].ToString();
            }
            DataRow TommorowDate = Db.ExecSql("Select top 1 convert(varchar(10), Date, 120) from Log where ((Channel = '#Annabot')) and (Left(convert(varchar(10), Date, 120), 10)>@p1)  Order By Date ASC", new object[] { tempVar, date });
            if (TommorowDate is null)
            {
                TommorowHelp = date;
            }
            else
            {
                TommorowHelp = TommorowDate.ItemArray[0].ToString();
            }
            ALogsViewModel m = new ALogsViewModel()
            {
                Logs = Values,
                UniqeDates = Dates,
                channelName = tempVar,
                Last = date,
                Yesterday = YesterdayHelp,
                Tommorow = TommorowHelp
                };
            return View("All", m);
        }
    }
}