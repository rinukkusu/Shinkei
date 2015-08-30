using Shinkei.IRC;

namespace ScriptPlugin.ScriptEngine.ScriptCommand
{
    public class ClearCommand : ScriptCommandBase
    {
        public ClearCommand() : base("clear")
        {
        }

        public override bool OnExecute(ScriptContext context, string[] args, string label)
        {
            context.Code = "";
            context.MessageCallBack.SendMessage(ColorCode.RED + "History cleared");
            return true;
        }

        public override string Usage
        {
            get { return ""; }
        }

        public override bool LanguageRequired()
        {
            return true;
        }
    }
}