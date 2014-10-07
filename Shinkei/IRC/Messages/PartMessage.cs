using Shinkei.IRC.Entities;

namespace Shinkei.IRC.Messages
{
    public class PartMessage : Message
    {
        public string Text;

        public PartMessage(Server _Server, EntUser _Sender, EntChannel _Recipient, string _Text)
        {
            ServerInstance = _Server;
            Sender = _Sender;
            Recipient = _Recipient;
            Text = _Text;
        }
    }
}
