using System;
using Shinkei.IRC;

namespace ScriptPlugin.ScriptEngine.ScriptCommand
{
    public class AutostartCommand : ScriptCommandBase
    {
        public AutostartCommand() : base("autostart")
        {
        }

        public override bool OnExecute(ScriptContext context, string[] args, string label)
        {
            if (args.Length == 0)
            {
                ScriptPlugin.Instance.LoadAutoStart(context);
                return true;
            }

            String languageNames = "";
            foreach (var arg in args)
            {
                if (languageNames.Equals(""))
                {
                    languageNames = arg;
                }
                languageNames += "," + arg;
            }

            foreach(String languageName in languageNames.Split(',')) {
                ScriptLanguage.ScriptLanguage language = ScriptHandler.GetInstance().GetScriptLanguageByName(languageName.Trim());
                if (language == null) {
                    context.MessageCallBack.SendMessage(ColorCode.RED + "Warning: Unknown language: " + languageName);
                    continue;
                }
                language.PerformAutoStart(context);
            }

            return true;
        }

        public override string Usage
        {
            get { return "[language]"; }
        }

        public override bool LanguageRequired()
        {
            return false;
        }
    }
}