using System;
using Shinkei.API.Commands;
using Shinkei.IRC;
using Shinkei.IRC.Entities;
using Shinkei.IRC.Messages;

namespace SandraPlugin.Commands
{
    public class MarkovCommandExecutor : ICommandExecutor
    {
        private readonly SandraPlugin _plugin;

        public MarkovCommandExecutor(SandraPlugin plugin)
        {
            _plugin = plugin;
        }

        public bool Execute(Command command, EntUser executor, CommandMessage data)
        {
            if (data.Arguments.Count < 1)
            {
                return false;
            }

            if (_plugin.IsMarkovEnabled(executor.GetName()))
            {
                String sentence = default(String);
                if (data.Arguments.Count == 1)
                {
                    String keyword = data.Arguments[0];
                    sentence = _plugin.Markov.GetSentence(keyword);
                }

                if (String.IsNullOrWhiteSpace(sentence))
                {
                    data.SendResponse(ColorCode.RED + "Error: Ungültiges Startwort");
                }
                else
                {
                    data.SendResponse(sentence);
                }
            }

            return true;
        }
    }
}