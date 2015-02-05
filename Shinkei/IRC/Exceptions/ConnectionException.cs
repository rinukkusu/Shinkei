using System;

namespace Shinkei.IRC.Exceptions
{
    public abstract class ConnectionException : Exception
    {
        protected ConnectionException(String msg) : base(msg)
        {
            
        }
    }
}
