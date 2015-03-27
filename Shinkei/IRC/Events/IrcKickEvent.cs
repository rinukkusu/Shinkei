using System;
using Shinkei.API.Entities;
using Shinkei.IRC.Entities;

namespace Shinkei.IRC.Events
{
    public class IrcKickEvent : Event
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

        private readonly EntChannel _channel;
        public EntChannel Channel
        {
            get { return _channel; }
        }

        public IrcKickEvent(Server server, IEntity sender, IEntity recipient, EntChannel channel, String text)
        {
            _serverInstance = server;
            _sender = sender;
            _recipient = recipient;
            _text = text;
            _channel = channel;
        }
    }
}