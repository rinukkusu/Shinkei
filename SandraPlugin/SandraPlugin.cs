using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
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

namespace SandraPlugin
{
    [Export(typeof(Plugin))]
    [ExportMetadata("Name", "Sandra")]
    [ExportMetadata("Version", "0.1")]
    [ExportMetadata("Author", "Trojaner")]
    [ExportMetadata("Description", "A bot with some useful functions.")]
    public class SandraPlugin: Plugin, IListener
    {
        private Dictionary<string, Feed> _feeds;
        private List<String> _repos;

        public List<String> Repos
        {
            get { return _repos; }
        } 

        private Dictionary<String, List<String>> _commitChannels;

        public Dictionary<String, List<String>> CommitChannels
        {
            get { return _commitChannels; }
        }
        private bool _listen;

        [DataContract]
        public class Settings
        {
            [DataMember]
            public Dictionary<String, List<String>> CommitChannels;

            [DataMember]
            public List<String> Repos;
        }
    

        public override void OnEnable()
        {
            _settingsPath = Path.Combine(DataDirectory, "SandraPlugin.json");

            LoadSettings();
            _feeds = new Dictionary<string, Feed>();
            _listen = true;

            EventManager.GetInstance().RegisterEvents(this, this);
            CommandHandler.GetInstance().RegisterCommand(new Command("addrepo", "addrepo <username>/<repo>",
                 "Adds a github reporsitory and announces for new commits.", new AddRepoCommandExecutor(this),  CommandPermission.WHITELIST), this);
            CommandHandler.GetInstance().RegisterCommand(new Command("calc", "calc <query>", "Executes a Wolfram Alpha query", new CalcCommandExecutor()), this);
            CommandHandler.GetInstance().RegisterCommand(new Command("listrepos", "listrepo",
     "Shows all saved repos.", new ListReposCommandExecutor(this)), this);
            CommandHandler.GetInstance().RegisterCommand(new Command("m", "m <keyword>", "Outputs a markov-generated sentence", new MarkovCommandExecutor()), this);
            CommandHandler.GetInstance().RegisterCommand(new Command("removerepo", "removerepo <username>/<repo>",
                             "Removes a repo which was added previously.", new RemoveRepoCommandExecutor(this), CommandPermission.WHITELIST), this);
            CommandHandler.GetInstance().RegisterCommand(new Command("addcommitchannel", "addcommitchannel <server-identifier> #<channel>",
                     "Adds a channel for announcing new commits.", new AddCommitChannelCommandExecutor(this), CommandPermission.OP), this);
            Thread thread = new Thread(OnCheck); 
            thread.Start();
        }

        public void OnCheck()
        {
            while (_listen)
            {
                Console.WriteLine("OnCheck");
                Dictionary<String, List<Entry>> commits = new Dictionary<string, List<Entry>>();
                if (_repos.Count > 0)
                {
                    foreach (String repo in _repos)
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

                            List<Entry> newCommits = (from commit in lastFeed.Entries 
                                                        let commitTime = ToDateTime(commit.Updated) 
                                                        where commitTime >= lastFeedTime 
                                                        select commit).ToList();

                            
                            commits.Add(repo, newCommits);
                            _feeds.Remove(repo);
                            _feeds.Add(repo, lastFeed);
                            continue;
                        }

                        Console.WriteLine(repo + ": no new commits");

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
            foreach (String serverIdentifier in _commitChannels.Keys)
            {
                Server server = Server.GetServer(serverIdentifier);
                if (server == null)
                {
                    Console.WriteLine("[Sandra] Warning: Unknown server id:" + serverIdentifier);
                    continue;
                }

                foreach (String channelName in _commitChannels[serverIdentifier])
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
                            "Neue Commits (" + ColorCode.CYAN + commits.Count + ColorCode.NORMAL + ") in "
                            + ColorCode.PURPLE + org + ColorCode.NORMAL + "/" + ColorCode.MAGENTA + repoName + ColorCode.NORMAL + ": ");
                        foreach (Entry commit in commits[repo])
                        {
                            String commitId = commit.Id.Split('/')[1].Substring(0, 6);
                            channel.SendMessage("    " + ColorCode.LIGHT_GRAY + "[" + ColorCode.DARK_GREEN + "#" + ColorCode.GREEN + commitId + ColorCode.LIGHT_GRAY + "] " + ColorCode.NORMAL + ColorCode.BOLD + commit.Author.Name + ColorCode.NORMAL + ": " + commit.Title.Trim());
                            channel.SendMessage("        " + MakeGitIoLink(commit.Link.Href));
                        }
                    }
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
            catch (Exception)
            {
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
                Console.WriteLine("Failed: Pattern mismatch");
                return false;
            }

            HttpWebResponse response;
            try
            {
                HttpWebRequest request = WebRequest.CreateHttp(@"https://github.com/" + repo);
                response = (HttpWebResponse) request.GetResponse();
            }
            catch (WebException e)
            {
                Console.WriteLine("Failed: " + e);
                response = (HttpWebResponse) e.Response;
            }

            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Accepted && response.StatusCode != HttpStatusCode.Created)
            {
                return false;
            }

            _repos.Add(repo);
            return true;
        }

        private string _settingsPath;
        public void LoadSettings()
        {
            //DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Settings));


            Settings newSettings;

            if (!File.Exists(_settingsPath))
            {
                newSettings = new Settings
                {
                    CommitChannels = new Dictionary<string, List<string>>(),
                    Repos = new List<string>()
                };

                JsonHelper.Serialize<Settings>(newSettings, _settingsPath);
            }
            else
            {
                newSettings = JsonHelper.Deserialize<Settings>(_settingsPath);
            }

            _repos = newSettings.Repos;
            _commitChannels = newSettings.CommitChannels;
        }

        public void SaveSettings()
        {
            Settings settings = new Settings
            {
                CommitChannels = _commitChannels,
                Repos = _repos
            };

            JsonHelper.Serialize<Settings>(settings, _settingsPath);
        }

        public bool DeleteRepo(String  repo)
        {
            if (!_repos.Contains(repo)) return false;
            _repos.Remove(repo);
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
                channels = _commitChannels[server];
            }
            catch (KeyNotFoundException)
            {
                channels = new List<string>();
            }

            channels.Add(channel);
            if (_commitChannels.ContainsKey(server))
            {
                _commitChannels.Remove(server);
            }
            _commitChannels.Add(server, channels);

            return true;
        }
    }
}
