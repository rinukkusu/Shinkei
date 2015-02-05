using System;

namespace Shinkei.API.Commands
{
    [Flags]
    public enum CommandPermission
    {
        VOICE = 1,
        WHITELIST = 2,
        OP = 4,
        CONSOLE = 8,
        NONE = 0xFF
    }
}