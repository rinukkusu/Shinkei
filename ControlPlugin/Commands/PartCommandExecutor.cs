using System.Linq;
using Shinkei.API.Commands;
using Shinkei.IRC.Entities;
using Shinkei.IRC.Messages;

namespace ControlPlugin.Commands
{
    public class PartCommandExecutor : ICommandExecutor
    {
        public bool Execute(Command command, EntUser executor, CommandMessage data)
        {
            if (data.Arguments.Count <= 0)
            {
                return false;
            }
            string channel = data.Arguments[0];

            if (data.Server.Channels.Keys.Contains(channel))
            {
                bool partSucceeded = data.Server.Channels[channel].Part();

                if (!partSucceeded)
                {
                    data.SendResponse("Couldn't part channel.");
                }
            }

            return true;
        }
    }
}