using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shinkei.IRC.Exceptions
{
    public abstract class ConnectionException : Exception
    {
        public ConnectionException(String msg) : base(msg)
        {
            
        }
    }
}
