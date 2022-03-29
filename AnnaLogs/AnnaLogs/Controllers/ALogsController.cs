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
            return ViewLog(testing,"");
        }

        public IActionResult ViewLog(string channLog,string datLog)
        {
            string DateTimeLog = datLog;
            return All(channLog, DateTimeLog);
        }
        public IActionResult All(string channLog, string date)
        {
            string tempVar = channLog;
            DataRow x = Db.ExecSql("Select top 1 convert(varchar(10), Date, 120) from Log Order By Date DESC;", new object[] { });
            string nn = @x.ItemArray[0].ToString();
            string[] n = nn.Split(' ');
            if (date == "")
            {
                date = n[0];
            }
            DataRowCollection Values = Db.ExecSqlCollection("Select Id,UserNick,Message,Date,Channel from Log where (Channel = @p0) and (Left(convert(varchar(10), Date, 120), 10) =@p1);", new object[] { tempVar, date });
            DataRowCollection Dates = Db.ExecSqlCollection("Select Distinct convert(varchar(10), Date, 120) from Log;", new object[] { });
            ALogsViewModel m = new ALogsViewModel()
            {
                Logs = Values,
                UniqeDates = Dates,
                channelName = tempVar,
                Last = date
            };
            return View("All", m);
        }
    }
}