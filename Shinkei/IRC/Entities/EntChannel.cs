
namespace Shinkei.IRC.Entities
{
    public class EntChannel : IEntity
    {
        private readonly string _name;
        public string Name
        { 
            get
            {
                return _name;
            }
        }

        public EntChannel (string name)
        {
            _name = name;
        }

        public string GetName()
        {
            return Name;
        }
    }
}
