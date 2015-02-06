using Shinkei.IRC.Entities;

namespace Shinkei.IRC.Events
{
    public class IrcMessageEvent : Event
    {
        private readonly string _text;
        public string Text
        {
            get { return _text; }
        }

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

        public IrcMessageEvent(Server server, IEntity sender, IEntity recipient, string text)
        {
            _text = text;
            _serverInstance = server;
            _sender = sender;
            _recipient = recipient;
        }
    }
}