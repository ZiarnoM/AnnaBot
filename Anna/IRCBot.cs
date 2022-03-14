using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Linq;
using System.Text.RegularExpressions;
using Anna;

namespace Anna
{
    public class Message
    {
        public string command { get; set; }
        public string[] parameters { get; set; }
    }
}


public class IRCBot
{
    private ConfigModel _config;

    public IRCBot(ConfigModel configModel)
    {
        _config = configModel;
    }


    public Message checkMessage(string message)
    {
        message = message[1..];
        Console.WriteLine("message: " + message);
        List<string> data = new List<string>();
        bool inargp = false;
        bool inquotep = false;
        string hold = "";
        foreach (char c in message)
        {
            if (inquotep)
            {
                if (c == '\"')
                {
                    data.Add(hold);
                    hold = "";
                    inquotep = false;
                }
                else
                {
                    hold += c;   
                }
                
            }
            else if (inargp)
            {
                if (c != ' ')
                {
                    hold += c;
                }
                else
                {
                    data.Add(hold);
                    hold = "";
                    inargp = false;
                }
            }
            else
            {
                if (c != ' ')
                {
                    if (c == '\"')
                    {
                        inquotep = true;
                    }
                    else
                    {
                        inargp = true;
                        hold += c;
                    }
                }
            }
        }
        if (hold.Length > 0)
        {
            data.Add(hold);
            hold = "";
        }
        Message messageContent = new Message {command = data[0], parameters = data.Skip(1).ToArray()};
        return messageContent;
    }


    public void run()
    {
        using (var client = new TcpClient())
        {
            Console.WriteLine($"Connecting to {_config.server}");
            client.Connect(_config.server, 6667);
            Console.WriteLine($"Connected: {client.Connected}");

            using (var stream = client.GetStream())
            using (var writer = new StreamWriter(stream))
            using (var reader = new StreamReader(stream))
            {
                writer.WriteLine($"USER {_config.id} * 8 {_config.gecos}");
                writer.WriteLine($"NICK {_config.nick}");
                // identify with the server so your bot can be an op on the channel
                writer.WriteLine($"PRIVMSG NickServ :IDENTIFY {_config.nick}");
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
                                    foreach (string channel in _config.channels)
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
                                        if ((d[2] == _config.nick) || (_config.channels.Contains(d[2])))
                                        {
                                            //if someone send private message change destination to sender nick
                                            if (d[2] == _config.nick)
                                            {
                                                d[2] = data.Split('!')[0].Substring(1);
                                            }
                                            var message = data.Split(':')[2].Trim();
                                            Console.WriteLine($"Message: {message}");
                                            if (message[0] == '!')
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