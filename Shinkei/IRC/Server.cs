using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;
using System.Text.RegularExpressions;

namespace Shinkei.IRC
{
    public class Server
    {
        Regex MessageParser = new Regex("^(?:[:](\\S+) )?(\\S+)(?: (?!:)(.+?))?(?: [:](.+))?$");

        List<Channel> Channels;
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
        }

        private void ReadThread()
        {
            if (Socket.Connected) 
            {
                Authenticate();

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

                        User Sender = new User(Parts.Groups[1].Value);

                        if (ResponseCode > 0)
                        {

                        }
                        else if (Parts.Groups[2].Value == "PRIVMSG")
                        {
                            IEntity Recipient;
                            if (Parts.Groups[3].Value.StartsWith("#")) {
                                Recipient = new Channel(Parts.Groups[3].Value);
                            }
                            else {
                                Recipient = new User(Parts.Groups[3].Value);
                            }
                            IRC.Eventsink.GetInstance().OnIrcMessage(new Message(this, Sender, Recipient, Parts.Groups[4].Value));
                        }
                        else if (Parts.Groups[0].Value.StartsWith("PING"))
                        {
                            WriteLine(Parts.Groups[0].Value.Replace("PING", "PONG"));
                        }

                        foreach (Group G in Parts.Groups)
                        {
                            string s = G.Value;
                        }
                    }
                }
            }
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

        public void JoinChannel(string _Channel)
        {
            
        }
    }
}
