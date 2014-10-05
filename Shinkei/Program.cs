using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shinkei
{
    class Program
    {
        static void Main(string[] args)
        {
            IRC.Eventsink myEventsink = new IRC.Eventsink();

            PluginContainer myPluginContainer = new PluginContainer();
            myPluginContainer.LoadPlugins();

            foreach (Lazy<IPlugin, IPluginData> Plugin in myPluginContainer.Plugins)
            {
                Plugin.Value.RegisterEvents(myEventsink);
            }

            myEventsink.OnIrcMessage(new IRC.Message());

            IRC.SettingsLoader mySettings = new IRC.SettingsLoader();
            mySettings.Load();

            IRC.Server myServer = new IRC.Server("lolnein.de", 6667, "test");
            myServer.Connect();

            Console.ReadLine();
        }
    }
}
