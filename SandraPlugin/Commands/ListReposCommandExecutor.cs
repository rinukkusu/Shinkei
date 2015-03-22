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
            var answerRcpt = data.Recipient.GetType() == typeof(EntUser) ? data.Sender : data.Recipient;

            if (_plugin.Repos.Count == 0)
            {
                data.ServerInstance.PrivateMessage(answerRcpt, executor.GetName() + ColorCode.RED + "Keine Reporsitories vorhanden.");
            }

            String s = "";
            foreach (String repo in _plugin.Repos)
            {
                if (s.Equals(""))
                {
                    s = repo;
                    continue;
                }

                s += ", " + repo;
            }

            data.ServerInstance.PrivateMessage(answerRcpt, s);
            return true;
        }
    }
}