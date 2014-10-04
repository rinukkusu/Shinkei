using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shinkei
{
    public interface IPluginData
    {
        string Name { get; }
        string Version { get; }
        string Author { get; }
    }


    public interface IPlugin
    {
        bool IsEnabled();

        void RegisterEvents(IRC.Eventsink Eventdata);
    }
}
