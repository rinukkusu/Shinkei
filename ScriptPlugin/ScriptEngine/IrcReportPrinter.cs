using System;
using System.IO;
using System.Text;
using Mono.CSharp;
using Shinkei.API.Entities;
using Shinkei.IRC.Entities;

namespace ScriptPlugin.ScriptEngine
{
    public class IrcReportPrinter : StreamReportPrinter
    {
        public IrcReportPrinter(EntUser user, ServerEntity callback)
            : base(new IrcTextWriter(user, callback))
        {
        }
    }

    public class IrcTextWriter : TextWriter
    {
        private readonly EntUser _user;
        private readonly ServerEntity _callback;
        public IrcTextWriter(EntUser user, ServerEntity callback)
        {
            _user = user;
            _callback = callback;
        }
        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }

        public override void WriteLine(String s)
        {
            _callback.SendMessage(_user.Nickname + ": " + s);
        }
    }
}