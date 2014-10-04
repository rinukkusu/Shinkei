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

        public Eventsink()
        {
            OnIrcMessage = new IrcMessageDelegate(IrcMessageHandler);
        }
    }
}
