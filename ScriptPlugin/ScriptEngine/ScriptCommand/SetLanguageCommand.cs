using System.Linq;
using Shinkei.IRC;

namespace ScriptPlugin.ScriptEngine.ScriptCommand
{
    public class SetLanguageCommand : ScriptCommandBase
    {
        public SetLanguageCommand() : base("setlanguage")
        {
        }

        public override bool OnExecute(ScriptContext context, string[] args, string label)
        {
            ScriptLanguage.ScriptLanguage newLanguage = ScriptHandler.GetInstance().GetScriptLanguages().FirstOrDefault(
                lang => lang.Name.ToLower().Equals(args[0].ToLower()) || lang.FileExtension.ToLower().Equals(args[0].ToLower()));
            if (newLanguage == null)
            {
                context.User.SendMessage(ColorCode.RED + "Unknown language: " + args[0]);
                return true;
            }

            context.Language = newLanguage;
            context.MessageCallBack.SendMessage(ColorCode.OLIVE + "Language has been set to: " + ColorCode.RED + newLanguage.Name);
            return true;
        }

        public override string Usage
        {
            get { return "<language>"; }
        }

        public override bool LanguageRequired()
        {
            return false;
        }
    }
}