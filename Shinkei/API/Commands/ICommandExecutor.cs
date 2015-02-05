using Shinkei.IRC.Entities;
using Shinkei.IRC.Messages;

namespace Shinkei.API.Commands
{
    public interface ICommandExecutor
    {
        bool Execute(Command command, EntUser executor, CommandMessage data);
    }
}