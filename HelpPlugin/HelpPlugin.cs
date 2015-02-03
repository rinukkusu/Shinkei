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
[ExportMetadata("Name", "Help")]
[ExportMetadata("Version", "0.1")]
[ExportMetadata("Author", "rinukkusu")]
[ExportMetadata("Description", "Retrieves information of other plugins and displays it to the user.")]
public class HelpPlugin : IPlugin
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
        _commands.Add(new CommandDescription("help", "help [command]", "Retrieves the description and usage information of commands."));
        _commands.Add(new CommandDescription("listplugins", "listplugins", "Lists all plugins and their description."));

        eventdata.OnIrcCommand += new Eventsink.IrcCommandDelegate(this.IrcCommandHandler);
    }

    private void IrcCommandHandler(CommandMessage data)
    {
        switch (data.Command)
        {
            case "help":
            {
                string commandInfo = "";
                List<CommandDescription> allCommands = PluginContainer.GetInstance().GetAllCommands();

                if (data.Arguments.Count > 0)
                {
                    string reqCommand = data.Arguments[0];
                    CommandDescription command = allCommands.Find(x => x.Command.Equals(reqCommand));

                    if (command != null)
                    {
                        IEntity answerRcpt;
                        if (data.Recipient.GetType() == typeof(EntUser)) {
                            answerRcpt = data.Sender;
                        }
                        else {
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
                    foreach (CommandDescription command in allCommands)
                    {
                        if (commandInfo.Length > 0)
                        {
                            commandInfo += ", ";
                        }

                        commandInfo += command.Command;
                    }

                    data.ServerInstance.PrivateMessage(data.Sender, "Available commands: " + commandInfo);
                }

                break;
            }
            case "listplugins":
            {
                data.ServerInstance.PrivateMessage(data.Sender, "Loaded plugins:");
                foreach (Lazy<IPlugin, IPluginData> plugin in PluginContainer.GetInstance().Plugins)
                {
                    string pluginInfo = String.Format("  {0} v{1} ({2}) - {3}", 
                                                      plugin.Metadata.Name, 
                                                      plugin.Metadata.Version, 
                                                      plugin.Metadata.Author, 
                                                      plugin.Metadata.Description);
                    data.ServerInstance.PrivateMessage(data.Sender, pluginInfo);
                }
                break;
            }
            default:
                break;
        }
    }

    public List<CommandDescription> GetCommands()
    {
        return this.Commands;
    }
}

