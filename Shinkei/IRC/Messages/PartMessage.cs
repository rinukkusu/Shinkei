using Shinkei.IRC.Entities;

namespace Shinkei.IRC.Messages
{
    public class PartMessage : Message
    {
        private string _Text;
        public string Text
        {
            get { return _Text; }
        }

        public PartMessage(Server _Server, EntUser _Sender, EntChannel _Recipient, string _Text)
            : base(_Server, _Sender, _Recipient)
        {
            this._Text = _Text;
        }
    }
}
