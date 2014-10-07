using Shinkei.IRC.Entities;
using System.Collections.Generic;

namespace Shinkei.IRC.Messages
{
    public class CommandMessage : Message
    {
        public string Command;
        public List<string> Arguments;

        public CommandMessage(Server _Server, IEntity _Sender, IEntity _Recipient, string _Command, List<string> _Arguments)
        {
            ServerInstance = _Server;
            Sender = _Sender;
            Recipient = _Recipient;
            Command = _Command;
            Arguments = _Arguments;
        }
    }
}
