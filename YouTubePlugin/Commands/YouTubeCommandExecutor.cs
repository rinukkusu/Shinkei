using Shinkei.API.Commands;
using Shinkei.IRC.Entities;
using Shinkei.IRC.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTubePlugin.Commands
{
    class YouTubeCommandExecutor : ICommandExecutor
    {
        private readonly YouTubeInterface _interface;

        public YouTubeCommandExecutor(YouTubeInterface _interface)
        {
            this._interface = _interface;
        }

        public bool Execute(Command command, EntUser executor, CommandMessage data)
        {
            if (data.Arguments.Count < 1)
            {
                return false;
            }

            String keywords = default(String);
            foreach (string keyword in data.Arguments)
            {
                keywords += keyword + " ";
            }

            var videos = _interface.SearchVideos(keywords.Trim());

            if (videos.Count() > 0)
            {
                data.SendResponse(String.Format("%i videos found:", videos.Count()));
                foreach (var video in videos)
                {
                    data.SendResponse(_interface.FormatResponse(video));
                }
            }
            else
            {
                data.SendResponse("No videos found");
            }

            return true;
        }
    }
}
