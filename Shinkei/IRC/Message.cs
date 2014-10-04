using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shinkei.IRC
{
    public class Message
    {
        public IEntity Sender;
        public IEntity Recipient;
        public string Text;
    }
}
