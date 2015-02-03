using Shinkei.IRC.Entities;

namespace Shinkei.IRC.Messages
{
    public class KickMessage : Message
    {
        private EntChannel _channel;
        public EntChannel Channel
        {
            get { return _channel; }
        }

        private string _text;
        public string Text
        {
            get { return _text; }
        }

        public KickMessage(Server server, EntUser sender, EntUser recipient, EntChannel channel, string text)
            : base(server, sender, recipient)
        {
            this._text = text;
            this._channel = channel;
        }
    }
}
