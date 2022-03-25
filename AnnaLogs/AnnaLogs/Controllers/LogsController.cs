using AnnaLogs.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Anna;
using System.Data;

namespace AnnaLogs.Controllers
{
    public class LogsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult All()
        {

            LogsViewModel m = new LogsViewModel()
            {
                Logs = new List<string> { },
                Users = new List<string> { },
                AllCont = new List<string> { }
            };

            for (int i = 0; i < 101; i++)
            {
                string msg = "User" + i.ToString() + (i + 2).ToString();
                m.Logs.Add(msg);
            }
            for (int i = 0; i < 101; i++)
            {
                string msgx = "Message" + i.ToString() + (i + 2).ToString();
                m.Users.Add(msgx);
            }
            for (int i = 0; i < 101; i++)
            {
                string jd = m.Logs[i] + " " + m.Users[i];
                m.AllCont.Add(jd);
            }
            return View(m); 
        }
    }
}