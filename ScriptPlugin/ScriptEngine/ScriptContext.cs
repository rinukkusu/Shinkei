using System;
using Shinkei.API;
using Shinkei.API.Entities;
using Shinkei.IRC.Entities;

namespace ScriptPlugin.ScriptEngine
{
    public class ScriptContext
    {
        public ScriptContext()
        {
            
        }

        public ScriptContext(ScriptContext parent)
        {
            Plugin = parent.Plugin;
            User = parent.User;
            MessageCallBack = parent.MessageCallBack;
            Language = parent.Language;
            Code = parent.Code;
            Bindings = parent.Bindings;
        }

        public Plugin Plugin { get; set; }
        public EntUser User { get; set; }
        public ServerEntity MessageCallBack { get; set; }

        private ScriptLanguage.ScriptLanguage _language;
        public ScriptLanguage.ScriptLanguage Language
        {
            get { return _language;; }
            set
            {
                _language = value;
                Bindings = _language.CreateBindings(User, MessageCallBack);
            }
        }

        public String Code { get; set; }
        public Object Bindings { get; set; }
    }
}