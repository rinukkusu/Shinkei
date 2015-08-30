using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Shinkei.API.Entities;
using Shinkei.IRC;
using Shinkei.IRC.Entities;

namespace ScriptPlugin.ScriptEngine.ScriptLanguage
{
    public abstract class ScriptLanguage
    {
        public readonly string ScriptlanguageDirectory;
        public readonly string FrameworkFolder;
        public readonly string AutostartDirectory;

        public ScriptPlugin Plugin { get; private set; }
        public string FileExtension { get; private set; }
        public string Name { get; private set; }

        protected ScriptLanguage(ScriptPlugin plugin, String name, String fileExtension)
        {
            FileExtension = fileExtension.ToLower();
            Plugin = plugin;
            Name = name;

            ScriptlanguageDirectory = Path.Combine(plugin.ScriptsFolder, name);
            FrameworkFolder = Path.Combine(ScriptlanguageDirectory, "framework");
            AutostartDirectory = Path.Combine(ScriptlanguageDirectory, "autostart");
            if (!CreateDirectory(ScriptlanguageDirectory) || !CreateDirectory(FrameworkFolder)
                || !CreateDirectory(AutostartDirectory))
            {
                throw new Exception(Name + ": Couldn't create required directories!");
            }
        }

        private bool CreateDirectory(String dir)
        {
            if (Directory.Exists(dir))
            {
                return true;
            }

            try
            {
                Directory.CreateDirectory(dir);
            }
            catch (Exception)
            {
                return false;
            }

            return Directory.Exists(dir);
        }

        public Object Run(ScriptContext context,  bool skipImports, bool clear, out bool resulset, bool silent)
        {
            Console.WriteLine(context.User + " executed script: " + context.Code);
            String code = context.Code;

            if (!skipImports)
            {
                code = OnUpdateImports(code);
            }

            Object result = Eval(context, code, out resulset, silent);

            if (clear)
            {
                context.Code = "";
            }

            return result;
        }

        protected Object Run(ScriptContext context, String file, out  bool resultSet)
        {
            Console.WriteLine(context.User.Username + " executed script file: " + file);

            SetVariable(context, "scriptfile", file);

            return Eval(context, OnUpdateImports(File.ReadAllText(file)), out resultSet, true);
        }

        protected String OnUpdateImports(String code)
        {
            String defaultImports = GetDefaultImports();
            if (defaultImports == null)
            {
                return code;
            }
            if (defaultImports != "")
            {
                code = code.Replace(defaultImports, "");
            }
            return defaultImports + code;
        }

        public Object ParseValue(String[] args, EntUser user) {
            String arg = args[0];

            try 
            {
                Int64 l = Int64.Parse(arg);
                if (l <= Byte.MaxValue) 
                {
                    return Byte.Parse(arg); // Value is a byte
                }
                if (l <= Int16.MaxValue) 
                {
                    return Int16.Parse(arg); // Value is a Short (16 bit integer)
                }
                if (l <= Int32.MaxValue) 
                {
                    return Int32.Parse(arg); // Value is an 32-bit Integer
                }
                return l; // Value is a Long
            }
            catch (FormatException) 
            {
            }

            try 
            {
                return Single.Parse(arg); // Value is Single
            } 
            catch (FormatException) 
            {
            }

            try 
            {
                return Double.Parse(arg); // Value is Double
            } 
            catch (FormatException) 
            {
            }

            //Parse Booleans
            if (arg.ToLower().Equals("true")) {
                return true;
            }

            if (arg.ToLower().Equals("false")) 
            {
                return false;
            }

            if (arg.StartsWith("'") && arg.EndsWith("'") && arg.Length == 3) 
            {
                return arg.ToCharArray()[1]; // Value is char
            }


            String tmp = "";
            foreach(String s in args) 
            {
                if (tmp.Equals("")) 
                {
                    tmp = s;
                } 
                else 
                {
                    tmp += " " + s;
                }
            }

            if(tmp.StartsWith("[") && tmp.EndsWith("]"))
            {
                string[] list = tmp.Substring(1, tmp.Length - 1).Split(new[] {", "}, StringSplitOptions.None);
                return list.Select(s => ParseValue(s.Split(' '), user)).ToArray();
            }

            if (!tmp.StartsWith("\"") || !tmp.EndsWith("\"")) throw new ArgumentException("Unknown value: " + tmp);
            tmp = tmp.ReplaceFirstOccurrence("\"", "");
            tmp = tmp.ReplaceLastOccurrence("\"", "");
            return tmp;
        }



        protected abstract Object Eval(ScriptContext context, String code, out bool resultSet, bool silent);

        public abstract String GetDefaultImports();
        public abstract void SetVariable(ScriptContext context, String name, Object value);
        public abstract List<string> GetImportIdentifiers(); 
        public abstract Object CreateBindings(EntUser user, ServerEntity callback);

        private void AutoStartRecur(String directory, ScriptContext context)
        {
            List<String> allFiles = Directory.GetFiles(directory).ToList();
            allFiles.AddRange(Directory.GetDirectories(directory));

            foreach (string file in allFiles)
            {
                FileAttributes attr = File.GetAttributes(file);
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    AutoStartRecur(file, context);
                }
                else
                {
                    if (!file.EndsWith(FileExtension))
                    {
                        continue;
                    }

                    context.MessageCallBack.SendMessage(
                        ColorCode.DARK_GREEN + "[AutoStart] " + ColorCode.OLIVE + Name + ColorCode.WHITE + ": " +
                        file);

                    ScriptHandler.GetInstance().SetDefaultVariables(context);
                    bool resultSet;
                    Run(context, file, out resultSet);
                }
            }
        }

        public virtual void PreInit()
        {
            
        }

        public virtual void Init()
        {
            
        }

        public void PerformAutoStart(ScriptContext context)
        {
            AutoStartRecur(AutostartDirectory, context);
        }

        public abstract object FormatCode(string code);
    }
}