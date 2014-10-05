using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shinkei.IRC
{
    public class Message
    {
        public Server ServerInstance;
        public IEntity Sender;
        public IEntity Recipient;
        public string Text;

        public Message(Server _Server, IEntity _Sender, IEntity _Recipient, string _Text)
        {
            ServerInstance = _Server;
            Sender = _Sender;
            Recipient = _Recipient;
            Text = _Text;
        }
    }
}
