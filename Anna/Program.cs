using System;
using System.Threading;

namespace Anna
{
    class Program
    {
        static void Main(string[] args)
        {
            Scheduler.checkScheduler(); 
            
            var config = ConfigModel.DeserializeData("config.json");
            var db = new Db(config);
            IrcBot ircbot = new IrcBot(config);
            ircbot.Run();
        }
    }
}