using System;

namespace Anna
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = ConfigModel.DeserializeData("../../../config.json");
            var db = new Db(config);
            IRCBot ircbot = new IRCBot();
            ircbot.run();
        }
    }
}