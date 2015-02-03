using System.Collections.Generic;

namespace Shinkei
{
    public interface IPluginData
    {
        string Name { get; }
        string Version { get; }
        string Author { get; }
        string Description { get; }
    }

    public interface IPlugin
    {
        bool IsEnabled();

        void RegisterEvents(IRC.Eventsink eventdata);
        List<CommandDescription> GetCommands();
    }

    public class CommandDescription
    {
        private string _command;
        public string Command
        {
            get { return _command; }
        }

        private string _usage;
        public string Usage
        {
            get { return _usage; }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
        }

        public CommandDescription(string command, string usage, string description)
        {
            this._command = command;
            this._usage = usage;
            this._description = description;
        }
    }

    public class CommandComparer : IComparer<CommandDescription>
    {
        public int Compare(CommandDescription x, CommandDescription y)
        {
            if (x.Command == null)
            {
                if (y.Command == null)
                {
                    // If x is null and y is null, they're 
                    // equal.  
                    return 0;
                }
                else
                {
                    // If x is null and y is not null, y 
                    // is greater.  
                    return -1;
                }
            }
            else
            {
                // If x is not null... 
                // 
                if (y.Command == null)
                // ...and y is null, x is greater.
                {
                    return 1;
                }
                else
                {
                    // ...and y is not null, compare the  
                    // lengths of the two strings. 
                    // 
                    int retval = x.Command.Length.CompareTo(y.Command.Length);

                    if (retval != 0)
                    {
                        // If the strings are not of equal length, 
                        // the longer string is greater. 
                        // 
                        return retval;
                    }
                    else
                    {
                        // If the strings are of equal length, 
                        // sort them with ordinary string comparison. 
                        // 
                        return x.Command.CompareTo(y.Command);
                    }
                }
            }
        }
    }

}
