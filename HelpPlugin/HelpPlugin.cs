using System.ComponentModel.Composition;
using HelpPlugin.Commands;
using Shinkei.API;
using Shinkei.API.Commands;
using Shinkei.IRC.Commands;

namespace HelpPlugin
{
    [Export(typeof(Plugin))]
    [ExportMetadata("Name", "Help")]
    [ExportMetadata("Version", "0.1")]
    [ExportMetadata("Author", "rinukkusu")]
    [ExportMetadata("Description", "Retrieves information of other plugins and displays it to the user.")]

    public class HelpPlugin : Plugin
    {
        public override void OnEnable()
        {
            CommandHandler.GetInstance().RegisterCommand(new Command("help", "help [command]",
                "Retrieves the description and usage information of commands.", new HelpCommandExecutor()), this);
            CommandHandler.GetInstance().RegisterCommand(new Command("listplugins", "listplugins",
                "Lists all plugins and their description.", new ListPluginsCommandExecutor()), this);
            CommandHandler.GetInstance()
                .RegisterCommand(
                    new Command("listservers", "listservers", "Lists all servers", new ListServersCommandExecutor()),
                    this);
        }
    }
}
