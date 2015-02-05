using Shinkei.IRC.Entities;

namespace Shinkei.IRC.Messages
{
    public class KickMessage : Message
    {
        private readonly EntChannel _channel;
        public EntChannel Channel
        {
            get { return _channel; }
        }

        private readonly string _text;
        public string Text
        {
            get { return _text; }
        }

        public KickMessage(Server server, EntUser sender, EntUser recipient, EntChannel channel, string text)
            : base(server, sender, recipient)
        {
            _text = text;
            _channel = channel;
        }
    }
}
