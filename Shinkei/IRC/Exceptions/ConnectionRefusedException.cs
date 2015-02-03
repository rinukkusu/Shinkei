using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
