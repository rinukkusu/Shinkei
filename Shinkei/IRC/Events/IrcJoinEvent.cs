using Shinkei.IRC.Entities;

namespace Shinkei.IRC.Events
{
    public class IrcJoinEvent : Event
    {
        private readonly Server _serverInstance;
        public Server ServerInstance
        {
            get { return _serverInstance; }
        }

        private readonly IEntity _sender;
        public IEntity Sender
        {
            get { return _sender; }
        }

        private readonly IEntity _recipient;
        public virtual IEntity Recipient
        {
            get { return _recipient; }
        }

        public IrcJoinEvent(Server server, IEntity sender, IEntity recipient)
        {
            _serverInstance = server;
            _sender = sender;
            _recipient = recipient;
        }
    }
}