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

            if (data.ServerInstance.Channels.Keys.Contains(channel))
            {
                bool partSucceeded = data.ServerInstance.Channels[channel].Part();

                if (!partSucceeded)
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

                    data.ServerInstance.PrivateMessage(answerRcpt,
                        data.Sender.GetName() + ": Couldn't part channel.");
                }
            }

            return true;
        }
    }
}