using Shinkei.IRC.Entities;

namespace Shinkei.IRC.Messages
{
    public class Message
    {
        protected Server _ServerInstance;
        public Server ServerInstance
        {
            get { return _ServerInstance; }
        }

        protected IEntity _Sender;
        public IEntity Sender
        {
            get { return _Sender; }
        }

        protected IEntity _Recipient;
        public IEntity Recipient
        {
            get { return _Recipient; }
        }

        public Message(Server _Server, IEntity _Sender, IEntity _Recipient)
        {
            this._ServerInstance = _Server;
            this._Sender = _Sender;
            this._Recipient = _Recipient;
        }
    }
}
