using System;

namespace Anna
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = ConfigModel.DeserializeData("config.json");
            var db = new Db(config);
            Message msg = new Message();
            msg.command = "deploy";
            msg.parameters = new string[]
            {
                "anna",
                "C:/Users/KDR Praktyki/repo/AnnaBot",
                "main",
                "C:/Users/KDR Praktyki/repo/AnnaBot5",
                "-o publish3"
            };
            CommandRunner.DetectAndRunComamandFunction(msg);
        }
    }
}