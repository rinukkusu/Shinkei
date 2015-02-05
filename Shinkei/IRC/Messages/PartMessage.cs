using Shinkei.IRC.Entities;

namespace Shinkei.IRC.Messages
{
    public class PartMessage : Message
    {
        private readonly string _text;
        public string Text
        {
            get { return _text; }
        }

        public PartMessage(Server server, EntUser sender, EntChannel recipient, string text)
            : base(server, sender, recipient)
        {
            _text = text;
        }
    }
}
