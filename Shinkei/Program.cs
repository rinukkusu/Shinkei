using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shinkei.IRC;

namespace Shinkei
{
    class Program
    {
        static void Main(string[] args)
        {
            IRC.Eventsink myEventsink = IRC.Eventsink.GetInstance();

            PluginContainer myPluginContainer = new PluginContainer();
            myPluginContainer.LoadPlugins();

            foreach (Lazy<IPlugin, IPluginData> Plugin in myPluginContainer.Plugins)
            {
                Plugin.Value.RegisterEvents(myEventsink);
            }

            //myEventsink.OnIrcMessage(new IRC.Message());

            IRC.SettingsLoader mySettings = IRC.SettingsLoader.GetInstance();
            mySettings.Load();

            IRC.Server myServer = new IRC.Server("lolnein.de", 6667, "test");
            myServer.Connect();

            Console.ReadLine();
        }
    }
}
