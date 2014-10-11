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
    private List<CommandDescription> _Commands;
    public List<CommandDescription> Commands
    {
        get { return _Commands; }
    }

    public bool IsEnabled()
    {
        return true;
    }

    public void RegisterEvents(Eventsink Eventdata)
    {
        _Commands = new List<CommandDescription>();
        _Commands.Add(new CommandDescription("help", "help [command]", "Retrieves the description and usage information of commands."));
        _Commands.Add(new CommandDescription("listplugins", "listplugins", "Lists all plugins and their description."));

        Eventdata.OnIrcCommand += new Eventsink.IrcCommandDelegate(this.IrcCommandHandler);
    }

    private void IrcCommandHandler(CommandMessage data)
    {
        switch (data.Command)
        {
            case "help":
            {
                string CommandInfo = "";
                List<CommandDescription> AllCommands = PluginContainer.GetInstance().GetAllCommands();

                if (data.Arguments.Count > 0)
                {
                    string ReqCommand = data.Arguments[0];
                    CommandDescription Command = AllCommands.Find(x => x.Command.Equals(ReqCommand));

                    if (Command != null)
                    {
                        IEntity AnswerRcpt;
                        if (data.Recipient.GetType() == typeof(EntUser)) {
                            AnswerRcpt = data.Sender;
                        }
                        else {
                            AnswerRcpt = data.Recipient;
                        }

                        CommandInfo = String.Format("Usage: {0}{1}",
                                                    SettingsLoader.GetInstance().m_Settings.CommandCharacter,
                                                    Command.Usage);

                        data.ServerInstance.PrivateMessage(AnswerRcpt, CommandInfo);
                    }
                    else
                    {
                        data.ServerInstance.PrivateMessage(data.Sender, "Command '" + ReqCommand + "' not found.");
                    }
                }
                else
                {
                    foreach (CommandDescription Command in AllCommands)
                    {
                        if (CommandInfo.Length > 0)
                        {
                            CommandInfo += ", ";
                        }

                        CommandInfo += Command.Command;
                    }

                    data.ServerInstance.PrivateMessage(data.Sender, "Available commands: " + CommandInfo);
                }

                break;
            }
            case "listplugins":
            {
                data.ServerInstance.PrivateMessage(data.Sender, "Loaded plugins:");
                foreach (Lazy<IPlugin, IPluginData> Plugin in PluginContainer.GetInstance().Plugins)
                {
                    string PluginInfo = String.Format("  {0} v{1} ({2}) - {3}", 
                                                      Plugin.Metadata.Name, 
                                                      Plugin.Metadata.Version, 
                                                      Plugin.Metadata.Author, 
                                                      Plugin.Metadata.Description);
                    data.ServerInstance.PrivateMessage(data.Sender, PluginInfo);
                }
                break;
            }
            default:
                break;
        }
    }

    public string GetHelp()
    {
        return "Retrieves information of other plugins and displays it to the user.";
    }

    public List<CommandDescription> GetCommands()
    {
        return this.Commands;
    }
}

