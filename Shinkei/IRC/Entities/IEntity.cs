
using System;

namespace Shinkei.IRC.Entities
{
    public interface IEntity
    {
        string GetName();

        void SendMessage(String s);
    }
}
