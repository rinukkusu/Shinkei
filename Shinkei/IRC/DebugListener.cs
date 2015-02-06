using System;
using Shinkei.API.Events;
using Shinkei.IRC.Events;

namespace Shinkei.IRC
{
    public class DebugListener : IListener
    {
        private static bool _initialized;
        public static void Init()
        {
            if (_initialized)
            {
                throw new InvalidOperationException("Already initialized");
            }
            DebugListener listener = new DebugListener();
            EventManager.GetInstance().RegisterEvents(listener,  ShinkeiPlugin.GetInstance());
            _initialized = true;
        }

        [Events.EventHandler(Priority = EventPriority.LOWEST)]
        public void OnLowest(IrcMessageEvent evnt)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("IrcMessageEvent: LOWEST");
            Console.ResetColor();
        }

        [Events.EventHandler(Priority = EventPriority.LOW)]
        public void OnLow(IrcMessageEvent evnt)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("IrcMessageEvent: LOW");
            Console.ResetColor();
        }

        [Events.EventHandler(Priority = EventPriority.NORMAL)]
        public void OnNormal(IrcMessageEvent evnt)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("IrcMessageEvent: NORMAL");
            Console.ResetColor();
        }

        [Events.EventHandler(Priority = EventPriority.HIGH)]
        public void OnHigh(IrcMessageEvent evnt)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("IrcMessageEvent: HIGH");
            Console.ResetColor();
        }

        [Events.EventHandler(Priority = EventPriority.HIGHEST)]
        public void OnHighest(IrcMessageEvent evnt)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("IrcMessageEvent: HIGHEST");
            Console.ResetColor();
        }

        [Events.EventHandler(Priority = EventPriority.MONITOR)]
        public void OnMonitor(IrcMessageEvent evnt)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("IrcMessageEvent: MONITOR");
            Console.ResetColor();
        }
    }
}