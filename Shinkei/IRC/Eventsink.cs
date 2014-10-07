using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shinkei.IRC.Messages;

namespace Shinkei.IRC
{
    public class Eventsink
    {
        #region Delegates

        public delegate void IrcMessageDelegate(PrivateMessage data);
        public delegate void IrcCommandDelegate(CommandMessage data);
        public delegate void IrcJoinDelegate(JoinMessage data);
        public delegate void IrcKickDelegate(KickMessage data);
        public delegate void IrcPartDelegate(PartMessage data);

        #endregion

        #region Members

        public IrcMessageDelegate OnIrcMessage;
        public IrcCommandDelegate OnIrcCommand;
        public IrcJoinDelegate OnIrcJoin;
        public IrcKickDelegate OnIrcKick;
        public IrcPartDelegate OnIrcPart;

        #endregion

        #region Main Eventhandlers

        private void IrcMessageHandler(PrivateMessage data)
        {
            Console.WriteLine("Eventsink.IrcMessageHandler");
        }

        private void IrcCommandHandler(CommandMessage data)
        {
            Console.WriteLine("Eventsink.IrcCommandHandler");
        }

        private void IrcJoinHandler(JoinMessage data)
        {
            Console.WriteLine("Eventsink.IrcJoinHandler");
        }

        private void IrcKickHandler(KickMessage data)
        {
            Console.WriteLine("Eventsink.IrcKickHandler");
        }

        private void IrcPartHandler(PartMessage data)
        {
            Console.WriteLine("Eventsink.IrcPartHandler");
        }

        #endregion

        private static Eventsink Instance = new Eventsink();
        public static Eventsink GetInstance()
        {
            return Instance;
        }

        private Eventsink()
        {
            OnIrcMessage = new IrcMessageDelegate(IrcMessageHandler);
            OnIrcCommand = new IrcCommandDelegate(IrcCommandHandler);
            OnIrcJoin = new IrcJoinDelegate(IrcJoinHandler);
            OnIrcKick = new IrcKickDelegate(IrcKickHandler);
            OnIrcPart = new IrcPartDelegate(IrcPartHandler);
        }
    }
}
