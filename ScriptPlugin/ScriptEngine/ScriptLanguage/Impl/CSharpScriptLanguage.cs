using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mono.CSharp;
using Shinkei;
using Shinkei.API.Entities;
using Shinkei.IRC;
using Shinkei.IRC.Entities;

namespace ScriptPlugin.ScriptEngine.ScriptLanguage.Impl
{
    public class CSharpScriptLanguage : ScriptLanguage
    {
        public CSharpScriptLanguage(ScriptPlugin plugin) : base(plugin, "C#", "cs")
        {
        }

        protected override object Eval(ScriptContext context, String code, out bool resultSet, bool silent)
        {
            Evaluator eval = (Evaluator) context.Bindings;
            object result = null;
            object r = null;
            resultSet = false;

            foreach (var line in ScriptHandler.GetCodeLines(code))
            {
                r = eval.Evaluate(line, out result, out resultSet);
            }
            context.Bindings = CreateBindings(context.User, context.MessageCallBack);

            if (!resultSet)
                result = r;
            if (!resultSet && result == null && !silent)
            {
                context.MessageCallBack.SendMessage(ColorCode.RED + context.User.Nickname + ": No result returned");
                return null;
            }

            return result;
        }

        public override string GetDefaultImports()
        {
            return "using System;" +
                   "using System.Collections.Generic;" +
                   "using System.Linq;" +
                   "using Shinkei;" +
                   "using Shinkei.API.Entites;" +
                   "using Shinkei.API;" +
                   "using Shinkei.IRC;" +
                   "using Shinkei.IRC.Entities;"; 
        }

        public override void SetVariable(ScriptContext context, string name, object value)
        {

            FieldInfo fieldInfo = typeof(Evaluator).GetField("fields", BindingFlags.NonPublic | BindingFlags.Instance);
            if (fieldInfo != null)
            {
                var fields = fieldInfo.GetValue(context.Bindings) as Dictionary<string, Tuple<FieldSpec, FieldInfo>>;
                if (fields != null)
                {
                    Tuple<FieldSpec, FieldInfo> tuple;
                    try
                    {
                        tuple = fields[name];
                    }
                    catch (Exception)
                    {
                        tuple = null;
                    }

                    if (tuple == null)
                    {
                        //Dirty code :(
                        //A field with that name doesnt exists, so lets create one
                        if (value != null)
                        {
                            ((Evaluator) context.Bindings).Run(value.GetType().FullName + " " + name + " = null;");
                        }
                        else
                        {
                            ((Evaluator)context.Bindings).Run("object " + name + " = null;");
                        }
                        SetVariable(context, name, value);
                        return;
                    }

                    tuple.Item2.SetValue(context.Bindings, value);
                }
            }
        }

        public override List<string> GetImportIdentifiers()
        {
            return new List<string> {"using"};
        }

        public override object CreateBindings(EntUser user, ServerEntity callback)
        {
            
            if (callback == null) callback = user;


            CompilerSettings csettings = new CompilerSettings();
            csettings.Unsafe = true;
            csettings.Optimize = false;
            CompilerContext context = new CompilerContext(csettings, new IrcReportPrinter(user, callback));
            var evaluator = new Evaluator(context);
            
            //Reference all plugins
            foreach (var plugin in PluginContainer.GetInstance().Plugins)
            {
                evaluator.ReferenceAssembly(plugin.GetType().Assembly);
            }

            evaluator.ReferenceAssembly(typeof(Enumerable).Assembly);

            return evaluator;
        }

        public override object FormatCode(string code)
        {
            return code; //Todo
        }
    }
}