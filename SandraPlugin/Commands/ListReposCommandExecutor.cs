using System;
using Shinkei.API.Commands;
using Shinkei.IRC;
using Shinkei.IRC.Entities;
using Shinkei.IRC.Messages;

namespace SandraPlugin.Commands
{
    public class ListReposCommandExecutor : ICommandExecutor
    {
        private SandraPlugin _plugin;

        public ListReposCommandExecutor(SandraPlugin plugin)
        {
            _plugin = plugin;
        }

        public bool Execute(Command command, EntUser executor, CommandMessage data)
        {
            if (_plugin.Settings.Repos.Count == 0)
            {
                data.SendResponse(ColorCode.RED + "Keine Repositorys vorhanden.");
                return true;
            }

            String s = "";
            foreach (String repo in _plugin.Settings.Repos)
            {
                if (s.Equals(""))
                {
                    s = repo;
                    continue;
                }

                s += ", " + repo;
            }

            data.SendResponse(s);
            return true;
        }
    }
}