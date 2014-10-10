using System;

namespace Shinkei
{
    class Program
    {
        static void Main(string[] args)
        {
            IRC.Eventsink myEventsink = IRC.Eventsink.GetInstance();

            PluginContainer myPluginContainer = PluginContainer.GetInstance();
            myPluginContainer.LoadPlugins();

            foreach (Lazy<IPlugin, IPluginData> Plugin in myPluginContainer.Plugins)
            {
                Plugin.Value.RegisterEvents(myEventsink);
            }

            //myEventsink.OnIrcMessage(new IRC.Message());

            IRC.SettingsLoader mySettings = IRC.SettingsLoader.GetInstance();
            mySettings.Load();

            mySettings.EnforceSettings();

            Console.ReadLine();
        }
    }
}
