
using Shinkei.API.Entities;

namespace Shinkei.IRC.Entities
{
    public class EntChannel : ServerEntity
    {
        private readonly string _name;
        
        public string Name
        { 
            get
            {
                return _name;
            }
        }

        public EntChannel (Server server, string name) : base(server)
        {
            _name = name;
        }

        public override string GetName()
        {
            return Name;
        }
    }
}
