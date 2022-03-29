using AnnaLogs.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Anna;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

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
            string res = asd.Substring(0, 1);
            return All(testing);
        }
        public IActionResult All(string channLog)
        {
            string tempVar = channLog;
            DataRow x = Db.ExecSql("Select top 1 convert(varchar(10), Date, 104) from Log Order By Date DESC;", new object[] { });
            string nn = @x.ItemArray[0].ToString();
            string[] n = nn.Split(' ');
            DataRowCollection Values = Db.ExecSqlCollection("Select Id,UserNick,Message,Date,Channel from Log where Channel = @p0;", new object[] { tempVar });
            DataRowCollection Dates = Db.ExecSqlCollection("Select Distinct convert(varchar(10), Date, 104) from Log;", new object[] { });
            ALogsViewModel m = new ALogsViewModel()
            {
                Logs = Values,
                UniqeDates = Dates,
                channelName = tempVar,
                Last = n[0]
            };
            return View("All",m);
        }
    }
}