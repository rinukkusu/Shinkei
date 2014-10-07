﻿using Shinkei.IRC.Entities;

namespace Shinkei.IRC.Messages
{
    public class PrivateMessage : Message
    {
        public string Text;

        public PrivateMessage(Server _Server, IEntity _Sender, IEntity _Recipient, string _Text)
        {
            ServerInstance = _Server;
            Sender = _Sender;
            Recipient = _Recipient;
            Text = _Text;
        }
    }
}
