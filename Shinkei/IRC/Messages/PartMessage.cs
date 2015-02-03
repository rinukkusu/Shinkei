using Shinkei.IRC.Entities;

namespace Shinkei.IRC.Messages
{
    public class PartMessage : Message
    {
        private string _text;
        public string Text
        {
            get { return _text; }
        }

        public PartMessage(Server server, EntUser sender, EntChannel recipient, string text)
            : base(server, sender, recipient)
        {
            this._text = text;
        }
    }
}
