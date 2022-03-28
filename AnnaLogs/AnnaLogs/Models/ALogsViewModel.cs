using AnnaLogs.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Anna;
using System.Data;

namespace AnnaLogs.Models
{
    public class ALogsViewModel
    {
        public DataRowCollection Logs { get; set; }
        public DataRowCollection UniqeDates { get; set; }
        public List<string>? AllCont { get; set; }
    }
}