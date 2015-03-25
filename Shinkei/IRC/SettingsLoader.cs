using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;

namespace Shinkei.IRC
{
    public class SettingsLoader
    {
        [DataContract]
        public class Settings
        {
            [DataContract]
            public class ServerSettings
            {
                [DataContract]
                public class ChannelSettings
                {
                    [DataMember]
                    public string Channel;
                    [DataMember]
                    public string Key;
                }

                [DataMember]
                public string Nickname;
                [DataMember]
                public string Username;
                [DataMember]
                public string Realname;

                [DataMember]
                public string Hostname;
                [DataMember]
                public int Port;

                [DataMember]
                public String Identifier;

                [DataMember]
                public List<ChannelSettings> Channels;
            }

            [DataMember]
            public char CommandCharacter = '+';

            [DataMember]
            public List<ServerSettings> Servers;
        }

        readonly string _mPath;
        public Settings MSettings;
        public List<Server> Servers;

        private static readonly SettingsLoader Instance = new SettingsLoader("config.json");
        public static SettingsLoader GetInstance()
        {
            return Instance;
        }

        private SettingsLoader(string path)
        {
            _mPath = path;

            if (!File.Exists(_mPath))
            {
                GenerateSampleConfig();
            }
        }

        public void GenerateSampleConfig()
        {
            if (!File.Exists(_mPath))
            {
                Settings newSettings = new Settings();
                newSettings.Servers = new List<Settings.ServerSettings>();
                
                Settings.ServerSettings newServer = new Settings.ServerSettings();
                newServer.Hostname = "irc.lolnein.de";
                newServer.Port = 6667;
                newServer.Nickname = "Shinkei_" + (new Random()).Next(1000, 9999);
                newServer.Username = "shinkei"; ;
                newServer.Realname = "Shinkei Bot"; ;
                newServer.Identifier = "lolnein";
                newServer.Channels = new List<Settings.ServerSettings.ChannelSettings>();

                Settings.ServerSettings.ChannelSettings newChannel = new Settings.ServerSettings.ChannelSettings();
                newChannel.Channel = "#shinkei";
                newChannel.Key = "";

                newServer.Channels.Add(newChannel);
                newSettings.Servers.Add(newServer);

                JsonHelper.Serialize<Settings>(newSettings, _mPath);
            }
        }

        public void Load()
        {
            MSettings = JsonHelper.Deserialize<Settings>(_mPath);
        }

        public void Save()
        {
            JsonHelper.Serialize<Settings>(MSettings, _mPath);
        }

        public void Reload()
        {
            Load();
            EnforceSettings();
        }

        public void EnforceSettings()
        {
            if (Servers != null)
            {
                foreach (Server s in Servers)
                {
                    s.Disconnect();
                }

                Servers.Clear();
            }
            else
            {
                Servers = new List<Server>();
            }

            int i = 0;
            foreach (Settings.ServerSettings servSettings in MSettings.Servers)
            {
                if (servSettings.Identifier == null)
                {
                    servSettings.Identifier = "server_" + i;
                }
                Server newServer = new Server(servSettings.Hostname, 
                                              servSettings.Port,
                                              servSettings.Identifier,
                                              servSettings.Nickname, 
                                              servSettings.Username, 
                                              servSettings.Realname);

                newServer.LocalSettings = servSettings;

                foreach (Settings.ServerSettings.ChannelSettings chanSettings in servSettings.Channels)
                {
                    Channel newChannel = new Channel(newServer, chanSettings.Channel, chanSettings.Key);

                    newServer.Channels.Add(chanSettings.Channel, newChannel);
                }

                newServer.Connect();
                Servers.Add(newServer);
                i++;
            }
        }
    }
}
