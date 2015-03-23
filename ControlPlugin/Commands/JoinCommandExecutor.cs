using System.Linq;
using Shinkei.API.Commands;
using Shinkei.IRC;
using Shinkei.IRC.Entities;
using Shinkei.IRC.Messages;

namespace ControlPlugin.Commands
{
    public class JoinCommandExecutor : ICommandExecutor
    {
        public bool Execute(Command command, EntUser executor, CommandMessage data)
        {
            if (data.Arguments.Count <= 0)
            {
                return false;
            }

            string channel = data.Arguments[0];
            string key = "";
            if (data.Arguments.Count > 1)
            {
                key = data.Arguments[1];
            }

            Channel newChannel;

            if (data.Server.Channels.Keys.Contains(channel))
            {
                newChannel = data.Server.Channels[channel];
            }
            else
            {
                newChannel = new Channel(data.Server, channel, key);
            }

            bool joinSucceeded = newChannel.Join();

            if (joinSucceeded)
            {
                data.Server.Channels.Add(channel, newChannel);
            }
            else
            {
                data.SendResponse("Couldn't join channel.");
            }

            return true;
        }
    }
}