using AnnaLogs.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Anna;
using System.Data;

namespace AnnaLogs.Models
{
    public class ALogsViewModel
    {
        public List<string>? Logs { get; set; }
        public List<string>? Users { get; set; }
        public List<string>? AllCont { get; set; }
        public DataRowCollection Message { get; set; }
    }
}