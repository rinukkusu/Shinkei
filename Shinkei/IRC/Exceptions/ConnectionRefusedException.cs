namespace Shinkei.IRC.Exceptions
{
    public class ConnectionRefusedException : ConnectionException, IReportable
    {
        public ConnectionRefusedException()
            : base("Connection Refused")
        {
        }
        
        public string GetReportableMessage()
        {
            return Message;
        }
    }
}
