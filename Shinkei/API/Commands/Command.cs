namespace Shinkei.API.Commands
{
    public class Command
    {
        private string _commandName;
        public string CommandName
        {
            get { return _commandName; }
        }

        private CommandPermission _permission;

        public CommandPermission Permission
        {
            get { return _permission; }
        }
        private string _usage;
        public string Usage
        {
            get { return _usage; }
        }

        private string _description;

        public ICommandExecutor Executor;

        public string Description
        {
            get { return _description; }
        }

        public Command(string commandName, string usage, string description, ICommandExecutor executor, CommandPermission permission = CommandPermission.NONE)
        {
            _commandName = commandName;
            _usage = usage;
            _description = description;
            _permission = permission;
            Executor = executor;
        } 
    }
}