using System;
using System.Collections.Generic;
using Shinkei.API.Commands;

namespace Shinkei.API
{
    public interface IPluginData
    {
        string Name { get; }
        string Version { get; }
        string Author { get; }
        string Description { get; }
    }

    public abstract class Plugin
    {
        public bool IsEnabled()
        {
            return true;
        }

        public void Disable()
        {
            if (!IsEnabled())
            {
                return;
            }

            throw new NotImplementedException();
        }

        public void Enable()
        {
            if (IsEnabled())
            {
                return;
            }

            throw new NotImplementedException();
        }

        public virtual void OnEnable()
        {
            
        }
    }

    public class CommandComparer : IComparer<Command>
    {
        public int Compare(Command x, Command y)
        {
            if (x.CommandName == null)
            {
                if (y.CommandName == null)
                {
                    // If x is null and y is null, they're 
                    // equal.  
                    return 0;
                }
                // If x is null and y is not null, y 
                // is greater.  
                return -1;
            }
            // If x is not null... 
            // 
            if (y.CommandName == null)
                // ...and y is null, x is greater.
            {
                return 1;
            }
            // ...and y is not null, compare the  
            // lengths of the two strings. 
            // 
            int retval = x.CommandName.Length.CompareTo(y.CommandName.Length);

            if (retval != 0)
            {
                // If the strings are not of equal length, 
                // the longer string is greater. 
                // 
                return retval;
            }
            // If the strings are of equal length, 
            // sort them with ordinary string comparison. 
            // 
            return String.Compare(x.CommandName, y.CommandName, StringComparison.Ordinal);
        }
    }

}
