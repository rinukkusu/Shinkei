using System;
using Shinkei.API.Commands;
using Shinkei.IRC.Entities;
using Shinkei.IRC.Messages;

namespace ControlPlugin.Commands
{
    public class QuitCommand : ICommandExecutor
    {
        public bool Execute(Command command, EntUser executor, CommandMessage data)
        {
            String reason = "";
            foreach (String s in data.Arguments)
            {
                if (reason == "")
                {
                    reason = s;
                    continue;
                }

                reason += " " + s;
            }

            data.Server.Quit(reason);

            return true;
        }
    }
}