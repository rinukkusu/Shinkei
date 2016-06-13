using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml.Serialization;
using SandraPlugin.Commands;
using SandraPlugin.GitHub;
using Shinkei;
using Shinkei.API;
using Shinkei.API.Commands;
using Shinkei.API.Events;
using Shinkei.IRC;
using Shinkei.IRC.Commands;
using Shinkei.IRC.Entities;
using Shinkei.IRC.Events;
using ChatterBotAPI;
using TrelloNet;
using WolframAlphaNET;
using Action = TrelloNet.Action;

namespace SandraPlugin
{
    [Export(typeof(Plugin))]
    [ExportMetadata("Name", "Sandra")]
    [ExportMetadata("Version", "0.1")]
    [ExportMetadata("Author", "Trojaner")]
    [ExportMetadata("Description", "A bot with some useful developer functions.")]
    public class SandraPlugin: Plugin, IListener
    {
        private Dictionary<string, Feed> _feeds;
        private bool _listen;

        public WolframAlpha WolframAlphaClient { get; private set; }

        public SandraSettings Settings { get; private set; }

        public Markov Markov { get; private set; }

        public ChatterBotFactory Factory { get; private set; }

        private Dictionary<String, ChatterBotSession> sessions = new Dictionary<string, ChatterBotSession>(); 
        
        [DataContract]
        public class SandraSettings
        {
            [DataMember]
            public Dictionary<String, List<String>> CommitChannels;

            [DataMember]
            public List<String> Repos;

            [DataMember]
            public List<String> MarkovBlocked;

            [DataMember] 
            public string MarkovHost;

            [DataMember]
            public string MarkovDb;

            [DataMember]
            public string MarkovUser;

            [DataMember]
            public string MarkovPassword;

            [DataMember] 
            public string WolframAppId;

            [DataMember]
            public string TrelloId;

            [DataMember]
            public string TrelloBoard;
        }

        public static bool Validator(Object sender, 
                                System.Security.Cryptography.X509Certificates.X509Certificate certificate, 
                                System.Security.Cryptography.X509Certificates.X509Chain chain,
                                System.Net.Security.SslPolicyErrors xxlPolicyErrors)
        {
            return true;
        }

        public override void OnEnable()
        {
            ServicePointManager.ServerCertificateValidationCallback = Validator;
            Factory = new ChatterBotFactory();
            _settingsPath = Path.Combine(DataDirectory, "SandraPlugin.json");

            Settings = LoadSettings();
            if (!String.IsNullOrEmpty(Settings.TrelloId) && !String.IsNullOrEmpty(Settings.TrelloBoard))
            {
                InitTrello(Settings.TrelloId, Settings.TrelloBoard);
            }
            WolframAlphaClient = !String.IsNullOrWhiteSpace(Settings.WolframAppId) ? new WolframAlpha(Settings.WolframAppId) : null;

            try
            {
                Markov = new Markov(Settings.MarkovHost, Settings.MarkovDb, Settings.MarkovUser,
                    Settings.MarkovPassword);
                CommandHandler.GetInstance().RegisterCommand(new Command("markovblock", "markovblock <name>",
"Blacklists an user from markov.", new MarkovBlockCommandExecutor(this), CommandPermission.OP), this);
                CommandHandler.GetInstance().RegisterCommand(new Command("markovunblock", "markovunblock <name>",
    "Removen an user from markov Blacklist.", new MarkovUnblockCommandExecturo(this), CommandPermission.OP), this);
            }
            catch (Exception e)
            {
                Console.WriteLine("Couldn't enable markov: " + e);
                Markov = null;
            }
            _feeds = new Dictionary<string, Feed>();
            _listen = true;

            EventManager.GetInstance().RegisterEvents(this, this);
            CommandHandler.GetInstance().RegisterCommand(new Command("addrepo", "addrepo <username>/<repo>",
                 "Adds a github reporsitory and announces for new commits.", new AddRepoCommandExecutor(this),  CommandPermission.WHITELIST), this);
            CommandHandler.GetInstance().RegisterCommand(new Command("calc", "calc <query>", "Executes a Wolfram Alpha query", new CalcCommandExecutor(this)), this);
            CommandHandler.GetInstance().RegisterCommand(new Command("listrepos", "listrepo",
     "Shows all saved repos.", new ListReposCommandExecutor(this)), this);
            CommandHandler.GetInstance().RegisterCommand(new Command("m", "m <keyword>", "Outputs a markov-generated sentence", new MarkovCommandExecutor(this)), this);
            CommandHandler.GetInstance().RegisterCommand(new Command("removerepo", "removerepo <username>/<repo>",
                             "Removes a repo which was added previously.", new RemoveRepoCommandExecutor(this), CommandPermission.WHITELIST), this);
            CommandHandler.GetInstance().RegisterCommand(new Command("addcommitchannel", "addcommitchannel #<channel>",
                     "Adds a channel for announcing new commits.", new AddCommitChannelCommandExecutor(this), CommandPermission.OP), this);
            Thread thread = new Thread(OnCheck); 
            thread.Start();
        }

