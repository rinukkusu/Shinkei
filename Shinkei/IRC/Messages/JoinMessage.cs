﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shinkei.IRC.Entities;

namespace Shinkei.IRC.Messages
{
    public class JoinMessage : Message
    {
        public JoinMessage(Server _Server, IEntity _Sender, IEntity _Recipient)
        {
            ServerInstance = _Server;
            Sender = _Sender;           // User
            Recipient = _Recipient;     // Channel
        }
    }
}