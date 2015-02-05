namespace Shinkei.IRC.Messages
{
    public class ResponseMessage
    {
        private readonly Server _serverInstance;
        public Server ServerInstance
        {
            get { return _serverInstance; }
        }

        private readonly int _responseCode;
        public int ResponseCode
        {
            get { return _responseCode; }
        }

        private readonly string _rawLine;
        public string RawLine
        {
            get { return _rawLine; }
        }

        public ResponseMessage(Server server, int responseCode, string rawLine)
        {
            _serverInstance = server;
            _responseCode = responseCode;
            _rawLine = rawLine;
        }
    }
}
