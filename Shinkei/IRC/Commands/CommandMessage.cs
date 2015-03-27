using System;
using System.Collections.Generic;
using Shinkei.API.Entities;
using Shinkei.IRC.Entities;

namespace Shinkei.IRC.Messages
{
    public class CommandMessage
    {
        private readonly string _command;
        public string Command
        {
            get { return _command; }
        }

        private readonly List<string> _arguments;
        public List<string> Arguments
        {
            get { return _arguments; }
        }

        private readonly ServerEntity _sender;
        public ServerEntity Sender
        {
            get { return _sender; }
        }

        private readonly ServerEntity _recipient;
        public virtual ServerEntity Recipient
        {
            get { return _recipient; }
        }

        private readonly Server _server;
        public Server Server
        {
            get { return _server; }
        }

        public void SendResponse(String s)
        {
            ServerEntity answerRcpt;
            if (Recipient.GetType() != typeof (EntUser))
            {
                answerRcpt = Recipient;
                s = Sender.GetName() + ": " + s;
            }
            else
            {
                answerRcpt = Sender;
            }
            answerRcpt.SendMessage(s);
        }

        public void SendResponseNotice(String s)
        {
            if (Sender.GetType() != typeof (EntUser))
            {
                SendResponse(s);
                return;
            }

            Server.Notice(Sender, s);
        }

        public CommandMessage(Server server, ServerEntity sender, ServerEntity recipient, string command, List<string> arguments)
        {
            _server = server;
            _sender = sender;
            _recipient = recipient;
            _command = command;
            _arguments = arguments;
        }
    }
}
