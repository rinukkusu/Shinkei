
using System;

namespace Shinkei.API.Entities
{
    public interface IEntity
    {
        string GetName();

        void SendMessage(String s);
    }
}
