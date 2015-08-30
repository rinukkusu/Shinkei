using System.ComponentModel.Composition;
using System.IO;
using ScriptPlugin.Command;
using ScriptPlugin.ScriptEngine;
using ScriptPlugin.ScriptEngine.ScriptCommand;
using ScriptPlugin.ScriptEngine.ScriptLanguage;
using ScriptPlugin.ScriptEngine.ScriptLanguage.Impl;
using Shinkei.API;
using Shinkei.API.Commands;
using Shinkei.API.Entities;
using Shinkei.API.Events;
using Shinkei.IRC;
using Shinkei.IRC.Commands;
using Shinkei.IRC.Entities;
using Shinkei.IRC.Events;

namespace ScriptPlugin
{
    [Export(typeof(Plugin))]
    [ExportMetadata("Name", "ScriptPlugin")]
    [ExportMetadata("Version", "1.0")]
    [ExportMetadata("Author", "Trojaner")]
    [ExportMetadata("Description", "A plugin which adds scripting functions")]
    public class ScriptPlugin : Plugin, IListener
    {
        public string ScriptsFolder;
        public static ScriptPlugin Instance;
        public override void OnEnable()
        {
            ScriptCommandBase.RegisterCommand(new AutostartCommand());
            ScriptCommandBase.RegisterCommand(new ClearCommand());
            ScriptCommandBase.RegisterCommand(new ExecuteCommand());
            ScriptCommandBase.RegisterCommand(new SetLanguageCommand());

            ScriptsFolder = Path.Combine(DataDirectory, "scripts");
            if (!Directory.Exists(ScriptsFolder)) Directory.CreateDirectory(ScriptsFolder);
            EventManager.GetInstance().RegisterEvents(this, this);
            CommandHandler.GetInstance().RegisterCommand(new Shinkei.API.Commands.Command("script", "script [subcommand/line]",
                "Start scripting", new ScriptCommand(this), CommandPermission.OP), this);

            //Init languages.
            ScriptHandler.GetInstance().Register(new CSharpScriptLanguage(this));

            foreach(ScriptLanguage language in ScriptHandler.GetInstance().GetScriptLanguages()) 
            {
                language.Init();
            }
            Instance = this;
        }

        public void LoadAutoStart()
        {
            LoadAutoStart(GetConsoleContext());
        }

        public void LoadAutoStart(ScriptContext context) {
            ScriptContext tmpContext = new ScriptContext(context);
            foreach(ScriptLanguage language in ScriptHandler.GetInstance().GetScriptLanguages())
            {
                tmpContext.Language = language;
                language.PerformAutoStart(tmpContext);
            }
        }

        private ScriptContext _consoleContext;
        public ScriptContext GetConsoleContext()
        {
            if (_consoleContext == null)
            {
                _consoleContext = new ScriptContext
                {
                    Plugin = this,
                    User = EntConsole.GetInstance(),
                    MessageCallBack = EntConsole.GetInstance()
                };
            }

            return _consoleContext;
        }

        [EventHandler(Priority = EventPriority.HIGHEST)]
        public void OnChat(IrcMessageEvent @event)
        {
            if (!(@event.Sender is EntUser) || !(@event.Recipient is EntChannel)) return;

            if (!ScriptHandler.GetInstance().IsEnabled((EntUser)@event.Sender))
            {
                return;
            }

            ScriptHandler.GetInstance().HandleLine((EntUser)@event.Sender, (ServerEntity)@event.Recipient, @event.Text, this);
        }
    }
}
