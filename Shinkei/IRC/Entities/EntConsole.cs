using Shinkei.API.Commands;

namespace Shinkei.IRC.Entities
{
    class EntConsole : EntUser
    {
        private static EntConsole _instance;
        public static EntConsole GetInstance()
        {
            return _instance ?? (_instance = new EntConsole("Console"));
        }

        public EntConsole(string name) : base(name)
        {
            _permission = CommandPermission.CONSOLE;
        }

        public override bool HasPermission(CommandPermission permission)
        {
            return true;
        }
    }
}
