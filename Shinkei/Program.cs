using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Shinkei.API;
using Shinkei.API.Events;
using Shinkei.IRC;
using Shinkei.IRC.Events;

namespace Shinkei
{
    class Program
    {
        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

        private delegate bool EventHandler(CtrlType sig);
        static EventHandler _handler;

        enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        private static bool Handler(CtrlType sig)
        {
            List<Server> servers;
            try
            {
                servers = Server.GetServers();
            }
            catch (Exception)
            {
                return false;
            }

            foreach (Server server in servers)
            {
                try
                {
                    server.Quit();
                }
                catch (Exception)
                {

                }
            }
            return false;
        }


        private static Boolean _stop;

        static void Main()
        {
            Thread @shinkeiThread = new Thread(StartShinkei);
            @shinkeiThread.Start();

            _handler += Handler;
            SetConsoleCtrlHandler(_handler, true);

            while (!_stop)
            {
                String line = Console.ReadLine();
                if (!String.IsNullOrWhiteSpace(line))
                {
                    string command = line.Split(' ')[0];
                    List<string> arguments = line.Split(' ').ToList();
                    arguments.RemoveAt(0);

                    ConsoleCommandEvent evnt = new ConsoleCommandEvent(command, arguments);
                    EventManager.GetInstance().CallEvent(evnt);
                }
            }
        }

        public static void Stop()
        {
            _stop = true;
        }

        private static void StartShinkei()
        {

            PluginContainer myPluginContainer = PluginContainer.GetInstance();
            myPluginContainer.LoadPlugins();

            foreach (Plugin plugin in myPluginContainer.Plugins)
            {
                plugin.Enable();
            }

            SettingsLoader mySettings = SettingsLoader.GetInstance();
            mySettings.Load();

            mySettings.EnforceSettings();
        }
    }
}
