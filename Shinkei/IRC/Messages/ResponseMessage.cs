using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shinkei.IRC.Messages
{
    public class ResponseMessage
    {
        private Server _serverInstance;
        public Server ServerInstance
        {
            get { return _serverInstance; }
        }

        private int _responseCode;
        public int ResponseCode
        {
            get { return _responseCode; }
        }

        private string _rawLine;
        public string RawLine
        {
            get { return _rawLine; }
        }

        public ResponseMessage(Server server, int responseCode, string rawLine)
        {
            this._serverInstance = server;
            this._responseCode = responseCode;
            this._rawLine = rawLine;
        }
    }
}
