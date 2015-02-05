using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Shinkei.API;
using Shinkei.API.Commands;
using Shinkei.IRC.Entities;
using Shinkei.IRC.Messages;

namespace Shinkei.IRC.Commands
{
    public class CommandHandler
    {
        private static CommandHandler _instance;
        private readonly Dictionary<Command, Plugin> _commands = new Dictionary<Command, Plugin>();

        public void RegisterCommand(Command cmd, Plugin plugin)
        {
            if (_commands.ContainsKey(cmd))
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("WARNING: Duplicate command: " + cmd.CommandName);
                Console.ResetColor();
                return;
            }

            _commands.Add(cmd, plugin);
        }

        public List<Command> GetCommands()
        {
            return new ReadOnlyCollection<Command>(_commands.Keys.ToList()).ToList();
        } 

        public static CommandHandler GetInstance()
        {
            return _instance ?? (_instance = new CommandHandler());
        }

        public void HandleCommand(CommandMessage data)
        {
            if (data.Sender.GetType() != typeof (EntUser))
            {
                //throw new InvalidOperationException();
                return;
            }

            EntUser executor = (EntUser) data.Sender;

            Command cmd = GetCommands().FirstOrDefault(c => c.CommandName.Equals(data.Command, StringComparison.InvariantCultureIgnoreCase));

            if (cmd == null || cmd.Executor == null)
            {
                data.ServerInstance.PrivateMessage(data.Sender, "Unknown command: " + data.Command);
                return;
            }

            if (!executor.HasPermission(cmd.Permission))
            {
                data.ServerInstance.PrivateMessage(data.Sender, "You dont have enough permission to do that");
                return;
            }

            if (!cmd.Executor.Execute(cmd, executor, data))
            {
                String usage = "Usage: " + SettingsLoader.GetInstance().MSettings.CommandCharacter + cmd.Usage;
                data.ServerInstance.PrivateMessage(data.Sender, usage);
            }
        }
    }
}