using System;
using CommandLine;
using CommandLine.Text;
using Shinkei.IRC;

namespace ScriptPlugin.ScriptEngine.ScriptCommand
{
    public class ExecuteCommand : ScriptCommandBase
    {
        private Options _options;
        public ExecuteCommand() : base("execute")
        {
        }

        public override bool OnExecute(ScriptContext context, string[] args, string label)
        {
            _options = new Options();
            if (Parser.Default.ParseArguments(args, _options))
            {
                if (_options.File != null)
                {
                    String[] tmp = _options.File.Split('.');

                    if (tmp.Length < 1 && context.Language == null) {
                        throw new Exception("Couldn't find extension/language for script file: " + _options.File);
                    }

                    if(tmp.Length > 1) {
                        String extension = tmp[tmp.Length-1].Trim();
                        ScriptLanguage.ScriptLanguage
                                extensionLanguage =
                                ScriptHandler.GetInstance().GetScriptLanguageByExtension(extension); // get language by extension

                        if (extensionLanguage == null) {
                            context.User.SendMessage(ColorCode.RED
                                                          + "Unknown file extension: " + extension);
                            return true;
                        }

                        // Use a new context when -f is defined
                        context = new ScriptContext {User = context.User, Language = extensionLanguage, Plugin = context.Plugin, Code = LoadFile(_options.File, extensionLanguage)};
                    }
                }

                ScriptHandler.GetInstance().SetDefaultVariables(context);
                bool resultSet;
                var result = context.Language.Run(context, _options.NoImports, _options.Clear, out resultSet, _options.Silent);
                if (!_options.Silent && resultSet)
                {
                    context.MessageCallBack.SendMessage(ColorCode.CYAN + "Output: " + ColorCode.DARK_GREEN + context.Language.FormatCode(result == null ? "null" : result.ToString()));
                }
                return true;
            }

            return false;
        }

        private string LoadFile(string file, ScriptLanguage.ScriptLanguage language)
        {
            throw new NotImplementedException();
        }

        public override string Usage
        {
            get { return _options.GetUsage(); }
        }

        public override bool LanguageRequired()
        {
            return true;
        }
    }

    class Options
    {
        [Option('a', "async", DefaultValue = false,
          HelpText = "Execute async.")]
        public bool Async { get; set; }

        [Option('f', "file",
          HelpText = "Execute file", DefaultValue = null)]
        public String File { get; set; }

        [Option('s', "silent",
            HelpText = "No output", DefaultValue = false)]
        public bool Silent { get; set; }

        [Option('n', "noimports",
            HelpText = "Don't auto import types", DefaultValue = false)]
        public bool NoImports { get; set; }

        [Option('c', "clear",
            HelpText = "Clear code after execution", DefaultValue = false)]
        public bool Clear { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
              current => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}