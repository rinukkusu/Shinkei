using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;
using Shinkei;
using Shinkei.API;
using Shinkei.API.Commands;
using Shinkei.API.Events;
using Shinkei.IRC;
using Shinkei.IRC.Commands;
using Shinkei.IRC.Entities;
using Shinkei.IRC.Events;
using WeatherPlugin.Commands;

namespace WeatherPlugin
{
    [Export(typeof(Plugin))]
    [ExportMetadata("Name", "Weather")]
    [ExportMetadata("Version", "0.1")]
    [ExportMetadata("Author", "rinukkusu")]
    [ExportMetadata("Description", "A plugin to gather information about weather.")]
    public class WeatherPlugin : Plugin, IListener
    {
        public override void OnEnable()
        {
            CommandHandler.GetInstance().RegisterCommand(new Command("w", "w <place>", "Gathers wather information for the given place.", new WeatherCommandExecutor()), this);
        }
    }
}
