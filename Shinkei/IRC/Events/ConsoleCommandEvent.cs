using System;
using System.Collections.Generic;

namespace Shinkei.IRC.Events
{
    public class ConsoleCommandEvent : Event
    {
        private readonly String _command;
        public String Command
        {
            get { return _command;  }
        }

        private readonly List<String> _arguments;
        public List<String> Arguments
        {
            get { return _arguments;  }
        } 
        public ConsoleCommandEvent(String command, List<String> arguments)
        {
            _command = command;
            _arguments = arguments;
        }
    }
}