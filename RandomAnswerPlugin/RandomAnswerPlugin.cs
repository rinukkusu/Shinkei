using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;
using Shinkei;
using Shinkei.API;
using Shinkei.API.Commands;
using Shinkei.API.Events;
using Shinkei.IRC;
using Shinkei.IRC.Commands;
using Shinkei.IRC.Entities;
using Shinkei.IRC.Events;
using System.Text.RegularExpressions;
using Shinkei.API.Entities;
using System.Net;
using System.IO;

namespace RandomAnswerPlugin
{
    [Export(typeof(Plugin))]
    [ExportMetadata("Name", "Random Answers")]
    [ExportMetadata("Version", "0.1")]
    [ExportMetadata("Author", "rinukkusu")]
    [ExportMetadata("Description", "Collects answers and replies, if someone asks a given phrase.")]
    public class RandomAnswerPlugin : Plugin, IListener
    {
        [DataContract]
        public class RandomAnswerSettings
        {
            [DataMember]
            public string SaveTrigger = @"([Ww]eil\s.*)";

            [DataMember]
            public string AnswerTrigger = @"[Ww](arum|ieso).*\?";

            [DataMember]
            public List<string> AnswerList = new List<string>();

            public RandomAnswerSettings()
            {
                if (AnswerList.Count == 0)
                {
                    AnswerList.Add("Weil das halt so ist.");
                }
            }
        }

        RandomAnswerSettings _settings;
        public RandomAnswerSettings Settings
        {
            get { return _settings; }
        }

        public override void OnEnable()
        {
            _settings = LoadSettings();

            EventManager.GetInstance().RegisterEvents(this, this);
        }

        private Object settingsLock = new Object();

        [Shinkei.IRC.Events.EventHandler]
        public void OnIrcMessage(IrcMessageEvent evnt)
        {
            if (evnt.Recipient.GetType() == typeof(EntChannel))
            {
                Regex reg_save = new Regex(_settings.SaveTrigger);
                Regex reg_answer = new Regex(_settings.AnswerTrigger);

                if (reg_save.IsMatch(evnt.Text))
                {
                    var matches = reg_save.Matches(evnt.Text);
                    string answer = matches[0].Groups[1].Captures[0].Value;

                    _settings.AnswerList.Add(answer);

                    SaveSettings();
                }
                else if (reg_answer.IsMatch(evnt.Text))
                {
                    Random rnd = new Random(DateTime.Now.Millisecond);

                    string randomAnswer = _settings.AnswerList[rnd.Next(0, _settings.AnswerList.Count)];

                    if (!String.IsNullOrWhiteSpace(randomAnswer))
                    {
                        ServerEntity answerRcpt;
                        answerRcpt = (ServerEntity)evnt.Recipient;
                        answerRcpt.SendMessage(randomAnswer);
                    }
                }
            }
        }

        public RandomAnswerSettings LoadSettings()
        {
            lock (settingsLock)
            {
                string path = Path.Combine(DataDirectory, "RandomAnswerPlugin.json");
                RandomAnswerSettings newSettings;

                if (!File.Exists(path))
                {
                    newSettings = new RandomAnswerSettings();

                    JsonHelper.Serialize<RandomAnswerSettings>(newSettings, path);
                }
                else
                {
                    newSettings = JsonHelper.Deserialize<RandomAnswerSettings>(path);
                }

                return newSettings;
            }
        }

        public void SaveSettings()
        {
            lock (settingsLock)
            {
                string path = Path.Combine(DataDirectory, "RandomAnswerPlugin.json");

                if (_settings == null)
                {
                    _settings = new RandomAnswerSettings();
                }

                JsonHelper.Serialize<RandomAnswerSettings>(_settings, path);
            }
        }
    }
}
