using Shinkei.IRC.Entities;

namespace Shinkei.IRC.Messages
{
    public class KickMessage : Message
    {
        private EntChannel _Channel;
        public EntChannel Channel
        {
            get { return _Channel; }
        }

        private string _Text;
        public string Text
        {
            get { return _Text; }
        }

        public KickMessage(Server _Server, EntUser _Sender, EntUser _Recipient, EntChannel _Channel, string _Text)
            : base(_Server, _Sender, _Recipient)
        {
            this._Text = _Text;
            this._Channel = _Channel;
        }
    }
}
