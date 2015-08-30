using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;
using Shinkei;
using Shinkei.API;
using Shinkei.API.Commands;
using Shinkei.API.Events;
using Shinkei.IRC;
using Shinkei.IRC.Commands;
using Shinkei.IRC.Entities;
using Shinkei.IRC.Events;
using LinqToTwitter;
using System.IO;
using TwitterPlugin.Commands;

namespace TwitterPlugin
{
    [Export(typeof(Plugin))]
    [ExportMetadata("Name", "Twitter")]
    [ExportMetadata("Version", "0.1")]
    [ExportMetadata("Author", "rinukkusu")]
    [ExportMetadata("Description", "A twitter interface.")]
    public class TwitterPlugin : Plugin, IListener
    {
        public TwitterSettings Settings { get; private set; }
        private TwitterContext ctx;
        private IAuthorizer auth;

        private IAuthorizer DoXAuth(TwitterSettings.TwitterAccount acc)
        {
            var auth = new SingleUserAuthorizer
            {
                CredentialStore = new SingleUserInMemoryCredentialStore
                {
                    ConsumerKey = Settings.ConsumerKey,
                    ConsumerSecret = Settings.ConsumerSecret,
                    AccessToken = acc.AuthToken,
                    AccessTokenSecret = acc.AuthSecret
                }
            };

            return auth;
        }

        public override async void OnEnable()
        {
            _settingsPath = Path.Combine(DataDirectory, "TwitterPlugin.json");

            Settings = LoadSettings();

            auth = DoXAuth(Settings.Accounts[0]);
            await auth.AuthorizeAsync();

            ctx = new TwitterContext(auth);

            EventManager.GetInstance().RegisterEvents(this, this);
            CommandHandler.GetInstance().RegisterCommand(new Command("tweet", "tweet <text>", "Tweets with the given account.", new TwitterCommandExecutor(ctx)), this);
        }


        private string _settingsPath;
        public TwitterSettings LoadSettings()
        {
            TwitterSettings newSettings;

            if (!File.Exists(_settingsPath))
            {
                newSettings = new TwitterSettings
                {
                    Accounts = new List<TwitterSettings.TwitterAccount>(),
                    ConsumerKey = "COSNUMER_KEY",
                    ConsumerSecret = "CONSUMER_SECRET"
                };

                TwitterSettings.TwitterAccount acc = new TwitterSettings.TwitterAccount();
                acc.AuthToken = "AUTH_TOKEN";
                acc.AuthSecret = "AUTH_SECRET";
                acc.Channels = new Dictionary<string,List<string>>();
                List<string> channels = new List<string>();
                channels.Add("CHANNEL_1");
                channels.Add("CHANNEL_2");
                acc.Channels.Add("SERVER_ALIAS", channels);
                newSettings.Accounts.Add(acc);

                JsonHelper.Serialize<TwitterSettings>(newSettings, _settingsPath);
            }
            else
            {
                newSettings = JsonHelper.Deserialize<TwitterSettings>(_settingsPath);
            }

            return newSettings;
        }

        public void SaveSettings()
        {
            JsonHelper.Serialize<TwitterSettings>(Settings, _settingsPath);
        }
    }
}
