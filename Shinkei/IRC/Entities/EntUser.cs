using System.Linq;
using Shinkei.API.Commands;

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

        protected CommandPermission _permission;

        public EntUser (string name)
        {
            if (name.Contains('@'))
            {
                _permission = CommandPermission.OP;
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
                _permission = CommandPermission.NONE;
                _nickname = name;
            }
            //Todo: implement whitelist and voice permission
        }

        public string GetName()
        {
            return Nickname;
        }
        

        public virtual bool HasPermission(CommandPermission permission)
        {
            return (_permission & permission) != 0;
        }
    }
}
