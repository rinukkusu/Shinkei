using Shinkei.IRC.Entities;
using Shinkei.IRC.Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;

namespace Shinkei.IRC
{
    public class Server
    {
        Regex MessageParser = new Regex("^(?:[:](\\S+) )?(\\S+)(?: (?!:)(.+?))?(?: [:](.+))?$");

        public SettingsLoader.Settings.ServerSettings localSettings; // just for reference
        public Dictionary<string, Channel> Channels;
        string Host;
        int Port;

        string Nickname;
        string Username;
        string Realname;
        string NickservPassword;

        Thread TReader;
        TcpClient Socket;
        bool bRunning;

        public Server(string _Host, int _Port, string _Nickname, string _Username = "shinkei", string _Realname = "Shinkei Bot")
        {
            Host = _Host;
            Port = _Port;
            Nickname = _Nickname;
            Username = _Username;
            Realname = _Realname;

            Channels = new Dictionary<string, Channel>();
        }

        private void JoinThread()
        {
            Thread.Sleep(5000);

            foreach (KeyValuePair<string, Channel> C in Channels)
            {
                C.Value.Join();
            }
        }

        private void ReadThread()
        {
            if (Socket.Connected) 
            {
                Authenticate();

                Thread TJoin = new Thread(JoinThread);
                TJoin.Start();

                StreamReader Reader = new StreamReader(Socket.GetStream());
                while (bRunning)
                {
                    string Line = Reader.ReadLine();
                    if (Line != null)
                    {
                        Console.WriteLine("<< " + Line);

                        Match Parts = MessageParser.Match(Line);

                        int ResponseCode = 0;
                        try { ResponseCode = Convert.ToInt32(Parts.Groups[2].Value); }
                        catch { }

                        EntUser Sender = new EntUser(Parts.Groups[1].Value);

                        if (ResponseCode > 0)
                        {

                        }
                        else if (Parts.Groups[2].Value == "JOIN")
                        {
                            EntChannel Recipient = new EntChannel(Parts.Groups[4].Value);

                            // Dispatch JOIN event
                            IRC.Eventsink.GetInstance().OnIrcJoin(new JoinMessage(this, Sender, Recipient));
                        }
                        else if (Parts.Groups[2].Value == "KICK")
                        {
                            string ChannelRecipient = Parts.Groups[3].Value;
                            EntChannel Channel = new EntChannel(ChannelRecipient.Split(' ')[0]);
                            EntUser Recipient = new EntUser(ChannelRecipient.Split(' ')[1]);

                            // Dispatch KICK event
                            IRC.Eventsink.GetInstance().OnIrcKick(new KickMessage(this, Sender, Recipient, Channel, Parts.Groups[4].Value));
                        }
                        else if (Parts.Groups[2].Value == "PART")
                        {
                            EntChannel Channel = new EntChannel(Parts.Groups[3].Value);

                            // Dispatch PART event
                            IRC.Eventsink.GetInstance().OnIrcPart(new PartMessage(this, Sender, Channel, Parts.Groups[4].Value));
                        }
                        else if (Parts.Groups[2].Value == "PRIVMSG")
                        {
                            IEntity Recipient;
                            if (Parts.Groups[3].Value.StartsWith("#"))
                            {
                                Recipient = new EntChannel(Parts.Groups[3].Value);
                            }
                            else
                            {
                                Recipient = new EntUser(Parts.Groups[3].Value);
                            }

                            // Dispatch PRIVMSG event
                            IRC.Eventsink.GetInstance().OnIrcMessage(new PrivateMessage(this, Sender, Recipient, Parts.Groups[4].Value));

                            if (IsCommandCharacter(Parts.Groups[4].Value[0]))
                            {
                                // Dispatch command event
                                string CommandString = Parts.Groups[4].Value.Split(' ')[0];
                                string Command = CommandString.Substring(1);

                                string ArgumentString = Parts.Groups[4].Value.Substring(CommandString.Length);
                                List<string> Arguments = ParseArguments(ArgumentString);

                                IRC.Eventsink.GetInstance().OnIrcCommand(new CommandMessage(this, Sender, Recipient, Command, Arguments));
                            }

                        }
                        else if (Parts.Groups[0].Value.StartsWith("PING"))
                        {
                            WriteLine(Parts.Groups[0].Value.Replace("PING", "PONG"));
                        }
                    }
                }
            }
        }

        public bool IsCommandCharacter(char C)
        {
            return (C == SettingsLoader.GetInstance().m_Settings.CommandCharacter);
        }

        public List<string> ParseArguments(string ArgumentString)
        {
            List<string> Arguments = new List<string>();

            string SingleArgument = "";
            bool bInQuotes = false;
            foreach (char C in ArgumentString)
            {
                if (C == '\'')
                {
                    bInQuotes = !bInQuotes;
                }

                if (C == ' ')
                {
                    if (bInQuotes)
                    {
                        SingleArgument += C.ToString();
                    }
                    else
                    {
                        Arguments.Add(SingleArgument);
                        SingleArgument = "";
                    }
                }
                else
                {
                    SingleArgument += C.ToString();
                }
            }

            return Arguments;
        }

        public void WriteLine(string text)
        {
            if (Socket.Connected && bRunning)
            {
                StreamWriter Writer = new StreamWriter(Socket.GetStream());

                Console.WriteLine(">> " + text);
                Writer.WriteLine(text);
                Writer.Flush();
            }
        }

        private void Authenticate()
        {
            WriteLine("NICK " + Nickname);
            WriteLine("USER " + Username + " 0 * :" + Realname);
        }

        public void Connect()
        {
            Socket = new TcpClient();

            try
            {
                Socket.Connect(Host, Port);
                if (Socket.Connected)
                {
                    bRunning = true;
                    TReader = new Thread(ReadThread);
                    TReader.Start();
                }
            }
            catch (Exception ex)
            {
                try
                {
                    Socket.Close();
                }
                catch { }
                Disconnect();
            }
        }

        public void Disconnect()
        {
            bRunning = false;
            TReader.Abort();
        }

        public void Reconnect()
        {
            Disconnect();
            Reconnect();
        }
    }
}
