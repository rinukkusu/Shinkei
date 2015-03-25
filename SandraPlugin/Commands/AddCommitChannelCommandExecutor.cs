using System;
using Shinkei.API.Commands;
using Shinkei.IRC;
using Shinkei.IRC.Entities;
using Shinkei.IRC.Messages;

namespace SandraPlugin.Commands
{
    public class AddCommitChannelCommandExecutor : ICommandExecutor
    {
        private SandraPlugin _plugin;
        public AddCommitChannelCommandExecutor(SandraPlugin plugin)
        {
            _plugin = plugin;
        }

        public bool Execute(Command command, EntUser executor, CommandMessage data)
        {
            if (data.Arguments.Count != 1)
            {
                return false; 
            }

            String server = data.Server.Identifier;
            String channel = data.Arguments[0];

            if (!channel.StartsWith("#")) channel = "#" + channel;
            if (_plugin.Settings.CommitChannels.ContainsKey(server) && _plugin.Settings.CommitChannels[server].Contains(channel))
            {
                data.SendResponse(ColorCode.RED + "Channel wurde schon hinzugefügt!");
                return true;
            } 

            bool success = _plugin.AddChannel(server, channel);


            if (success)
            {
                data.SendResponse(ColorCode.GREEN + "Channel erfolgreich hinzugefügt.");
                _plugin.SaveSettings();
                return true;
            }

            data.SendResponse(ColorCode.RED + "Ungültiger Server oder Channel");
            return true;
        }
    }
}