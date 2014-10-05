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

        public delegate void IrcMessageDelegate(Message data);

        #endregion

        #region Members

        public IrcMessageDelegate OnIrcMessage;

        #endregion

        #region Main Eventhandlers

        private void IrcMessageHandler (Message data) {
            Console.WriteLine("Mainhandler");
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
        }
    }
}
