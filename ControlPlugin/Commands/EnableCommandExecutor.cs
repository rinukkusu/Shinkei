using System;
using System.Linq;
using Shinkei;
using Shinkei.API;
using Shinkei.API.Commands;
using Shinkei.IRC.Entities;
using Shinkei.IRC.Messages;

namespace ControlPlugin.Commands
{
    public class EnableCommandExecutor : ICommandExecutor
    {
        public bool Execute(Command command, EntUser executor, CommandMessage data)
        {
            if (data.Arguments.Count < 1)
            {
                return false;
            }

            String pluginName = data.Arguments[0];
            Plugin matchedPlugin = (from plugindata in PluginContainer.GetInstance().Plugins where plugindata.Metadata.Name.StartsWith(pluginName) select plugindata).FirstOrDefault();
            if (matchedPlugin == null)
            {
                data.ServerInstance.PrivateMessage(data.Sender, "Plugin not found");
                return true;
            }
            
            matchedPlugin.Enable();
            data.ServerInstance.PrivateMessage(data.Sender, matchedPlugin.Metadata.Name + ": Enabled");

            return true;
        }
    }
}