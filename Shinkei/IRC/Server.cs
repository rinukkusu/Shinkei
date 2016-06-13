using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using Shinkei.API.Entities;
using Shinkei.API.Events;
using Shinkei.IRC.Commands;
using Shinkei.IRC.Entities;
using Shinkei.IRC.Events;
using Shinkei.IRC.Messages;

namespace Shinkei.IRC
{
    public class Server
    {
        public const int MaxMessageLength = 512;

        readonly Regex _messageParser = new Regex("^(?:[:](\\S+) )?(\\S+)(?: (?!:)(.+?))?(?: [:](.+))?$");

        public SettingsLoader.Settings.ServerSettings LocalSettings; // just for reference
        public Dictionary<string, Channel> Channels;
        
        public readonly string Host;
        public readonly int Port;

        public readonly string Nickname;
        public readonly string Username;
        public readonly string Realname;
        public readonly string NickservPassword;
        public string Identifier;

        Thread _reader;
        TcpClient _socket;
        bool _bRunning;

        public static Server GetServer(string identifier)
        {
            try
            {
                return GetServers().FirstOrDefault(server => server.Identifier.Equals(identifier, StringComparison.InvariantCultureIgnoreCase));
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Server(string host, int port, string serverIdentifier, string nickname, string username = "shinkei", string realname = "Shinkei Bot", string nickservPassword = "")
        {
            Host = host;
            Port = port;
            Nickname = nickname;
            Username = username;
            Realname = realname;
            Identifier = serverIdentifier;
            NickservPassword = nickservPassword;
            Channels = new Dictionary<string, Channel>();
        }

        private void JoinThread()
        {
            Thread.Sleep(5000);

            // Login
            EntUser user = new EntUser(this, null, "NickServ");
            user.SendMessage("login " + NickservPassword);

            foreach (KeyValuePair<string, Channel> c in Channels)
            {
                c.Value.Join();
            }
        }

        private void ReadThread()
        {
            if (!_socket.Connected) return;
            try
            {
                Authenticate();

                Thread @join = new Thread(JoinThread);
                @join.Start();

                StreamReader reader = new StreamReader(_socket.GetStream());
                while (_bRunning)
                {
                    string line = reader.ReadLine();
                    if (line == null) continue;
                    Console.WriteLine(Identifier + ": << " + line);

                    Match parts = _messageParser.Match(line);

                    int responseCode = 0;
                    try
                    {
                        responseCode = Convert.ToInt32(parts.Groups[2].Value);
                    }
                    catch
                    {
                    }

                    if (responseCode > 0)
                    {
                        EventManager.GetInstance().CallEvent(new IrcServerResponseEvent(this, responseCode, line));
                    }
                    else if (parts.Groups[2].Value == "JOIN")
                    {
                        EntChannel recipient = new EntChannel(this, parts.Groups[4].Value);
                        EntUser sender = new EntUser(this, recipient, parts.Groups[1].Value);
                        EventManager.GetInstance().CallEvent(new IrcJoinEvent(this, sender, recipient));
                    }
                    else if (parts.Groups[2].Value == "KICK")
                    {
                        string channelRecipient = parts.Groups[3].Value;
                        EntChannel channel = new EntChannel(this, channelRecipient.Split(' ')[0]);
                        EntUser sender = new EntUser(this, channel, parts.Groups[1].Value);
                        EntUser recipient = new EntUser(this, channel, channelRecipient.Split(' ')[1]);
                        EventManager.GetInstance()
                            .CallEvent(new IrcKickEvent(this, sender, recipient, channel, parts.Groups[4].Value));
                    }
                    else if (parts.Groups[2].Value == "PART")
                    {
                        EntChannel channel = new EntChannel(this, parts.Groups[3].Value);
                        EntUser sender = new EntUser(this, channel, parts.Groups[1].Value);
                        EventManager.GetInstance()
                            .CallEvent(new IrcPartEvent(this, sender, channel, parts.Groups[4].Value));
                    }
                    else if (parts.Groups[2].Value == "PRIVMSG")
                    {
                        ServerEntity recipient;
                        EntChannel channel = null;
                        if (parts.Groups[3].Value.StartsWith("#"))
                        {
                            recipient = new EntChannel(this, parts.Groups[3].Value);
                            channel = (EntChannel) recipient;
                        }
                        else
                        {
                            recipient = new EntUser(this, null, parts.Groups[3].Value);
                        }

                        EntUser sender = new EntUser(this, channel, parts.Groups[1].Value);

                        if (IsCommandCharacter(parts.Groups[4].Value[0]))
                        {
                            // Dispatch command
                            string commandString = parts.Groups[4].Value.Split(' ')[0];
                            string command = commandString.Substring(1);

                            string argumentString = parts.Groups[4].Value.Substring(commandString.Length);
                            List<string> arguments = Util.ParseArguments(argumentString);

                            CommandHandler.GetInstance()
                                .HandleCommand(new CommandMessage(this, sender, recipient, command, arguments));
                        }
                        else
                        {
                            EventManager.GetInstance()
                                .CallEvent(new IrcMessageEvent(this, sender, recipient, parts.Groups[4].Value));
                        }

                    }
                    else if (parts.Groups[0].Value.StartsWith("PING"))
                    {
                        WriteLine(parts.Groups[0].Value.Replace("PING", "PONG"));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public bool IsCommandCharacter(char c)
        {
            return (c == SettingsLoader.GetInstance().MSettings.CommandCharacter);
        }
        
        public void WriteLine(string text)
        {
            if (_socket.Connected && _bRunning)
            {
                Console.Write(Identifier + ": ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(">>");
                Console.ResetColor();
                Console.Write(" " + text + "\r\n");

                StreamWriter writer = new StreamWriter(_socket.GetStream());
                writer.WriteLine(text);
                writer.Flush();
            }
        }

        private void SendMessage(String messageHeader, String text)
        {

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

        public void PrivateMessage(ServerEntity recipient, string text)
        {
            if (recipient is EntConsole)
            {
                recipient.SendMessage(text);
                return;
            }
            string messageHeader = "PRIVMSG " + recipient.GetName() + " :";
            SendMessage(messageHeader, text);
        }

        private void Authenticate()
        {
            WriteLine("NICK " + Nickname);
            WriteLine("USER " + Username + " 0 * :" + Realname);
        }

        public void Connect()
        {
            _socket = new TcpClient();

            try
            {
                _socket.Connect(Host, Port);
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


        internal void Disconnect()
        {
            _bRunning = false;
            _reader.Abort();
            _socket.Close();
        }

        internal void Reconnect()
        {
            Disconnect();
            Connect();
        }

        public void Notice(ServerEntity recipient, string text)
        {
            if (recipient is EntConsole)
            {
                recipient.SendMessage(text);
                return;
            }
            string messageHeader = "NOTICE " + recipient.GetName() + " :";
            SendMessage(messageHeader, text);
        }

        public static List<Server> GetServers()
        {
            return SettingsLoader.GetInstance().Servers;
        }

        public void Quit()
        {
            Quit(null);
        }

        public void Quit(String quitmessage)
        {
            if (quitmessage != null && quitmessage.Trim() != "")
            {
                quitmessage = " :" + quitmessage.Trim();
            }
            else
            {
                quitmessage = "";
            }
            WriteLine("QUIT" + quitmessage);
            Disconnect();
        }
    }
}
