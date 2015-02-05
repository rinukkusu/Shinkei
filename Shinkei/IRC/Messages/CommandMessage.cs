using System.Collections.Generic;
using Shinkei.IRC.Entities;

namespace Shinkei.IRC.Messages
{
    public class CommandMessage : Message
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

        public CommandMessage(Server server, IEntity sender, IEntity recipient, string command, List<string> arguments) 
            : base(server, sender, recipient)
        {
            _command = command;
            _arguments = arguments;
        }
    }
}
