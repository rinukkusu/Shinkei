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
            if (data.Arguments.Count != 2)
            {
                return false; 
            }

            var answerRcpt = data.Recipient.GetType() == typeof(EntUser) ? data.Sender : data.Recipient;

            String server = data.Arguments[0];
            String channel = data.Arguments[1];

            if (!channel.StartsWith("#")) channel = "#" + channel;
            if (_plugin.CommitChannels.ContainsKey(server) && _plugin.CommitChannels[server].Contains(channel))
            {
                data.ServerInstance.PrivateMessage(answerRcpt, executor.GetName() + ColorCode.RED + "Channel wurde schon hinzugefügt!");
                return true;
            } 

            bool success = _plugin.AddChannel(server, channel);


            if (success)
            {
                data.ServerInstance.PrivateMessage(answerRcpt, executor.GetName() + ColorCode.GREEN + "Channel erfolgreich hinzugefügt.");
                _plugin.SaveSettings();
                return true;
            }

            data.ServerInstance.PrivateMessage(answerRcpt, executor.GetName() + ColorCode.RED + "Ungültiger Server oder Channel");
            return true;
        }
    }
}