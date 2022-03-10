using System;

namespace Anna
{
    class Program
    {
        static void Main(string[] args)
        {
            Db.Config();
            IRCBot ircbot = new IRCBot();
            ircbot.run();
        }
    }
}