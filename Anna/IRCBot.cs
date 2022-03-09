using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Linq;
using System.Net.Mime;
using System.Text.Json;
using Anna;

namespace Anna
{
    public class Datamodel
    {
        public string server { get; set; }
        public string gecos { get; set; }
        public string nick { get; set; }
        public string id { get; set; }
        public string[] channels { get; set; }
    }
    

    public class IRCBot
    {
        private string server;
        private string gecos;
        private string nick;
        private string id;
        private string[] channels;
        
        public void setup()
        {
            string fileName = "../../../config_irc.json";
            string jsonString = File.ReadAllText(fileName);
            Datamodel datamodel = JsonSerializer.Deserialize<Datamodel>(jsonString)!;
            server = datamodel.server;
            gecos = datamodel.gecos;
            nick = datamodel.nick;
            id = datamodel.id;
            channels = datamodel.channels;
        }

        public void run()
        {
            setup();
            using (var client = new TcpClient())
            {
                Console.WriteLine($"Connecting to {server}");
                client.Connect(server, 6667);
                Console.WriteLine($"Connected: {client.Connected}");

                using (var stream = client.GetStream())
                using (var writer = new StreamWriter(stream))
                using (var reader = new StreamReader(stream))
                {
                    writer.WriteLine($"USER {id} * 8 {gecos}");
                    writer.WriteLine($"NICK {nick}");
                    // identify with the server so your bot can be an op on the channel
                    writer.WriteLine($"PRIVMSG NickServ :IDENTIFY {nick}");
                    writer.Flush();

                    while (client.Connected)
                    {
                        var data = reader.ReadLine();

                        if (data != null)
                        {
                            var d = data.Split(' ');
                            Console.WriteLine($"Data: {data}");


                            if (d.Length > 1)
                            {
                                switch (d[1])
                                {
                                    case "376":
                                    case "422":

                                    {
                                        foreach (string channel in channels)
                                        {
                                            writer.WriteLine($"JOIN {channel}");

                                            // communicate with everyone on the channel as soon as the bot logs in
                                            writer.WriteLine($"PRIVMSG {channel} :Hello, World!");
                                            writer.Flush();
                                        }

                                        break;
                                    }
                                    case "PRIVMSG":
                                    {
                                        if (d.Length > 2)
                                        {
                                            if (d[2] == nick)
                                            {
                                                // someone sent a private message to the bot
                                                var sender = data.Split('!')[0].Substring(1);
                                                var message = data.Split(':')[2];
                                                Console.WriteLine($"Message: {message}");
                                                // handle all your bot logic here
                                                writer.WriteLine(
                                                    $@"PRIVMSG {sender} :Hello, thank you for talking to me.");
                                                writer.Flush();
                                            }

                                            if (channels.Contains(d[2]))
                                            {
                                                // someone sent a private message to the bot
                                                var sender = data.Split('!')[0].Substring(1);
                                                var message = data.Split(':')[2];
                                                Console.WriteLine($"Message: {message}");
                                                // handle all your bot logic here
                                                writer.WriteLine(
                                                    $@"PRIVMSG {d[2]} :Hello, thank you for talking to me.");
                                                writer.Flush();
                                            }
                                        }

                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}