using Shinkei.IRC.Entities;
using Shinkei.IRC.Messages;
using Shinkei.IRC.Exceptions;
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
        public const int MaxMessageLength = 512;

        Regex _messageParser = new Regex("^(?:[:](\\S+) )?(\\S+)(?: (?!:)(.+?))?(?: [:](.+))?$");

        public SettingsLoader.Settings.ServerSettings LocalSettings; // just for reference
        public Dictionary<string, Channel> Channels;
        string _host;
        int _port;

        string _nickname;
        string _username;
        string _realname;
        string _nickservPassword;

        Thread _reader;
        TcpClient _socket;
        bool _bRunning;

        public Server(string host, int port, string nickname, string username = "shinkei", string realname = "Shinkei Bot")
        {
            _host = host;
            _port = port;
            _nickname = nickname;
            _username = username;
            _realname = realname;

            Channels = new Dictionary<string, Channel>();
        }

        private void JoinThread()
        {
            Thread.Sleep(5000);

            foreach (KeyValuePair<string, Channel> c in Channels)
            {
                c.Value.Join();
            }
        }

        private void ReadThread()
        {
            if (_socket.Connected) 
            {
                Authenticate();

                Thread @join = new Thread(JoinThread);
                @join.Start();

                StreamReader reader = new StreamReader(_socket.GetStream());
                while (_bRunning)
                {
                    string line = reader.ReadLine();
                    if (line != null)
                    {
                        Console.WriteLine(_host + ": << " + line);

                        Match parts = _messageParser.Match(line);

                        int responseCode = 0;
                        try { responseCode = Convert.ToInt32(parts.Groups[2].Value); }
                        catch { }

                        EntUser sender = new EntUser(parts.Groups[1].Value);

                        if (responseCode > 0)
                        {
                            ResponseMessage rawMessage = new ResponseMessage(this, responseCode, parts.Groups[3].Value);
                            IRC.Eventsink.GetInstance().OnIrcServerResponse(rawMessage);
                        }
                        else if (parts.Groups[2].Value == "JOIN")
                        {
                            EntChannel recipient = new EntChannel(parts.Groups[4].Value);

                            // Dispatch JOIN event
                            IRC.Eventsink.GetInstance().OnIrcJoin(new JoinMessage(this, sender, recipient));
                        }
                        else if (parts.Groups[2].Value == "KICK")
                        {
                            string channelRecipient = parts.Groups[3].Value;
                            EntChannel channel = new EntChannel(channelRecipient.Split(' ')[0]);
                            EntUser recipient = new EntUser(channelRecipient.Split(' ')[1]);

                            // Dispatch KICK event
                            IRC.Eventsink.GetInstance().OnIrcKick(new KickMessage(this, sender, recipient, channel, parts.Groups[4].Value));
                        }
                        else if (parts.Groups[2].Value == "PART")
                        {
                            EntChannel channel = new EntChannel(parts.Groups[3].Value);

                            // Dispatch PART event
                            IRC.Eventsink.GetInstance().OnIrcPart(new PartMessage(this, sender, channel, parts.Groups[4].Value));
                        }
                        else if (parts.Groups[2].Value == "PRIVMSG")
                        {
                            IEntity recipient;
                            if (parts.Groups[3].Value.StartsWith("#"))
                            {
                                recipient = new EntChannel(parts.Groups[3].Value);
                            }
                            else
                            {
                                recipient = new EntUser(parts.Groups[3].Value);
                            }

                            // Dispatch PRIVMSG event
                            IRC.Eventsink.GetInstance().OnIrcMessage(new PrivateMessage(this, sender, recipient, parts.Groups[4].Value));

                            if (IsCommandCharacter(parts.Groups[4].Value[0]))
                            {
                                // Dispatch command event
                                string commandString = parts.Groups[4].Value.Split(' ')[0];
                                string command = commandString.Substring(1);

                                string argumentString = parts.Groups[4].Value.Substring(commandString.Length);
                                List<string> arguments = ParseArguments(argumentString);

                                IRC.Eventsink.GetInstance().OnIrcCommand(new CommandMessage(this, sender, recipient, command, arguments));
                            }

                        }
                        else if (parts.Groups[0].Value.StartsWith("PING"))
                        {
                            WriteLine(parts.Groups[0].Value.Replace("PING", "PONG"));
                        }
                    }
                }
            }
        }

        public bool IsCommandCharacter(char c)
        {
            return (c == SettingsLoader.GetInstance().MSettings.CommandCharacter);
        }

        public List<string> ParseArguments(string argumentString)
        {
            List<string> arguments = new List<string>();
            argumentString = argumentString.Trim();

            string singleArgument = "";
            bool bInQuotes = false;
            foreach (char c in argumentString)
            {
                if (c == '\'')
                {
                    bInQuotes = !bInQuotes;
                }

                if (c == ' ')
                {
                    if (bInQuotes)
                    {
                        singleArgument += c.ToString();
                    }
                    else
                    {
                        arguments.Add(singleArgument);
                        singleArgument = "";
                    }
                }
                else
                {
                    singleArgument += c.ToString();
                }
            }

            if (argumentString.Length > 0)
            {
                arguments.Add(argumentString);
            }

            return arguments;
        }

        public void WriteLine(string text)
        {
            if (_socket.Connected && _bRunning)
            {
                Console.WriteLine(_host + ": >> " + text);

                StreamWriter writer = new StreamWriter(_socket.GetStream());
                writer.WriteLine(text);
                writer.Flush();
            }
        }

        public void PrivateMessage(IEntity recipient, string text)
        {
            string messageHeader = "PRIVMSG " + recipient.GetName() + " :";

            if ((messageHeader.Length + text.Length) <= MaxMessageLength)
            {
                WriteLine(messageHeader + text);
            }
            else
            {
                int nOffset = 0;
                while (text.Length > 0)
                {
                    string part = text.Substring(nOffset, (MaxMessageLength - messageHeader.Length));

                    WriteLine(messageHeader + part);

                    nOffset += (MaxMessageLength - messageHeader.Length);
                }
            }
        }

        private void Authenticate()
        {
            WriteLine("NICK " + _nickname);
            WriteLine("USER " + _username + " 0 * :" + _realname);
        }

        public void Connect()
        {
            _socket = new TcpClient();

            try
            {
                _socket.Connect(_host, _port);
                if (_socket.Connected)
                {
                    _bRunning = true;
                    _reader = new Thread(ReadThread);
                    _reader.Start();
                }
            }
            catch (Exception ex)
            {
                try
                {
                    Console.WriteLine(ex.Message);
                    _socket.Close();
                }
                catch { }
                Disconnect();
            }
        }

        public void Disconnect()
        {
            _bRunning = false;
            _reader.Abort();
        }

        public void Reconnect()
        {
            Disconnect();
            Reconnect();
        }
    }
}