        private ITrello _trello;
        private Dictionary<string, Action> _trelloActions = new Dictionary<string, Action>();
        private Board _trelloBoard;

        private void OnCheckTrello()
        {
            while (_listen)
            {
                foreach (
                    var action in
                        GetActions(_trelloBoard).Where(action => !_trelloActions.ContainsKey(action.Id)))
                {
                    PublishTrelloAction(action);
                    _trelloActions.Add(action.Id, action);
                }
                Thread.Sleep(60 * 1000);
            }
        }

        private void PublishTrelloAction(Action action)
        {
            Dictionary<string, List<string>> commitChannels = Settings.CommitChannels;
            foreach (String serverIdentifier in commitChannels.Keys)
            {
                Server server = Server.GetServer(serverIdentifier);
                if (server == null)
                {
                    Console.WriteLine("[Sandra] Warning: Unknown server id:" + serverIdentifier);
                    continue;
                }

                foreach (String channelName in commitChannels[serverIdentifier])
                {
                    EntChannel channel = new EntChannel(server, channelName);
                    channel.SendMessage("Trello action: " + action.GetActionId());
                }
            }
        }

        private void InitTrello(string trelloId, string boardId)
        {
            _trello = new Trello(trelloId);
            _trelloBoard = _trello.Boards.WithId(boardId);
            if (_trelloBoard == null)
            {
                Console.WriteLine("[WARN] Trello Board with id " + boardId + " not found, skipping Trello listener");
                return;
            }
            foreach (Action action in GetActions(_trelloBoard))
            {
                _trelloActions.Add(action.Id, action);
            }
            Thread thread = new Thread(OnCheckTrello);
            thread.Start();
        }

        private IEnumerable<Action> GetActions(Board board)
        {
            IEnumerable<Action> actions = _trello.Actions.ForBoard(board);
            return actions ?? new List<Action>();
        }

        [Shinkei.IRC.Events.EventHandler]
        public void OnIrcMessage(IrcMessageEvent @event)
        {
            string sender = @event.Sender.GetName(); 
            var regex = new Regex("(^Sandra:?\\s)");
            var text = regex.Replace(@event.Text, "", 1).StripColors();
            bool isBotMessage = false;

            if (sender.ToLower().Contains("bot"))
            {
                if (!text.StartsWith("[Schrei]"))
                {
                    return;
                }

                regex = new Regex("\\[Schrei\\](?:\\s\\[.*\\])?\\s\\[.*\\]\\s(.*):\\s(.*)");
                var match = regex.Match(text);
                if (match.Groups.Count == 2)
                {
                    sender = match.Groups[0].Value;
                    text = match.Groups[1].Value;
                    isBotMessage = true;
                }
            }

            /* For now this doesnt work since Cleverbot changed their API's
            if (@event.Text.Contains("Sandra"))
            {
                string internalName = sender + "_IRC";
                var prefix = "";
                // Patch for SinkIRC bots /*
                if(isBotMessage) {
                    internalName = sender + "_INGAME";
                    prefix = "~say ";
                }

                try
                {
                    ChatterBotSession session;
                    if (sessions.ContainsKey(internalName))
                    {
                        session = sessions[internalName];
                    }
                    else
                    {
                        ChatterBot bot = Factory.Create(ChatterBotType.CLEVERBOT);
                        session = bot.CreateSession();
                        sessions.Add(internalName, session);
                    }

                    @event.Recipient.SendMessage(prefix + sender + ": " + session.Think(text));
                }
                catch (Exception e)
                {
                    Console.WriteLine("[Sandra] Chatbot error: " + e);
                }
            }
            
        Markov:
            */

            if (Markov == null)
            {
                return;
            }
            if (!IsMarkovEnabled(sender))
            {
                return;
            }

            Markov.AddSentence(text);
        }


