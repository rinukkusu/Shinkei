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
            CommandHandler.GetInstance().RegisterCommand(new Command("follow", "follow <name>", "Follows the given account.", new TwitterCommandExecutor(this)), this);

			Thread HighlightT = new Thread(HighlightThread);
			HighlightT.Start ();

            Thread tweets = new Thread(TweetsListener);
            tweets.Start();
        }

        private void TweetsListener()
        {
            while (IsEnabled())
            {
                Thread.Sleep(10 * 1000);

                if (Settings.Follows == null || Settings.Follows.Count == 0)
                {
                    continue;
                }

                Console.WriteLine("TweetListener()");

                foreach (String s in Settings.Follows)
                {
                    var tweets = (from tweet in ctx.Status where tweet.Type == StatusType.Show && tweet.ScreenName.ToLower().Equals(s.ToLower()) select tweet).ToList();
                    var tweetsBefore = tweetCache.ContainsKey(s) ? tweetCache[s] : tweets;

                    if (tweetCache.ContainsKey(s)) tweetCache.Remove(s);
                    tweetCache.Add(s, tweets);

                    if(tweets.First() == null) continue;
                    if(tweets.First().ID.Equals(tweetsBefore.First().ID)) continue;
                    
                    foreach(Status status in tweets)
                    {
                        if (status.ID.Equals(tweetsBefore.First().ID)) break;
                        AnnounceTweet(status);
                    }
                }
            }
        }

        private void AnnounceTweet(Status status)
        {
            string s = Settings.Accounts[0].Channels.Keys.ElementAt(0);
            string c = Settings.Accounts[0].Channels[s][0];
            EntChannel channel = new EntChannel(Server.GetServer(s), c);
            channel.SendMessage("@" + status.ScreenName + ": " + status.Text);
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
                     where stream.Type == StreamingType.User
                     select stream).StartAsync(async strm =>
                     {
                         if (!String.IsNullOrWhiteSpace(strm.Content))
                         {
                             EntChannel channel = new EntChannel(Server.GetServer(s), c);

                             try
                             {
                                 Tweet tweet = JsonHelper.DeserializeFromString<Tweet>(strm.Content);
                                 if (tweet != default(Tweet))
                                 {
                                     if (tweet.text.StartsWith("RT") && tweet.text.Contains(highlight))
                                     {
                                         tweet.text = tweet.text.Remove(0, 3);
                                         channel.SendMessage(ColorCode.BOLD + "Twitter: " + ColorCode.BOLD +
                                                            ColorCode.CYAN + "@" + tweet.user.screen_name + ColorCode.COLOR + ColorCode.GREEN + " retweetete " + ColorCode.COLOR +
                                                            tweet.text);
                                     }
                                     else if (tweet.text.Contains(highlight))
                                     {
                                         channel.SendMessage(ColorCode.BOLD + "Twitter: " + ColorCode.BOLD +
                                                            ColorCode.CYAN + "@" + tweet.user.screen_name + ColorCode.COLOR + "> " +
                                                            tweet.text);
                                     }
                                 }

                                 FavoriteEvent fav_event = JsonHelper.DeserializeFromString<FavoriteEvent>(strm.Content);
                                 if (fav_event != default(FavoriteEvent))
                                 {
                                     if (fav_event.@event == "favorite" && fav_event.target.screen_name == highlight)
                                     {
                                         channel.SendMessage(ColorCode.BOLD + "Twitter: " + ColorCode.NORMAL +
                                                     ColorCode.CYAN + "@" + fav_event.source.screen_name + ColorCode.COLOR + ColorCode.YELLOW + " favorisierte " + ColorCode.COLOR +
                                                     ColorCode.CYAN + "@" + fav_event.target.screen_name + ColorCode.COLOR + " " + fav_event.target_object.text);
                                     }
                                 }

                                 FollowEvent fol_event = JsonHelper.DeserializeFromString<FollowEvent>(strm.Content);
                                 if (fol_event != default(FollowEvent))
                                 {
                                     if (fol_event.@event == "follow" && fol_event.target.screen_name == highlight)
                                     {
                                         channel.SendMessage(ColorCode.BOLD + "Twitter: " + ColorCode.NORMAL +
                                                     ColorCode.CYAN + "@" + fav_event.source.screen_name + ColorCode.COLOR + ColorCode.DARK_GREEN + " folgt nun.");
                                     }
                                 }
                             }
                             catch {
                                 string t = strm.Content;
                             }

                         }
                     });				
			//}
		}

        private string _settingsPath;
        private Dictionary<String, List<Status>> tweetCache = new Dictionary<string, List<Status>>();

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

        public void Follow(string target)
        {
            if (Settings.Follows == null)
            {
                Settings.Follows = new List<string>();
            }

            Settings.Follows.Add(target);

            JsonHelper.Serialize<TwitterSettings>(Settings, _settingsPath);
        }
    }
}
