using Shinkei.IRC.Entities;

namespace Shinkei.IRC.Messages
{
    public class JoinMessage : Message
    {
        public JoinMessage(Server _Server, IEntity _Sender, IEntity _Recipient)
            : base(_Server, _Sender, _Recipient)
        {
        }
    }
}
