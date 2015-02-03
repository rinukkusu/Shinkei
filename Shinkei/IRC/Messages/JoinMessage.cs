using Shinkei.IRC.Entities;

namespace Shinkei.IRC.Messages
{
    public class JoinMessage : Message
    {
        public JoinMessage(Server server, IEntity sender, IEntity recipient)
            : base(server, sender, recipient)
        {
        }
    }
}
