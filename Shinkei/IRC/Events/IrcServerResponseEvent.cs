namespace Shinkei.IRC.Events
{
    public class IrcServerResponseEvent : Event
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

        public IrcServerResponseEvent(Server serverInstance, int responseCode, string rawLine)
        {
            _rawLine = rawLine;
            _responseCode = responseCode;
            _serverInstance = serverInstance;
        }
    }
}