        public bool IsMarkovEnabled(String name)
        {
            if (Markov == null)
            {
                return false;
            }

            if (!Markov.DatabaseAvailable())
                return false;

            return (Settings.MarkovBlocked == null) || !Settings.MarkovBlocked.Contains(name);
        }

        public void MarkovBlock(String name)
        {
            if (Markov == null)
            {
                throw new InvalidOperationException("Markov is not enabled");
            }
            Settings.MarkovBlocked.Add(name);
            SaveSettings();
        }

        public void MarkovUnblock(string name)
        {
            if (Markov == null)
            {
                throw new InvalidOperationException("Markov is not enabled");
            }
            Settings.MarkovBlocked.Remove(name);
            SaveSettings();
        }

        public void OnCheck()
        {
            while (_listen)
            {
                Dictionary<String, List<Entry>> commits = new Dictionary<string, List<Entry>>();
                if (Settings.Repos.Count > 0)
                {
                    Console.WriteLine("OnCheck()");
                    foreach (String repo in Settings.Repos)
                    {
                        String feedUrl = @"https://github.com/" + repo + @"/commits/master.atom";
                        var webRequest = WebRequest.Create(feedUrl);
                        var responseStream = webRequest.GetResponse().GetResponseStream();
                        if (responseStream == null)
                        {
                            continue;
                        }
                        var streamReader = new StreamReader(responseStream);
                        var serializer = new XmlSerializer(typeof(Feed));
                        Feed currentFeed = (Feed)serializer.Deserialize(streamReader);

                        Feed lastFeed;
                        try
                        {
                            lastFeed = _feeds[repo];
                        }
                        catch (KeyNotFoundException)
                        {
                            lastFeed = null;
                        }

                        if (lastFeed != null)
                        {
                            DateTime currentFeedTime = ToDateTime(currentFeed.Updated);
                            DateTime lastFeedTime = ToDateTime(lastFeed.Updated);

                            if (currentFeedTime <= lastFeedTime)
                            {
                                continue;
                            }

                            List<Entry> newCommits = (from commit in currentFeed.Entries 
                                                        let commitTime = ToDateTime(commit.Updated) 
                                                        where commitTime > lastFeedTime 
                                                        select commit).ToList();

                            
                            commits.Add(repo, newCommits);
                            _feeds.Remove(repo);
                            _feeds.Add(repo, currentFeed);
                            continue;
                        }

                        _feeds.Add(repo, currentFeed);
                    }

                    if (commits.Keys.Count > 0)
                    {
                        AnnounceCommits(commits);
                    }
                }

                Thread.Sleep(1000*60);
            }
        }

        private void AnnounceCommits(Dictionary<String, List<Entry>> commits)
        {
            Dictionary<string, List<string>> commitChannels = Settings.CommitChannels;
            foreach (String serverIdentifier in commitChannels.Keys)
            {
                Server server = Server.GetServer(serverIdentifier);
                if (server == null)
                {
                    Console.WriteLine("[Sandra] Warning: Unknown server id:" + serverIdentifier);
                    continue;
                }

                foreach (String channelName in commitChannels[serverIdentifier])
                {
                    if (!server.Channels.ContainsKey(channelName))
                    {
                        Console.WriteLine("[Sandra] Warning: Invalid channel: " + channelName + " @ server:" + serverIdentifier);
                        continue;
                    }
                    
                    foreach (String repo in commits.Keys)
                    {
                        String org = repo.Split('/')[0];
                        String repoName = repo.Split('/')[1];
                        EntChannel channel = new EntChannel(server, channelName);
                        channel.SendMessage(
                            "Neue Commits (" + ColorCode.CYAN + commits[repo].Count + ColorCode.NORMAL + ") in "
                            + ColorCode.PURPLE + org + ColorCode.NORMAL + "/" + ColorCode.MAGENTA + repoName + ColorCode.NORMAL + ": ");
                        foreach (Entry commit in commits[repo])
                        {
                            String commitId = commit.Id.Split('/')[1].Substring(0, 6);
                            channel.SendMessage("    " + ColorCode.LIGHT_GRAY + "[" + ColorCode.DARK_GREEN + "#" + ColorCode.GREEN + commitId + ColorCode.LIGHT_GRAY + "] " + ColorCode.NORMAL + ColorCode.BOLD + commit.Author.Name + ColorCode.NORMAL + ": " + commit.Title.Trim());
                            channel.SendMessage("        " + MakeGitIoLink(commit.Link.Href));
                        }
                    }

                    commits.Clear();
                }
            }
        }

