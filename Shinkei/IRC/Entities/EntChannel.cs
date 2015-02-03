
namespace Shinkei.IRC.Entities
{
    public class EntChannel : IEntity
    {
        private string _name;
        public string Name
        { 
            get
            {
                return _name;
            }
        }

        public EntChannel (string name)
        {
            this._name = name;
        }

        public string GetName()
        {
            return this.Name;
        }
    }
}
