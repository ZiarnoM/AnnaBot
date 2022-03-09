using System;

namespace Anna
{
    class Program
    {
        static void Main(string[] args)
        {

            IRCBot ircbot = new IRCBot();
            ircbot.run();
        }
    }
}