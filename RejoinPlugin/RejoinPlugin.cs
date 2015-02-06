using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Runtime.Serialization;
using System.Threading;
using Shinkei;
using Shinkei.API;
using Shinkei.API.Events;
using Shinkei.IRC.Events;

namespace RejoinPlugin
{
    [Export(typeof(Plugin))]
    [ExportMetadata("Name", "Rejoin")]
    [ExportMetadata("Version", "0.1")]
    [ExportMetadata("Author", "rinukkusu")]
    [ExportMetadata("Description", "Automatically performs a rejoin after being kicked from a channel.")]
    public class RejoinPlugin : Plugin, IListener
    {
        [DataContract]
        public class Settings
        {
            [DataMember]
            public int WaitUntilRejoin;
        }
    
        public static Settings MSettings = LoadSettings();

        public override void OnEnable()
        {
            EventManager.GetInstance().RegisterEvents(this, this);
        }

        [Shinkei.IRC.Events.EventHandler]
        public void IrcKickHandler(IrcKickEvent evnt)
        {
            Console.WriteLine("RejoinPlugin.IrcKickHandler");

            Thread.Sleep(MSettings.WaitUntilRejoin);
            evnt.ServerInstance.Channels[evnt.Channel.Name].Join();
        }

        public static Settings LoadSettings()
        {
            //DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Settings));

            string path = "plugins/RejoinPlugin.json";
            Settings newSettings;

            if (!File.Exists(path))
            {
                newSettings = new Settings();
                newSettings.WaitUntilRejoin = 2000;

                JsonHelper.Serialize<Settings>(newSettings, path);
            }
            else
            {
                newSettings = JsonHelper.Deserialize<Settings>(path);
            }

            return newSettings;
        }
    }
}
