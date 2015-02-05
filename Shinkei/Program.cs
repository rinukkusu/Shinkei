using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Shinkei.API;
using Shinkei.IRC;
using Shinkei.IRC.Messages;

namespace Shinkei
{
    class Program
    {
        private static Boolean _stop;

        static void Main()
        {
            Thread @shinkeiThread = new Thread(StartShinkei);
            @shinkeiThread.Start();

            while (!_stop)
            {
                String line = Console.ReadLine();
                if (!String.IsNullOrWhiteSpace(line))
                {
                    string command = line.Split(' ')[0];
                    List<string> arguments = line.Split(' ').ToList();
                    arguments.RemoveAt(0);

                    Eventsink.GetInstance().OnConsoleCommand.Invoke(new ConsoleCommandMessage(command, arguments));
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

            foreach (Lazy<Plugin, IPluginData> plugin in myPluginContainer.Plugins)
            {
                plugin.Value.OnEnable();
            }

            //myEventsink.OnIrcMessage(new IRC.Message());

            SettingsLoader mySettings = SettingsLoader.GetInstance();
            mySettings.Load();

            mySettings.EnforceSettings();
        }
    }
}
