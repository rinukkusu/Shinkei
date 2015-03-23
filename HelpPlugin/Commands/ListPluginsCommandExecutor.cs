using System;
using Shinkei;
using Shinkei.API;
using Shinkei.API.Commands;
using Shinkei.IRC.Entities;
using Shinkei.IRC.Messages;

namespace HelpPlugin.Commands
{
    public class ListPluginsCommandExecutor : ICommandExecutor
    {
        public bool Execute(Command command, EntUser executor, CommandMessage data)
        {
            data.Sender.SendMessage("Loaded plugins:");
            foreach (Plugin plugin in PluginContainer.GetInstance().Plugins)
            {
                string pluginInfo = String.Format("  {0} v{1} ({2}) - {3}",
                    plugin.Metadata.Name,
                    plugin.Metadata.Version,
                    plugin.Metadata.Author,
                    plugin.Metadata.Description);
                data.SendResponseNotice(pluginInfo);
            }

            return true;
        }
    }
}