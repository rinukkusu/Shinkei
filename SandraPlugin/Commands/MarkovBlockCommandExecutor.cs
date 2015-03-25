using System;
using Shinkei.API.Commands;
using Shinkei.IRC;
using Shinkei.IRC.Entities;
using Shinkei.IRC.Messages;

namespace SandraPlugin.Commands
{
    public class MarkovBlockCommandExecutor : ICommandExecutor
    {
        private readonly SandraPlugin _plugin;

        public MarkovBlockCommandExecutor(SandraPlugin plugin)
        {
            _plugin = plugin;
        }

        public bool Execute(Command command, EntUser executor, CommandMessage data)
        {
            if (data.Arguments.Count != 1)
            {
                return false;
            }

            String name = data.Arguments[0];

            if (!_plugin.IsMarkovEnabled(name))
            {
                data.SendResponse(ColorCode.RED + name + " wurde schon geblockt!");
                return true;
            }

            _plugin.MarkovBlock(name);
            data.SendResponse(ColorCode.GREEN + name + " wurde geblockt!");
            return true;
        }
    }
}