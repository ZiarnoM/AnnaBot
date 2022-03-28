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

            ALogsViewModel m = new ALogsViewModel()
            {
            };
            return View(m); 
        }
    }
}