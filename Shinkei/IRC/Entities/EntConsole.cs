using System;
using Shinkei.API.Commands;

namespace Shinkei.IRC.Entities
{
    class EntConsole : EntUser
    {
        private static EntConsole _instance;
        public static EntConsole GetInstance()
        {
            return _instance ?? (_instance = new EntConsole());
        }

        private EntConsole() : base(null, null, "Console")
        {
            Permission = CommandPermission.CONSOLE;
        }

        public override void SendMessage(String s)
        {
            Console.WriteLine(s);
        }

        public override bool HasPermission(CommandPermission permission)
        {
            return true;
        }
    }
}
