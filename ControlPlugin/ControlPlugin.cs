using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;
using Shinkei;
using Shinkei.IRC;
using Shinkei.IRC.Messages;
using Shinkei.IRC.Entities;

[Export(typeof(Shinkei.IPlugin))]
[ExportMetadata("Name", "Control")]
[ExportMetadata("Version", "0.1")]
[ExportMetadata("Author", "rinukkusu")]
[ExportMetadata("Description", "Offers commands to control the bot.")]
public class ControlPlugin : IPlugin
{
    private List<CommandDescription> _commands;
    public List<CommandDescription> Commands
    {
        get { return _commands; }
    }

    public bool IsEnabled()
    {
        return true;
    }

    public void RegisterEvents(Eventsink eventdata)
    {
        _commands = new List<CommandDescription>();
        _commands.Add(new CommandDescription("join", "join <channel> <key>", "Lets the bot join a channel with an optional channel key."));
        _commands.Add(new CommandDescription("part", "part <channel>", "Lets the bot leave a channel."));

        eventdata.OnIrcCommand += new Eventsink.IrcCommandDelegate(this.IrcCommandHandler);
    }

    public List<CommandDescription> GetCommands()
    {
        return this.Commands;
    }

    private void IrcCommandHandler(CommandMessage data)
    {
        switch (data.Command)
        {
            case "join":
            {
                if (data.Arguments.Count > 0)
                {
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
                    else {
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
                        if (data.Recipient.GetType() == typeof(EntUser)) {
                            answerRcpt = data.Sender;
                        }
                        else {
                            answerRcpt = data.Recipient;
                        }

                        data.ServerInstance.PrivateMessage(answerRcpt, data.Sender.GetName() + ": Couldn't join channel.");
                    }
                }

                break;
            }
            case "part":
            {
                if (data.Arguments.Count > 0)
                {
                    string channel = data.Arguments[0];

                    if (data.ServerInstance.Channels.Keys.Contains(channel))
                    {
                        bool partSucceeded = data.ServerInstance.Channels[channel].Part();

                        if (!partSucceeded)
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

                            data.ServerInstance.PrivateMessage(answerRcpt, data.Sender.GetName() + ": Couldn't part channel.");
                        }
                    }
                }

                break;
            }
        }
    }
}
