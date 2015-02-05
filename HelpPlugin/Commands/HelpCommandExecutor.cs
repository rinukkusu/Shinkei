using System;
using System.Collections.Generic;
using Shinkei;
using Shinkei.API.Commands;
using Shinkei.IRC;
using Shinkei.IRC.Commands;
using Shinkei.IRC.Entities;
using Shinkei.IRC.Messages;

namespace HelpPlugin.Commands
{
    public class HelpCommandExecutor : ICommandExecutor
    {
        public bool Execute(Command description, EntUser executor, CommandMessage data)
        {
            string commandInfo = "";
            List<Command> allCommands = CommandHandler.GetInstance().GetCommands();

            if (data.Arguments.Count > 0)
            {
                string reqCommand = data.Arguments[0];
                Command command = allCommands.Find(x => x.CommandName.Equals(reqCommand));

                if (command != null)
                {
                    IEntity answerRcpt;
                    if (data.Recipient.GetType() == typeof(EntUser))
                    {
                        answerRcpt = data.Sender;
                    }
                    else
                    {
                        answerRcpt = data.Recipient;
                    }

                    commandInfo = String.Format("Usage: {0}{1}",
                                                SettingsLoader.GetInstance().MSettings.CommandCharacter,
                                                command.Usage);

                    data.ServerInstance.PrivateMessage(answerRcpt, commandInfo);
                }
                else
                {
                    data.ServerInstance.PrivateMessage(data.Sender, "Command '" + reqCommand + "' not found.");
                }
            }
            else
            {
                foreach (Command command in allCommands)
                {
                    if (commandInfo.Length > 0)
                    {
                        commandInfo += ", ";
                    }

                    commandInfo += command.CommandName;
                }

                data.ServerInstance.PrivateMessage(data.Sender, "Available commands: " + commandInfo);
            }
            return true;
        }
    }
}