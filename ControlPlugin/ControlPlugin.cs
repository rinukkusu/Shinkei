using System.ComponentModel.Composition;
using ControlPlugin.Commands;
using Shinkei.API;
using Shinkei.API.Commands;
using Shinkei.IRC.Commands;

namespace ControlPlugin
{
    [Export(typeof(Plugin))]
    [ExportMetadata("Name", "Control")]
    [ExportMetadata("Version", "0.1")]
    [ExportMetadata("Author", "rinukkusu")]
    [ExportMetadata("Description", "Offers commands to control the bot.")]
    public class ControlPlugin : Plugin
    {
        public override void OnEnable()
        {
            const CommandPermission perm = CommandPermission.OP;
            CommandHandler.GetInstance().RegisterCommand(new Command("join", "join <channel> <key>", "Lets the bot join a channel with an optional channel key.", new JoinCommandExecutor(), perm), this);
            CommandHandler.GetInstance().RegisterCommand(new Command("part", "part <channel>", "Lets the bot leave a channel.", new PartCommandExecutor(), perm), this);
            CommandHandler.GetInstance().RegisterCommand(new Command("quit", "quit [reason]", "Quits the bot.", new QuitCommand(), perm), this);
        }
    }
}
