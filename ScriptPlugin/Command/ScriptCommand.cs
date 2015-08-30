using System;
using System.Linq;
using ScriptPlugin.ScriptEngine;
using Shinkei.API.Commands;
using Shinkei.IRC;
using Shinkei.IRC.Entities;
using Shinkei.IRC.Messages;

namespace ScriptPlugin.Command
{
    public class ScriptCommand : ICommandExecutor
    {
        private ScriptPlugin plugin;
        public ScriptCommand(ScriptPlugin plugin)
        {
            this.plugin = plugin;
        }

        public bool Execute(Shinkei.API.Commands.Command command, EntUser executor, CommandMessage data)
        {
            if (!(data.Recipient is EntChannel)) return true;

            if (data.Arguments.Count > 0)
            {
                String currentLine = data.Arguments.Aggregate("", (current, arg) => current + (arg + ' '));
                ScriptHandler.GetInstance().HandleLine(executor, data.Recipient, currentLine, plugin);
                return true;
            }

            if (ScriptHandler.GetInstance().IsEnabled(executor))
            {
                ScriptHandler.GetInstance().SetEnabled(executor, false);
                data.SendResponse(ColorCode.RED + "Disabled Interactive Scripting Console");
                return true;
            }

            ScriptHandler.GetInstance().SetEnabled(executor, true);
            data.SendResponse(ColorCode.DARK_GREEN + "Enabled Interactive Scripting Console");
            return true;
        }
    }
}