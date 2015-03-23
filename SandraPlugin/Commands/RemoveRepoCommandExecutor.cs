using System;
using Shinkei.API.Commands;
using Shinkei.IRC;
using Shinkei.IRC.Entities;
using Shinkei.IRC.Messages;

namespace SandraPlugin.Commands
{
    public class RemoveRepoCommandExecutor: ICommandExecutor
    {
        private SandraPlugin _plugin;
        public RemoveRepoCommandExecutor(SandraPlugin plugin)
        {
            _plugin = plugin;
        }

        public bool Execute(Command command, EntUser executor, CommandMessage data)
        {
            if (data.Arguments.Count != 1)
            {
                return false; 
            }

            String repo = data.Arguments[0];
            bool success = _plugin.DeleteRepo(repo);

            if (success)
            {
                data.SendResponse(ColorCode.GREEN + "Repository erfolgreich entfernt.");
                _plugin.SaveSettings();
                return true;
            }

            data.SendResponse(ColorCode.RED + "Unbekannte Reporsitory: " + repo);
            return true;
        }
    }
}