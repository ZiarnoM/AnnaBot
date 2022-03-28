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
            };
            return View(m); 
        }
    }
}