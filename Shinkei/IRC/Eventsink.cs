using Shinkei.IRC.Messages;
using System;

namespace Shinkei.IRC
{
    public class Eventsink
    {
        #region Delegates

        public delegate void IrcRawMessageDelegate(RawMessage data);
        public delegate void IrcMessageDelegate(PrivateMessage data);
        public delegate void IrcCommandDelegate(CommandMessage data);
        public delegate void IrcJoinDelegate(JoinMessage data);
        public delegate void IrcKickDelegate(KickMessage data);
        public delegate void IrcPartDelegate(PartMessage data);

        public delegate void IrcQueuedCommandDelegate(CommandMessage data);
        public delegate void IrcQueuedJoinDelegate(JoinMessage data);
        public delegate void IrcQueuedKickDelegate(KickMessage data);
        public delegate void IrcQueuedPartDelegate(PartMessage data);

        #endregion

        #region Members

        public IrcRawMessageDelegate OnIrcRawMessage;
        public IrcMessageDelegate OnIrcMessage;
        public IrcCommandDelegate OnIrcCommand;
        public IrcJoinDelegate OnIrcJoin;
        public IrcKickDelegate OnIrcKick;
        public IrcPartDelegate OnIrcPart;

        public IrcQueuedCommandDelegate OnIrcQueuedCommand;
        public IrcQueuedJoinDelegate OnIrcQueuedJoin;
        public IrcQueuedKickDelegate OnIrcQueuedKick;
        public IrcQueuedPartDelegate OnIrcQueuedPart;

        #endregion

        #region Main Eventhandlers

        private void IrcRawMessageHandler(RawMessage data)
        {
            Console.WriteLine("Eventsink.IrcRawMessageHandler");
        }

        private void IrcMessageHandler(PrivateMessage data)
        {
            Console.WriteLine("Eventsink.IrcMessageHandler");
        }

        private void IrcCommandHandler(CommandMessage data)
        {
            Console.WriteLine("Eventsink.IrcCommandHandler");
            OnIrcQueuedCommand(data);
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

        #region Queued Eventhandlers (for Plugins)

        private void IrcQueuedCommandHandler(CommandMessage data)
        {
            Console.WriteLine("Eventsink.IrcQueuedCommandHandler");
        }

        private void IrcQueuedJoinHandler(JoinMessage data)
        {
            Console.WriteLine("Eventsink.IrcQueuedJoinHandler");
        }

        private void IrcQueuedKickHandler(KickMessage data)
        {
            Console.WriteLine("Eventsink.IrcQueuedKickHandler");
        }

        private void IrcQueuedPartHandler(PartMessage data)
        {
            Console.WriteLine("Eventsink.IrcQueuedPartHandler");
        }

        #endregion

        private static Eventsink Instance = new Eventsink();
        public static Eventsink GetInstance()
        {
            return Instance;
        }

        private Eventsink()
        {
            OnIrcRawMessage = new IrcRawMessageDelegate(IrcRawMessageHandler);
            OnIrcMessage = new IrcMessageDelegate(IrcMessageHandler);
            OnIrcCommand = new IrcCommandDelegate(IrcCommandHandler);
            OnIrcJoin = new IrcJoinDelegate(IrcJoinHandler);
            OnIrcKick = new IrcKickDelegate(IrcKickHandler);
            OnIrcPart = new IrcPartDelegate(IrcPartHandler);

            OnIrcQueuedCommand = new IrcQueuedCommandDelegate(IrcQueuedCommandHandler);
            OnIrcQueuedJoin = new IrcQueuedJoinDelegate(IrcQueuedJoinHandler);
            OnIrcQueuedKick = new IrcQueuedKickDelegate(IrcQueuedKickHandler);
            OnIrcQueuedPart = new IrcQueuedPartDelegate(IrcQueuedPartHandler);
        }
    }
}
