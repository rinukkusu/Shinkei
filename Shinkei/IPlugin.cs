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

        void RegisterEvents(IRC.Eventsink Eventdata);
        List<CommandDescription> GetCommands();
    }

    public class CommandDescription
    {
        private string _Command;
        public string Command
        {
            get { return _Command; }
        }

        private string _Usage;
        public string Usage
        {
            get { return _Usage; }
        }

        private string _Description;
        public string Description
        {
            get { return _Description; }
        }

        public CommandDescription(string _Command, string _Usage, string _Description)
        {
            this._Command = _Command;
            this._Usage = _Usage;
            this._Description = _Description;
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
                        return x.Command.CompareTo(y);
                    }
                }
            }
        }
    }

}
