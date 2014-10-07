using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shinkei.IRC.Entities;

namespace Shinkei.IRC.Messages
{
    public class KickMessage : Message
    {
        public EntChannel Channel;
        public string Text;

        public KickMessage(Server _Server, EntUser _Sender, EntUser _Recipient, EntChannel _Channel, string _Text)
        {
            ServerInstance = _Server;
            Sender = _Sender;
            Recipient = _Recipient;
            Text = _Text;
            Channel = _Channel;
        }
    }
}
