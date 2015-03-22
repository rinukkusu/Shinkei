using System;
using Shinkei.API.Commands;
using Shinkei.IRC;
using Shinkei.IRC.Entities;
using Shinkei.IRC.Messages;

namespace SandraPlugin.Commands
{
    public class AddRepoCommandExecutor : ICommandExecutor
    {
        private SandraPlugin _plugin;
        public AddRepoCommandExecutor(SandraPlugin plugin)
        {
            _plugin = plugin;
        }

        public bool Execute(Command command, EntUser executor, CommandMessage data)
        {
            if (data.Arguments.Count != 1)
            {
                return false; 
            }

            var answerRcpt = data.Recipient.GetType() == typeof(EntUser) ? data.Sender : data.Recipient;

            String repo = data.Arguments[0];

            if (_plugin.Repos.Contains(repo))
            {
                data.ServerInstance.PrivateMessage(answerRcpt, executor.GetName() + ColorCode.RED + "Repository wurde schon hinzugefügt.");
                return true;
            }
            
            bool success = _plugin.AddRepo(repo);

            if (success)
            {
                data.ServerInstance.PrivateMessage(answerRcpt, executor.GetName() + ColorCode.GREEN + "Repository erfolgreich hinzugefügt.");
                _plugin.SaveSettings();
                return true;
            }

            data.ServerInstance.PrivateMessage(answerRcpt, executor.GetName() + ColorCode.RED + "Ungültige Reporsitory: " + repo);
            return true;
        }
    }
}