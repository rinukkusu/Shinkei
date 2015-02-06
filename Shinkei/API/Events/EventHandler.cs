using System;

namespace Shinkei.IRC.Events
{
    [AttributeUsage(AttributeTargets.Method)]
    public class EventHandler : Attribute
    {
        public EventPriority Priority = EventPriority.NORMAL;
        public bool IgnoreCancelled = false;
    }
}