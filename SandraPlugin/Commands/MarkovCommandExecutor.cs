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

            if (data.Arguments.Count > 1)
            {
                goto failed;
            }

            String keyword = data.Arguments[0];
            String sentence = _plugin.Markov.GetSentence(keyword);
            if (sentence == null || sentence.Trim() == "")
            {
                goto failed;
            }

            data.SendResponse(sentence);
            return true;
            
         failed:
            data.SendResponse(ColorCode.RED + "Error: Ungültiges Startwort");
            return true;
        }
    }
}