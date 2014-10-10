using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shinkei.IRC.Messages
{
    public class RawMessage
    {
        private Server _Server;
        public Server Server
        {
            get { return _Server; }
        }

        private int _ResponseCode;
        public int ResponseCode
        {
            get { return _ResponseCode; }
        }

        private string _RawLine;
        public string RawLine
        {
            get { return _RawLine; }
        }

        public RawMessage(Server _Server, int _ResponseCode, string _RawLine)
        {
            this._Server = _Server;
            this._ResponseCode = _ResponseCode;
            this._RawLine = _RawLine;
        }
    }
}
