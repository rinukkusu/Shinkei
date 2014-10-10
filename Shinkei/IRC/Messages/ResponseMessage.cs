using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shinkei.IRC.Messages
{
    public class ResponseMessage
    {
        private Server _ServerInstance;
        public Server ServerInstance
        {
            get { return _ServerInstance; }
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

        public ResponseMessage(Server _Server, int _ResponseCode, string _RawLine)
        {
            this._ServerInstance = _Server;
            this._ResponseCode = _ResponseCode;
            this._RawLine = _RawLine;
        }
    }
}
