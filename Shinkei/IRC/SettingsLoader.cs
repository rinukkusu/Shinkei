using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
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

        string m_Path;
        public Settings m_Settings;
        public List<Server> m_Servers;

        private static SettingsLoader Instance = new SettingsLoader("config.json");
        public static SettingsLoader GetInstance()
        {
            return Instance;
        }

        private SettingsLoader(string _path)
        {
            m_Path = _path;

            if (!File.Exists(m_Path))
            {
                GenerateSampleConfig();
            }
        }

        public void GenerateSampleConfig()
        {
            if (!File.Exists(m_Path))
            {
                Settings newSettings = new Settings();
                newSettings.Nickname = "Shinkei";
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
                newChannel.Channel = "#dev";
                newChannel.Key = "";

                newServer.Channels.Add(newChannel);
                newSettings.Servers.Add(newServer);

                FileStream newFile = File.Create(m_Path);

                DataContractJsonSerializer Serializer = new DataContractJsonSerializer(typeof(Settings));
                Serializer.WriteObject(newFile, newSettings);
                newFile.Close();
            }
        }

        public void Load()
        {
            DataContractJsonSerializer Serializer = new DataContractJsonSerializer(typeof(Settings));

            FileStream SettingsFile = File.Open(m_Path, FileMode.Open);
            m_Settings = (Settings) Serializer.ReadObject(SettingsFile);
            SettingsFile.Close();
        }

        public void EnforceSettings()
        {
            if (m_Servers != null)
            {
                foreach (Server S in m_Servers)
                {
                    S.Disconnect();
                }

                m_Servers.Clear();
            }

            foreach (Settings.ServerSettings ServSettings in m_Settings.Servers)
            {
                Server newServer = new Server(ServSettings.Hostname, 
                                              ServSettings.Port, 
                                              ServSettings.Nickname, 
                                              ServSettings.Username, 
                                              ServSettings.Realname);

                newServer.localSettings = ServSettings;

                foreach (Settings.ServerSettings.ChannelSettings ChanSettings in ServSettings.Channels)
                {
                    Channel newChannel = new Channel(newServer, ChanSettings.Channel, ChanSettings.Key);

                    newServer.Channels.Add(ChanSettings.Channel, newChannel);
                }

                newServer.Connect();
            }
        }
    }
}
