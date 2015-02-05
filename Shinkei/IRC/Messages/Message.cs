using Shinkei.IRC.Entities;

namespace Shinkei.IRC.Messages
{
    public class Message
    {
        private Server _serverInstance;
        public Server ServerInstance
        {
            get { return _serverInstance; }
        }

        private IEntity _sender;
        public IEntity Sender
        {
            get { return _sender; }
        }

        private IEntity _recipient;
        public virtual IEntity Recipient
        {
            get { return _recipient; }
        }

        public Message(Server server, IEntity sender, IEntity recipient)
        {
            _serverInstance = server;
            _sender = sender;
            _recipient = recipient;
        }
    }
}
