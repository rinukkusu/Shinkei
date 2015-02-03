using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace Shinkei.IRC
{
    public class SettingsLoader
    {
        [DataContractAttribute]
        public class Settings
        {
            [DataContractAttribute]
            public class ServerSettings
            {
                [DataContractAttribute]
                public class ChannelSettings
                {
                    [DataMemberAttribute]
                    public string Channel;
                    [DataMemberAttribute]
                    public string Key;
                }

                [DataMemberAttribute]
                public string Nickname;
                [DataMemberAttribute]
                public string Username;
                [DataMemberAttribute]
                public string Realname;

                [DataMemberAttribute]
                public string Hostname;
                [DataMemberAttribute]
                public int Port;

                [DataMemberAttribute]
                public List<ChannelSettings> Channels;
            }

            [DataMemberAttribute]
            public string Nickname;
            [DataMemberAttribute]
            public string Username;
            [DataMemberAttribute]
            public string Realname;

            [DataMemberAttribute]
            public char CommandCharacter = '+';

            [DataMemberAttribute]
            public List<ServerSettings> Servers;
        }

        string _mPath;
        public Settings MSettings;
        public List<Server> MServers;

        private static SettingsLoader _instance = new SettingsLoader("config.json");
        public static SettingsLoader GetInstance()
        {
            return _instance;
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
                newSettings.Nickname = "Shinkei_" + (new Random()).Next(1000,9999);
                newSettings.Username = "shinkei";
                newSettings.Realname = "Shinkei Bot";
                newSettings.Servers = new List<Settings.ServerSettings>();
                
                Settings.ServerSettings newServer = new Settings.ServerSettings();
                newServer.Hostname = "irc.lolnein.de";
                newServer.Port = 6667;
                newServer.Nickname = newSettings.Nickname;
                newServer.Username = newSettings.Username;
                newServer.Realname = newSettings.Realname;
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
            if (MServers != null)
            {
                foreach (Server s in MServers)
                {
                    s.Disconnect();
                }

                MServers.Clear();
            }

            foreach (Settings.ServerSettings servSettings in MSettings.Servers)
            {
                Server newServer = new Server(servSettings.Hostname, 
                                              servSettings.Port, 
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
            }
        }
    }
}
