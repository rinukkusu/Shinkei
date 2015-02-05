﻿using System;
using Shinkei.IRC.Messages;

namespace Shinkei.IRC
{
    public class Eventsink
    {
        #region Delegates

        public delegate void ConsoleCommandDelegate(ConsoleCommandMessage data);
        public delegate void IrcServerResponseDelegate(ResponseMessage data);
        public delegate void IrcMessageDelegate(PrivateMessage data);
        public delegate void IrcJoinDelegate(JoinMessage data);
        public delegate void IrcKickDelegate(KickMessage data);
        public delegate void IrcPartDelegate(PartMessage data);

        public delegate void ConsoleQueuedCommandDelegate(ConsoleCommandMessage data);
        public delegate void IrcQueuedJoinDelegate(JoinMessage data);
        public delegate void IrcQueuedKickDelegate(KickMessage data);
        public delegate void IrcQueuedPartDelegate(PartMessage data);

        #endregion

        #region Members

        public ConsoleCommandDelegate OnConsoleCommand;
        public IrcServerResponseDelegate OnIrcServerResponse;
        public IrcMessageDelegate OnIrcMessage;
        public IrcJoinDelegate OnIrcJoin;
        public IrcKickDelegate OnIrcKick;
        public IrcPartDelegate OnIrcPart;

        public ConsoleQueuedCommandDelegate OnConsoleQueuedCommand;
        public IrcQueuedJoinDelegate OnIrcQueuedJoin;
        public IrcQueuedKickDelegate OnIrcQueuedKick;
        public IrcQueuedPartDelegate OnIrcQueuedPart;

        #endregion

        #region Main Eventhandlers

        private void ConsoleCommandHandler(ConsoleCommandMessage data)
        {
            Console.WriteLine("Eventsink.ConsoleCommandHandler");
            OnConsoleQueuedCommand(data);
        }

        private void IrcServerResponseHandler(ResponseMessage data)
        {
            Console.WriteLine("Eventsink.IrcServerResponseHandler");
        }

        private void IrcMessageHandler(PrivateMessage data)
        {
            Console.WriteLine("Eventsink.IrcMessageHandler");
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

        private void ConsoleQueuedCommandHandler(ConsoleCommandMessage data)
        {
            Console.WriteLine("Eventsink.ConsoleQueuedCommandHandler");
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

        private static Eventsink _instance = new Eventsink();
        public static Eventsink GetInstance()
        {
            return _instance;
        }

        private Eventsink()
        {
            OnConsoleCommand = ConsoleCommandHandler;
            OnIrcServerResponse = IrcServerResponseHandler;
            OnIrcMessage = IrcMessageHandler;
            OnIrcJoin = IrcJoinHandler;
            OnIrcKick = IrcKickHandler;
            OnIrcPart = IrcPartHandler;
            
            OnConsoleQueuedCommand = ConsoleQueuedCommandHandler;
            OnIrcQueuedJoin = IrcQueuedJoinHandler;
            OnIrcQueuedKick = IrcQueuedKickHandler;
            OnIrcQueuedPart = IrcQueuedPartHandler;
        }
    }
}
