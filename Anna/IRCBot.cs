using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Runtime.InteropServices;
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
        public string[] commands { get; set; }
    }

    public class Message
    {
        public string command { get; set; }
        public string[] parameters { get; set; }
    }
}


public class IRCBot
{
    private string server;
    private string gecos;
    private string nick;
    private string id;
    private string[] channels;
    private string[] commands;
    private DateTime time_last_conn;

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
        commands = datamodel.commands;
    }

    public Message checkMessage(string message)
    {
        //TODO: data valid
        message = message[1..];
        string[] data = message.Split(' ');
        Message messageContent = new Message {command = data[0], parameters = data.Skip(1).ToArray()};

        return messageContent;
    }
    


    public void run()
    {
        setup();
        time_last_conn = DateTime.Now;
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
                                            if (d[3][0] == ':')
                                            {
                                                checkMessage(message);
                                            }

                                            writer.Flush();
                                        }

                                        if (channels.Contains(d[2]))
                                        {
                                            // someone sent a private message to the bot
                                            var sender = data.Split('!')[0].Substring(1);
                                            var message = data.Split(':')[2];
                                            Console.WriteLine($"Message: {message}");
                                            ;
                                            if (d[3][0] == ':')
                                            {
                                                checkMessage(message);
                                            }
                                            

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
