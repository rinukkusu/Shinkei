using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shinkei.IRC.Entities;

namespace Shinkei.IRC.Messages
{
    public class Message
    {
        public Server ServerInstance;
        public IEntity Sender;
        public IEntity Recipient;
    }
}
