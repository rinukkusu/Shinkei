using System;
using System.Linq;
using Shinkei.API.Commands;
using Shinkei.API.Entities;

namespace Shinkei.IRC.Entities
{
    public class EntUser : ServerEntity
    {
        private readonly string _username;
        public string Username
        { 
            get
            {
                return _username;
            }
        }

        private readonly string _host;
        public string Host
        { 
            get
            {
                return _host;
            }
        }

        private readonly string _nickname;
        public string Nickname
        { 
            get
            {
                return _nickname;
            }
        }

        protected CommandPermission Permission;

        public EntUser (Server server, EntChannel channel, string name) : base(server)
        {
            if (name.Contains('@'))
            {
                Permission = CommandPermission.OP;
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
                Permission = CommandPermission.NONE;
                _nickname = name;
            }
            //Todo: implement whitelist and voice permission
        }

        public override string GetName()
        {
            return Nickname;
        }

        public virtual bool HasPermission(CommandPermission permission)
        {
            if (Permission == CommandPermission.NONE)
            {
                return false;
            }
            return Permission >= permission;
        }
    }
}
