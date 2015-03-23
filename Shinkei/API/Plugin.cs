using System;
using System.Collections.Generic;
using System.IO;
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
        private IPluginData _data;
        private bool _enabled = true;

        public IPluginData Metadata
        {
            get { return _data; }

            //Dirty
            set
            {
                if (_data != null)
                {
                    throw new InvalidOperationException("Metadata is readonly");
                }

                _data = value;
            }
        }

        public bool IsEnabled()
        {
            return _enabled;
        }

        public void Disable()
        {
            if (!IsEnabled())
            {
                return;
            }
            //_enabled = false;
            //OnDisable();
            throw new NotImplementedException();
        }

        public void Enable()
        {
            if (IsEnabled())
            {
                return;
            }
            //_enabled = true;
            //OnEnable();
            throw new NotImplementedException();
        }

        public virtual void OnEnable()
        {
            
        }

        public virtual void OnDisable()
        {
            
        }

        public String DataDirectory
        {
            get
            {
                String directory = Path.Combine("plugins", Metadata.Name);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                return directory;
            }
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
