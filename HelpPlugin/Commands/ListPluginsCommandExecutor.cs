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
            data.ServerInstance.PrivateMessage(data.Sender, "Loaded plugins:");
            foreach (Lazy<Plugin, IPluginData> plugin in PluginContainer.GetInstance().Plugins)
            {
                string pluginInfo = String.Format("  {0} v{1} ({2}) - {3}",
                    plugin.Metadata.Name,
                    plugin.Metadata.Version,
                    plugin.Metadata.Author,
                    plugin.Metadata.Description);
                data.ServerInstance.PrivateMessage(data.Sender, pluginInfo);
            }

            return true;
        }
    }
}