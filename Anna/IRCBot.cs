using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Linq;

namespace Anna
{
    public class Message
    {
        public string command { get; set; }

        public string[] parameters { get; set; }

        public string sender { get; set; }
        public Boolean isFlag { get; set; }
    }

    public class IrcBot
    {
        public static ConfigModel _config;

        public IrcBot(ConfigModel configModel)
        {
            _config = configModel;
        }


        public Message checkMessage(string sender, string message)
        {
            message = message[1..];
            Console.WriteLine("message: " + message);
            //convert args from message like: (!deploy arg1 "arg 2" arg3)
            //into string array ["arg1","arg 2","arg3"]
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
            }
            
            //not used
            Boolean isFlag;
            if (data.Count > 1)
            {
                isFlag = data[1].StartsWith("-");
            }
            else
            {
                isFlag = false;
            }

            //create message object
            Message messageContent = new Message
                {command = data[0], parameters = data.Skip(1).ToArray(), sender = sender, isFlag = isFlag};
            return messageContent;
        }

        public void sendMessage(StreamWriter writer, string receiver, string message)
        {
            //send message to receiver at irc server based on message
            //if message contains \n split it to several lines
            string[] messages = message.Split("\n");
            foreach (string mess in messages)
            {
                string[] args = new string[] {_config.nick, mess, receiver};
                CommandRunner.SqlInsertLog(args);
                writer.WriteLine($"PRIVMSG {receiver} :{mess}"); 
            }
               
        }

        public void Run()
        {
            using (var client = new TcpClient())
            {
                //connect
                Console.WriteLine($"Connecting to {_config.server}");
                client.Connect(_config.server, 6667);
                Console.WriteLine($"Connected: {client.Connected}");

                //log bot initiation
                object[] upTimeArgs = {_config.nick, "!BOTSTARTED!", 0, 0};
                CommandRunner.SqlInsertSystemLog(upTimeArgs);

                using (var stream = client.GetStream())
                using (var writer = new StreamWriter(stream))
                using (var reader = new StreamReader(stream))
                {
                    //login at server
                    writer.WriteLine($"USER {_config.id} {_config.id} {_config.id} :{_config.id}");
                    writer.WriteLine($"NICK {_config.nick}");
                    // identify with the server so your bot can be an op on the channel
                    writer.WriteLine($"PRIVMSG NickServ :IDENTIFY {_config.nick}");
                    writer.Flush();
                    
                    while (client.Connected)
                    {
                        //read data
                        var data = reader.ReadLine();
                        
                        if (data != null)
                        {
                            var d = data.Split(' ');
                            Console.WriteLine($"Data: {data}");
                            //ping method to prevent afk kick 
                            if (d[0] == "PING")
                            {
                                string message = data.Split(":")[1];
                                Console.WriteLine(data);
                                writer.WriteLine("PONG " + message);
                                writer.Flush();
                            }

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
                                            sendMessage(writer, channel,
                                                CommandRunner.GetCodesValue("WELCOME-MESSAGE"));
                                            writer.Flush();
                                        }

                                        break;
                                    }
                                    //if someone send private message
                                    case "PRIVMSG":
                                    {
                                        if (d.Length > 2)
                                        {
                                            if ((d[2] == _config.nick) || (_config.channels.Contains(d[2])))
                                            {
                                                var sender = data.Split('!')[0].Substring(1);
                                                string[] messageList = data.Split(':')[2..];
                                                string message = String.Join(":", messageList).Trim();
                                                Console.WriteLine($"Message: {message}");

                                                //private message case
                                                if (d[2] == _config.nick)
                                                {
                                                    //change destination to sender nick
                                                    d[2] = sender;
                                                }
                                                //channel message
                                                else
                                                {
                                                    string[] args = new string[] {sender, message, d[2]};
                                                    CommandRunner.SqlInsertLog(args);
                                                }

                                                if (message[0] == '!')
                                                {
                                                    //send message based on detected comend return 
                                                    sendMessage(writer, d[2],
                                                        CommandRunner.DetectAndRunComamandFunction(checkMessage(sender,
                                                            message)));
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
}