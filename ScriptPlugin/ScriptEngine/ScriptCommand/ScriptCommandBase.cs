using System;
using System.Collections.Generic;
using System.Linq;
using Shinkei.IRC;

namespace ScriptPlugin.ScriptEngine.ScriptCommand
{
    public abstract class ScriptCommandBase
    {
        private static readonly List<ScriptCommandBase> registerdScriptCommands = new List<ScriptCommandBase>(); 
        protected ScriptCommandBase(String name)
        {
            Name = name;
        }

        public void OnPreExecute(ScriptContext context, String[] args, String label)
        {
            if (LanguageRequired() && context.Language == null)
            {
                context.MessageCallBack.SendMessage(ColorCode.RED + "Couldn't execute command \"" + Name + "\"" + ": Language not set! Use .setlanguage <language>");
                return;
            }

            bool result = OnExecute(context, args, label);

            if (!result)
            {
                context.MessageCallBack.SendMessage("Usage: ." + Name + " " + Usage);
            }
        }

        public static ScriptCommandBase GetScriptCommand(String s)
        {
            return registerdScriptCommands.FirstOrDefault(cmd => cmd.Name.ToLower().Equals(s.ToLower()));
        }

        public static void RegisterCommand(ScriptCommandBase command)
        {
            registerdScriptCommands.Add(command);
        }

        public abstract bool OnExecute(ScriptContext context, string[] args, string label);

        public readonly String Name;
        public abstract String Usage { get; }

        public abstract bool LanguageRequired();
    }
}