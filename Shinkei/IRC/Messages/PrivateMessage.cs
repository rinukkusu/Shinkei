using Shinkei.IRC.Entities;

namespace Shinkei.IRC.Messages
{
    public class PrivateMessage : Message
    {
        private string _text;
        public string Text
        {
            get { return _text; }
        }

        public PrivateMessage(Server server, IEntity sender, IEntity recipient, string text)
            : base(server, sender, recipient)
        {
            this._text = text;
        }
    }
}
