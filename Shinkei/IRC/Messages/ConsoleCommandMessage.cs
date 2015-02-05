using System;
using System.Collections.Generic;
using Shinkei.IRC.Entities;

namespace Shinkei.IRC.Messages
{
    public class ConsoleCommandMessage : Message
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

        public ConsoleCommandMessage(string command, List<string> arguments)
            : base(null, EntConsole.GetInstance(), null)
        {
            _command = command;
            _arguments = arguments;
        }
        
        public override IEntity Recipient
        {
            get { throw new InvalidOperationException(); }
        }
    
    }
}
