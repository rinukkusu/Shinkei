using System;

namespace Shinkei.API.Commands
{
    public class Command
    {
        private readonly string _commandName;
        public string CommandName
        {
            get { return _commandName; }
        }

        private readonly CommandPermission _permission;

        public CommandPermission Permission
        {
            get { return _permission; }
        }
        private readonly string _usage;
        public string Usage
        {
            get { return _usage; }
        }

        private readonly string _description;

        public ICommandExecutor Executor;

        public string Description
        {
            get { return _description; }
        }

        public Command(string commandName, string usage, string description, ICommandExecutor executor, CommandPermission permission = CommandPermission.NONE)
        {
            if (_commandName.Contains("_"))
            {
                throw new ArgumentException("Command names may not contain spaces!");
            }
            _commandName = commandName;
            _usage = usage;
            _description = description;
            _permission = permission;
            Executor = executor;
        } 
    }
}