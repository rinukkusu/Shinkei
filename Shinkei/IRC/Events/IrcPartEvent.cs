using System;
using Shinkei.IRC.Entities;

namespace Shinkei.IRC.Events
{
    public class IrcPartEvent : Event
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

        private readonly string _text;
        public string Text
        {
            get { return _text; }
        }

        public IrcPartEvent(Server server, IEntity sender, IEntity recipient, String text)
        {
            _serverInstance = server;
            _sender = sender;
            _recipient = recipient;
            _text = text;
        }
    }
}