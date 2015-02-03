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
        public virtual IEntity Recipient
        {
            get { return _Recipient; }
        }

        public Message(Server server, IEntity sender, IEntity recipient)
        {
            this._ServerInstance = server;
            this._Sender = sender;
            this._Recipient = recipient;
        }
    }
}
