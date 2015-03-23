using System;
using Shinkei.API.Commands;
using Shinkei.IRC;
using Shinkei.IRC.Entities;
using Shinkei.IRC.Messages;

namespace HelpPlugin.Commands
{
    public class ListServersCommandExecutor : ICommandExecutor
    {
        public bool Execute(Command command, EntUser executor, CommandMessage data)
        {
            data.Sender.SendMessage("Connected servers:");
            foreach (Server server in SettingsLoader.GetInstance().Servers)
            {
                string serverInfo = String.Format("  {0} - {1}:{2}",
                    server.Identifier, server.Host, server.Port);
                data.SendResponseNotice(serverInfo);
            }
            return true;
        }
    }
}