using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shinkei;
using System.ComponentModel.Composition;
using Shinkei.IRC;
using Shinkei.IRC.Messages;

[Export(typeof(Shinkei.IPlugin))]
[ExportMetadata("Name", "RandomPlugin")]
[ExportMetadata("Version", "0.1")]
[ExportMetadata("Author", "rinukkusu")]
public class RandomPlugin : IPlugin
{
    public bool IsEnabled()
    {
        return true;
    }

    public void RegisterEvents(Eventsink Eventdata)
    {
        Eventdata.OnIrcMessage += new Eventsink.IrcMessageDelegate(this.IrcMessageHandler);
    }

    public void IrcMessageHandler(Message MessageData)
    {
        Console.WriteLine("Pluginhandler");
    }
}
