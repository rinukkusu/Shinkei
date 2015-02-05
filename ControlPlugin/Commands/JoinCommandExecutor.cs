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

            if (data.ServerInstance.Channels.Keys.Contains(channel))
            {
                newChannel = data.ServerInstance.Channels[channel];
            }
            else
            {
                newChannel = new Channel(data.ServerInstance, channel, key);
            }

            bool joinSucceeded = newChannel.Join();

            if (joinSucceeded)
            {
                data.ServerInstance.Channels.Add(channel, newChannel);
            }
            else
            {
                IEntity answerRcpt;
                if (data.Recipient.GetType() == typeof (EntUser))
                {
                    answerRcpt = data.Sender;
                }
                else
                {
                    answerRcpt = data.Recipient;
                }

                data.ServerInstance.PrivateMessage(answerRcpt, data.Sender.GetName() + ": Couldn't join channel.");
            }

            return true;
        }
    }
}