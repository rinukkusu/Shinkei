using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;
using System.IO;
using System.Runtime.Serialization;
using System.Threading;
using Shinkei;
using Shinkei.API;
using Shinkei.API.Events;
using Shinkei.IRC.Events;
using Shinkei.API.Commands;
using Shinkei.IRC.Commands;
using Shinkei.IRC.Entities;
using YouTubePlugin.Commands;
using System.Text.RegularExpressions;
using Shinkei.API.Entities;


namespace YouTubePlugin
{
    [Export(typeof(Plugin))]
    [ExportMetadata("Name", "YouTube")]
    [ExportMetadata("Version", "0.1")]
    [ExportMetadata("Author", "rinukkusu")]
    [ExportMetadata("Description", "Grabs metadata via the YouTube API.")]
    public class YouTubePlugin : Plugin, IListener
    {
        private YouTubeSettings _settings;

        public YouTubeSettings Settings
        {
            get { return _settings; }
        }

        private YouTubeInterface _ytInterface;

        public YouTubeInterface YTInterface
        {
            get { return _ytInterface; }
        }

        [DataContract]
        public class YouTubeSettings
        {
            [DataMember]
            public string ApiKey = "";

            [DataMember]
            public int MaxResults = 1;

            [DataMember]
            public string SearchResultFormat = "%link% :: %title% :: %category% :: Duration: %length% :: Views: %views% :: Rating: %rating%";
        }

        public override void OnEnable()
        {
            _settings = LoadSettings();
            _ytInterface = new YouTubeInterface(this);

            EventManager.GetInstance().RegisterEvents(this, this);
            CommandHandler.GetInstance().RegisterCommand(new Command("yt", "yt <keywords>", "Searches for a YouTube video matching the keywords.", new YouTubeCommandExecutor(YTInterface)), this);
        }

        [Shinkei.IRC.Events.EventHandler]
        public void OnIrcMessage(IrcMessageEvent evnt)
        {
            if (evnt.Recipient.GetType() == typeof(EntChannel))
            {
                Regex reg = new Regex(@"youtu.*v=([A-Za-z0-9_-]+)");
                if (reg.IsMatch(evnt.Text))
                {
                    var matches = reg.Matches(evnt.Text);
                    string Id = matches[0].Groups[1].Captures[0].Value;

                    ServerEntity answerRcpt;
                    answerRcpt = (ServerEntity) evnt.Recipient;

                    var Info = YTInterface.GetVideoInfo(Id);

                    if (Info != null) 
                    {
                        string response = YTInterface.FormatResponse(Info);
                        answerRcpt.SendMessage(response);
                    }
                }
            }
        }

        public YouTubeSettings LoadSettings()
        {
            string path = Path.Combine(DataDirectory, "YouTubePlugin.json");
            YouTubeSettings newSettings;

            if (!File.Exists(path))
            {
                newSettings = new YouTubeSettings();

                JsonHelper.Serialize<YouTubeSettings>(newSettings, path);
            }
            else
            {
                newSettings = JsonHelper.Deserialize<YouTubeSettings>(path);
            }

            return newSettings;
        }
    }
}
