using System.Linq;

namespace Shinkei.IRC.Entities
{
    public class EntUser : IEntity
    {
        private string _Username;
        public string Username
        { 
            get
            {
                return _Username;
            }
        }

        private string _Host;
        public string Host
        { 
            get
            {
                return _Host;
            }
        }

        private string _Nickname;
        public string Nickname
        { 
            get
            {
                return _Nickname;
            }
        }

        public EntUser (string Name)
        {
            if (Name.Contains('@'))
            {
                string Nickparts = Name.Split('@')[0];
                if (Nickparts.Contains('!'))
                {
                    _Nickname = Nickparts.Split('!')[0];
                    _Username = Nickparts.Split('!')[1];
                }
                _Host = Name.Split('@')[1];
            }
            else
            {
                _Nickname = Name;
            }
        }
    }
}
