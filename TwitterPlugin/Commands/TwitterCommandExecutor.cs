using System;
using LinqToTwitter;
using Shinkei.API.Commands;
using Shinkei.IRC.Entities;
using Shinkei.IRC.Messages;

namespace TwitterPlugin.Commands
{
    class TwitterCommandExecutor : ICommandExecutor
    {
        private readonly TwitterContext _ctx;
        private readonly TwitterPlugin _plugin;

        public TwitterCommandExecutor(TwitterPlugin _plugin)
        {
            this._plugin = _plugin;
            this._ctx = _plugin.ctx;
        }

        public bool Execute(Command command, EntUser executor, CommandMessage data)
        {
            if (data.Arguments.Count < 1)
            {
                return false;
            }

            if (command.CommandName.Equals("tweet"))
            {
                String status = default(String);
                foreach (string keyword in data.Arguments)
                {
                    status += keyword + " ";
                }

                status = status.Trim();

                _ctx.TweetAsync(status);
                data.SendResponse("Nice Tweet! :3");


                return true;
            }

            if (command.CommandName.Equals("follow"))
            {
                String target = data.Arguments[0];
                _plugin.Follow(target);
                return true;
            }

            return false;
        }
    }
}
