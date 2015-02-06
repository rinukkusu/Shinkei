using System.Collections.Generic;
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

        private readonly IEntity _sender;
        public IEntity Sender
        {
            get { return _sender; }
        }

        private readonly IEntity _recipient;
        public virtual IEntity Recipient
        {
            get { return _recipient; }
        }

        private readonly Server _serverInstance;
        public Server ServerInstance
        {
            get { return _serverInstance; }
        }


        public CommandMessage(Server server, IEntity sender, IEntity recipient, string command, List<string> arguments)
        {
            _serverInstance = server;
            _sender = sender;
            _recipient = recipient;
            _command = command;
            _arguments = arguments;
        }
    }
}
