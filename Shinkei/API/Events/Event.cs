using System;

namespace Shinkei.IRC.Events
{
    public abstract class Event
    {
        private readonly string _name;
        public String Name
        {
            get { return _name;  }
        }

        private readonly bool _isAsync;
        public bool IsAsync
        {
            get { return _isAsync; }    
        }

        protected Event() : this(false) { }

        protected Event(bool isAsync)
        {
            _name = GetType().Name;
            _isAsync = isAsync;
        }
    }
}