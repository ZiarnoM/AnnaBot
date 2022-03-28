using AnnaLogs.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Anna;
using System.Data;

namespace AnnaLogs.Controllers
{
    public class ALogsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult All()
        {
            DataRowCollection Values = Db.ExecSqlCollection("Select Id,UserNick,Message,Date,Channel from Log;", new object[] { });
            DataRowCollection Dates = Db.ExecSqlCollection("Select Distinct convert(varchar(10), Date, 120) from Log;", new object[] { });
            ALogsViewModel m = new ALogsViewModel()
            {
                Logs = Values,
                UniqeDates = Dates
            };
            return View(m); 
        }
    }
}