        public String MakeGitIoLink(String link)
        {
            try
            {
                HttpWebRequest request = WebRequest.CreateHttp(@"http://git.io");
                request.Method = "POST";
                String data = "url=" + link;
                byte[] bytes = Encoding.UTF8.GetBytes(data);
                Stream str = request.GetRequestStream();
                str.Write(bytes, 0, bytes.Length);
                str.Close();

                HttpWebResponse resp = (HttpWebResponse) request.GetResponse();
                return resp.GetResponseHeader("Location");
            }
            catch (Exception e)
            {
                if (e is NotImplementedException && Program.IsMono())
                {
                    Console.WriteLine(
                        "Warning: Your mono runtime is outdated. Please update to a version newer than 14 march 2014");
                }
                else
                {
                    Console.WriteLine("Couldn't shorten link: " + link + ": " + e);
                }
                return link;
            }
        }

        public static DateTime ToDateTime(String s)
        {
            return DateTime.Parse(s);
        }

        public const String REPO_PATTERN = "([a-zA-z0-9-_.]+\\/[a-zA-Z0-9-_.]+)";
        public bool AddRepo(String repo)
        {
            if (!Regex.IsMatch(repo, REPO_PATTERN)) 
            {
                return false;
            }

            try
            {
                var response = WebRequest.Create(@"https://github.com/" + repo).GetResponse().GetResponseStream();
                if(response == null)
                    throw new Exception();
            }
            catch (Exception)
            {
                return false;
            }

            Settings.Repos.Add(repo);
            return true;
        }

        private string _settingsPath;

        public SandraSettings LoadSettings()
        {
            //DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Settings));


            SandraSettings newSettings;

            if (!File.Exists(_settingsPath))
            {
                newSettings = new SandraSettings
                {
                    CommitChannels = new Dictionary<string, List<string>>(),
                    Repos = new List<string>(),
                    MarkovHost = "localhost",
                    MarkovDb = "markov",
                    MarkovUser = "root",
                    MarkovPassword = ""

                };

                JsonHelper.Serialize<SandraSettings>(newSettings, _settingsPath);
            }
            else
            {
                newSettings = JsonHelper.Deserialize<SandraSettings>(_settingsPath);
            }

            return newSettings;
        }

        public void SaveSettings()
        {
            JsonHelper.Serialize<SandraSettings>(Settings, _settingsPath);
        }

        public bool DeleteRepo(String  repo)
        {
            if (!Settings.Repos.Contains(repo)) return false;
            Settings.Repos.Remove(repo);
            return true;
        }

        public bool AddChannel(string server, string channel)
        {
            Server srv = Server.GetServer(server);
            if (srv == null)
            {
                return false;
            }

            /*
            if (!srv.Channels.ContainsKey(channel))
            {
                return false;
            }
            */


            List<String> channels;
            try
            {
                channels = Settings.CommitChannels[server];
            }
            catch (KeyNotFoundException)
            {
                channels = new List<string>();
            }

            channels.Add(channel);
            if (Settings.CommitChannels.ContainsKey(server))
            {
                Settings.CommitChannels.Remove(server);
            }
            Settings.CommitChannels.Add(server, channels);

            return true;
        }
    }
}
