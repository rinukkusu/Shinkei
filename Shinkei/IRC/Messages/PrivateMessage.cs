using Shinkei.IRC.Entities;

namespace Shinkei.IRC.Messages
{
    public class PrivateMessage : Message
    {
        private string _Text;
        public string Text
        {
            get { return _Text; }
        }

        public PrivateMessage(Server _Server, IEntity _Sender, IEntity _Recipient, string _Text)
            : base(_Server, _Sender, _Recipient)
        {
            this._Text = _Text;
        }
    }
}
