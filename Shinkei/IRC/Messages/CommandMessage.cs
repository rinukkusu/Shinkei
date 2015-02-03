using Shinkei.IRC.Entities;
using System.Collections.Generic;

namespace Shinkei.IRC.Messages
{
    public class CommandMessage : Message
    {
        private string _command;
        public string Command
        {
            get { return _command; }
        }

        private List<string> _arguments;
        public List<string> Arguments
        {
            get { return _arguments; }
        }

        public CommandMessage(Server server, IEntity sender, IEntity recipient, string command, List<string> arguments) 
            : base(server, sender, recipient)
        {
            this._command = command;
            this._arguments = arguments;
        }
    }
}
