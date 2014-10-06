using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shinkei.IRC
{
    public class Eventsink
    {
        #region Delegates

        public delegate void IrcMessageDelegate(PrivateMessage data);
        public delegate void IrcCommandDelegate(CommandMessage data);

        #endregion

        #region Members

        public IrcMessageDelegate OnIrcMessage;
        public IrcCommandDelegate OnIrcCommand;

        #endregion

        #region Main Eventhandlers

        private void IrcMessageHandler(PrivateMessage data)
        {
            Console.WriteLine("IrcMessageHandler");
        }

        private void IrcCommandHandler(CommandMessage data)
        {
            Console.WriteLine("IrcCommandHandler");
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
        }
    }
}
