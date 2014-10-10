using Shinkei.IRC.Entities;
using System.Collections.Generic;

namespace Shinkei.IRC.Messages
{
    public class CommandMessage : Message
    {
        private string _Command;
        public string Command
        {
            get { return _Command; }
        }

        private List<string> _Arguments;
        public List<string> Arguments
        {
            get { return _Arguments; }
        }

        public CommandMessage(Server _Server, IEntity _Sender, IEntity _Recipient, string _Command, List<string> _Arguments) 
            : base(_Server, _Sender, _Recipient)
        {
            this._Command = _Command;
            this._Arguments = _Arguments;
        }
    }
}
