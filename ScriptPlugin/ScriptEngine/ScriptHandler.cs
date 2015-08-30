using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ScriptPlugin.ScriptEngine.ScriptCommand;
using ScriptPlugin.ScriptEngine.ScriptLanguage.Impl;
using Shinkei.API;
using Shinkei.API.Entities;
using Shinkei.IRC;
using Shinkei.IRC.Entities;

namespace ScriptPlugin.ScriptEngine
{
    public class ScriptHandler
    {
        public readonly Dictionary<String, ScriptContext> Contexts = new Dictionary<string, ScriptContext>();
        private static ScriptHandler _instance;
        private readonly List<string> _enabledUsers = new List<string>();
        private readonly Dictionary<String, ScriptLanguage.ScriptLanguage> _scriptLanguages = new Dictionary<string, ScriptLanguage.ScriptLanguage>();

        public static ScriptHandler GetInstance()
        {
            return _instance ?? (_instance = new ScriptHandler());
        }

        public void Register(ScriptLanguage.ScriptLanguage scriptLanguage)
        {
            _scriptLanguages.Add(scriptLanguage.FileExtension, scriptLanguage);
            scriptLanguage.PreInit();
        }

        public void SetEnabled(EntUser user, bool enabled)
        {
            if (enabled)
            {
                _enabledUsers.Add(user.Username);
            }
            else
            {
                _enabledUsers.Remove(user.Username);
            }
        }

        public bool IsEnabled(EntUser user)
        {
            return _enabledUsers.Contains(user.Username);
        }

        public void HandleLine(EntUser user, ServerEntity messageCallback, string line, ScriptPlugin plugin)
        {
            bool async = line.Contains(" --async");
            ScriptContext context = GetScriptContext(user, plugin);

            ScriptLanguage.ScriptLanguage language = context.Language;
            context.MessageCallBack = messageCallback;
            if (language == null)
            {
                language = new CSharpScriptLanguage(plugin);
                context.Language = language;
            }
            Action action = delegate()
            {
                String nl = Environment.NewLine;
                String code = null;
                if (line.ToCharArray().Length > 0 && line.ToCharArray()[0] == '.')
                {
                    String[] args = line.Split(' ');
                    String cmd = args[0].ReplaceFirstOccurrence(".", "");

                    String[] parsedArgs = new String[args.Length - 1];
                    for (int i = 0; i < parsedArgs.Length; i++)
                    {
                        parsedArgs[i] = args[i + 1];
                    }

                    HandleCommand(context, cmd, parsedArgs, line);
                    return;
                }
                if (language == null)
                {
                    context.MessageCallBack.SendMessage(ColorCode.RED +
                                                        "Couldn't set code: Language not set! Use .setlanguage <language>");
                    return;
                }

                String currentLine = line;

                //check if code was set before
                bool codeSet = false;
                bool isReplace = false;

                if (String.IsNullOrEmpty(context.Code))
                {
                    // code not set
                    code = currentLine;
                    codeSet = true;
                }
                else
                {
                    code = context.Code;
                }

                if (currentLine.StartsWith("<"))
                {
                    currentLine = currentLine.ReplaceFirstOccurrence("<", "");
                    nl = "";
                }

                if (line.Length > 0 && line.ToCharArray()[0] == '^')
                {
                    List<String> lines = GetCodeLines(context.Code);

                    // remove last line
                    if (lines.Count > 0) lines.Remove(lines[lines.Count - 1]);

                    code = lines.Aggregate("", (current, s) => current + (s + Environment.NewLine));

                    if (line.Trim().Length > 1)
                    {
                        currentLine = currentLine.Trim().ReplaceFirstOccurrence("^", "");
                        code += currentLine;
                        isReplace = true;
                        codeSet = true;
                    }
                    else
                    {
                        context.MessageCallBack.SendMessage(ColorCode.OLIVE + "Removed last line");
                        context.Code = code;
                        return;
                    }
                }

                bool isImport = false;

                if (language != null && language.GetImportIdentifiers() != null && !codeSet)
                {
                    foreach (String s in language.GetImportIdentifiers())
                    {
                        if (currentLine.StartsWith(s))
                        {
                            code = currentLine + nl + code + nl;
                            isImport = true;
                            break;
                        }
                    }
                }

                if (!isImport && !codeSet)
                {
                    code = code + nl + currentLine + nl;
                }
                context.Code = code;

                String prefix = isReplace ? ColorCode.PURPLE + "[Replace]" : ColorCode.DARK_GREEN + "[Input]";
                messageCallback.SendMessage(prefix + " " + ColorCode.NORMAL + language.FormatCode(currentLine));
            };

            if (async)
            {
                new Task(action).Start();
            }
            else
            {
                action.Invoke();
            }
        }

        private static void HandleCommand(ScriptContext context, string cmd, string[] parsedArgs, string line)
        {
            ScriptCommandBase command = ScriptCommandBase.GetScriptCommand(cmd);

            if (command == null)
            {
                context.MessageCallBack.SendMessage(ColorCode.RED + "Unknown command: " + cmd);
                return;
            }

            command.OnPreExecute(context, parsedArgs, line);
        }

        public static List<string> GetCodeLines(string s)
        {
            List<String> lines = new List<string>();
            using (StringReader reader = new StringReader(s))
            {
                string line = string.Empty;
                do
                {
                    line = reader.ReadLine();
                    if (line != null)
                    {
                        lines.Add(line);
                    }

                } while (line != null);
            }
            return lines;
        }

        public ScriptContext GetScriptContext(EntUser user, Plugin plugin)
        {
            ScriptContext context;
            if (Contexts.ContainsKey(user.Username) && Contexts[user.Username] != null)
            {
                context = Contexts[user.Username];
            }
            else
            {
                Console.WriteLine("Initializing ShellInstance for " + user.Username);
                context = new ScriptContext {User = user, Plugin = plugin};
                if (Contexts.ContainsKey(user.Username)) Contexts.Remove(user.Username);
                Contexts.Add(user.Username, context);
            }
            return context;
        }

        public void SetDefaultVariables(ScriptContext context)
        {
            ScriptLanguage.ScriptLanguage language = context.Language;
            EntUser user = context.User;
            ServerEntity callback = context.MessageCallBack;
            Plugin plugin = context.Plugin;

            language.SetVariable(context, "me", user);
            language.SetVariable(context, "plugin", plugin);
            language.SetVariable(context, "server", user.Server);
            language.SetVariable(context, "language", language);
            language.SetVariable(context, "callback", callback);
        }

        public List<ScriptLanguage.ScriptLanguage> GetScriptLanguages()
        {
            return _scriptLanguages.Keys.Select(key => _scriptLanguages[key]).ToList();
        }

        public ScriptLanguage.ScriptLanguage GetScriptLanguageByExtension(String ext)
        {
            return _scriptLanguages[ext.ToLower()];
        }

        public ScriptLanguage.ScriptLanguage GetScriptLanguageByName(String name)
        {
            return GetScriptLanguages().FirstOrDefault(language => language.Name.ToLower().Equals(name));
        }
    }
}