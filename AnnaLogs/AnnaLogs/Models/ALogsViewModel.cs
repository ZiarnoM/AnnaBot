using AnnaLogs.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Anna;
using System.Data;
using System.ComponentModel.DataAnnotations;

namespace AnnaLogs.Models
{
    public class ALogsViewModel
    {
        public DataRowCollection Logs { get; set; }
        public string? Last { get; set; }
        public string? Yesterday { get; set; }
        public string? Tommorow { get; set; }
        public DataRowCollection UniqeDates { get; set; }
        public List<string>? AllCont { get; set; }
        public string? channelName { get; set; }
    }
}