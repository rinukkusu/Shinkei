using System.Linq;

namespace Shinkei.IRC.Entities
{
    public class EntUser : IEntity
    {
        private string _username;
        public string Username
        { 
            get
            {
                return _username;
            }
        }

        private string _host;
        public string Host
        { 
            get
            {
                return _host;
            }
        }

        private string _nickname;
        public string Nickname
        { 
            get
            {
                return _nickname;
            }
        }

        public EntUser (string name)
        {
            if (name.Contains('@'))
            {
                string nickparts = name.Split('@')[0];
                if (nickparts.Contains('!'))
                {
                    _nickname = nickparts.Split('!')[0];
                    _username = nickparts.Split('!')[1];
                }
                _host = name.Split('@')[1];
            }
            else
            {
                _nickname = name;
            }
        }

        public string GetName()
        {
            return this.Nickname;
        }
    }
}
