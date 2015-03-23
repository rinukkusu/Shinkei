using System;

namespace Shinkei.API.Commands
{
    public enum CommandPermission
    {
        NONE = 0,
        VOICE = 1,
        WHITELIST = 2,
        OP = 3,
        CONSOLE = 4
    }
}