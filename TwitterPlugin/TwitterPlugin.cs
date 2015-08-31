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
using System.Threading;

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
        public TwitterContext ctx;
        public PinAuthorizer auth;

        public PinAuthorizer DoXAuth(string token, string secret)
        {
            Console.WriteLine("  New Authorizer ...");
            PinAuthorizer auth = new PinAuthorizer();

            Console.WriteLine("  New Credentials ...");
            InMemoryCredentialStore CredentialStore = new InMemoryCredentialStore
            {
                ConsumerKey = Settings.ConsumerKey,
                ConsumerSecret = Settings.ConsumerSecret,
                OAuthToken = token,
                OAuthTokenSecret = secret
            };

            Console.WriteLine("  Setting Credentials in Authorizer ...");
            auth.CredentialStore = CredentialStore;

            Console.WriteLine("  returning Authorizer ...");
            return auth;
        }

        public override void OnEnable()
        {
            _settingsPath = Path.Combine(DataDirectory, "TwitterPlugin.json");

            Console.WriteLine("Loading TwitterSettings ...");
            Settings = LoadSettings();

            Console.WriteLine("Instantiating PinAuthorizer ...");
            Console.WriteLine("-> Token: " + Settings.Accounts[0].OAuthToken);
            Console.WriteLine("-> Secret: " + Settings.Accounts[0].OAuthSecret);
            auth = DoXAuth(Settings.Accounts[0].OAuthToken, Settings.Accounts[0].OAuthSecret);

            if (String.IsNullOrWhiteSpace(auth.CredentialStore.OAuthToken))
            {
                Console.WriteLine("Attempting Twitter Auth ...");
                auth.AuthorizeAsync();
            }

            Console.WriteLine("Instantiating TwitterContext ...");
            ctx = new TwitterContext(auth);

            Console.WriteLine("Registering TwitterCommands ...");
            EventManager.GetInstance().RegisterEvents(this, this);
            CommandHandler.GetInstance().RegisterCommand(new Command("tweet", "tweet <text>", "Tweets with the given account.", new TwitterCommandExecutor(this)), this);

			Thread HighlightT = new Thread(HighlightThread);
			HighlightT.Start ();
        }

		public void HighlightThread()
		{
			string highlight = Settings.Accounts [0].Highlight;
			string s = Settings.Accounts[0].Channels.Keys.ElementAt(0);
			string c = Settings.Accounts[0].Channels[s][0];

            System.Threading.Thread.Sleep(5000);

			//while (true) 
			//{
				

                var searchResponse =
                    (from stream in ctx.Streaming
                     where stream.Type == StreamingType.Filter &&
                         stream.Track == highlight
                     select stream).StartAsync(async strm =>
                     {
                         if (!String.IsNullOrWhiteSpace(strm.Content))
                         {
                             EntChannel channel = new EntChannel(Server.GetServer(s), c);

                             try
                             {
                                 Tweet tweet = JsonHelper.DeserializeFromString<Tweet>(strm.Content);
                                 channel.SendMessage(ColorCode.BOLD + "Twitter: " + ColorCode.BOLD +
                                                    ColorCode.CYAN + "@" + tweet.user.screen_name + ColorCode.COLOR + "> " +
                                                    tweet.text);
                             }
                             catch {}

                         }
                     });				
			//}
		}

        private string _settingsPath;
        public TwitterSettings LoadSettings()
        {
            TwitterSettings newSettings;

            Console.WriteLine("  Checking Settings existance ...");
            if (!File.Exists(_settingsPath))
            {
                newSettings = new TwitterSettings
                {
                    Accounts = new List<TwitterSettings.TwitterAccount>(),
                    ConsumerKey = "COSNUMER_KEY",
                    ConsumerSecret = "CONSUMER_SECRET"
                };

                TwitterSettings.TwitterAccount acc = new TwitterSettings.TwitterAccount();
                acc.OAuthToken = "AUTH_TOKEN";
                acc.OAuthSecret = "AUTH_SECRET";
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
                Console.WriteLine("  Deserializing Settings ...");
                newSettings = JsonHelper.Deserialize<TwitterSettings>(_settingsPath);
            }

            Console.WriteLine("  Returning Settings ...");
            return newSettings;
        }

        public void SaveSettings()
        {
            JsonHelper.Serialize<TwitterSettings>(Settings, _settingsPath);
        }
    }
}
