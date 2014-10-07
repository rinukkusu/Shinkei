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
