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

namespace goo.gl_ShortenerPlugin
{
    [Export(typeof(Plugin))]
    [ExportMetadata("Name", "goo.gl Shortener")]
    [ExportMetadata("Version", "0.1")]
    [ExportMetadata("Author", "rinukkusu")]
    [ExportMetadata("Description", "Shortens links via the goo.gl API and displays the title of the page.")]
    public class goo_gl_ShortenerPlugin : Plugin, IListener
    {
        [DataContract]
        public class goo_gl_Settings
        {
            [DataMember]
            public string ApiKey = "";
        }

        goo_gl_Settings _settings;
        public goo_gl_Settings Settings
        {
            get { return _settings; }
        }

        public override void OnEnable()
        {
            _settings = LoadSettings();

            EventManager.GetInstance().RegisterEvents(this, this);
        }

        [Shinkei.IRC.Events.EventHandler]
        public void OnIrcMessage(IrcMessageEvent evnt)
        {
            if (evnt.Recipient.GetType() == typeof(EntChannel))
            {
                Regex reg_youtube = new Regex(@"youtu.*v=([A-Za-z0-9_-]+)");
                if (!reg_youtube.IsMatch(evnt.Text))
                {
                    if (evnt.Text.Contains("http") || evnt.Text.Contains("www"))
                    {
                        Regex reg_link = new Regex(@"((https?:\/\/|www\.|https?:\/\/www\.)[a-z0-9\.:].*?(?=\s))");
                        if (reg_link.IsMatch(evnt.Text) || !evnt.Text.Contains(" "))
                        {

                            if (String.IsNullOrWhiteSpace(_settings.ApiKey))
                            {
                                throw new Exception("Please set the ApiKey for goo.gl Shortener.");
                            }

                            try
                            {
                                var matches = reg_link.Matches(evnt.Text);
                                string link = reg_link.IsMatch(evnt.Text) ? matches[0].Groups[1].Captures[0].Value : evnt.Text;
                                if (!link.StartsWith("http"))
                                {
                                    link = "http://" + link;
                                }

                                string shortUrl = GetShortenedLink(link);
                                string title = GetLinkTitle(link);

                                if (!String.IsNullOrWhiteSpace(shortUrl))
                                {
                                    ServerEntity answerRcpt;
                                    answerRcpt = (ServerEntity)evnt.Recipient;

                                    string returnString = String.Format(ColorCode.BOLD + "{0}" + ColorCode.NORMAL + " - {1}", shortUrl, title);
                                    answerRcpt.SendMessage(returnString);
                                }
                            }
                            catch {}
                        }
                    }
                }
            }
        }

        [DataContract]
        public class goo_gl_Request
        {
            [DataMember]
            public string longUrl { get; set; }
        }

        [DataContract]
        public class goo_gl_Response
        {
            [DataMember]
            public string kind { get; set; }
            [DataMember]
            public string id { get; set; }
            [DataMember]
            public string longUrl { get; set; }
        }

        public string GetShortenedLink(string longUrl)
        {
            string shortUrl = default(string);

            try
            {
                string goo_gl_url = String.Format("https://www.googleapis.com/urlshortener/v1/url?key={0}", _settings.ApiKey);
                HttpWebRequest wRequest = (HttpWebRequest)WebRequest.Create(goo_gl_url);

                wRequest.Method = "POST";
                wRequest.ContentType = "application/json";

                goo_gl_Request req = new goo_gl_Request();
                req.longUrl = longUrl;

                string strData = JsonHelper.SerializeToString<goo_gl_Request>(req);
                byte[] bData = System.Text.UTF8Encoding.UTF8.GetBytes(strData);

                Stream requestStream = wRequest.GetRequestStream();
                requestStream.Write(bData, 0, bData.Count());
                requestStream.Flush();

                HttpWebResponse webResponse = (HttpWebResponse) wRequest.GetResponse();
                StreamReader reader = new StreamReader(webResponse.GetResponseStream(), Encoding.UTF8);
                string strResponse = reader.ReadToEnd();
                reader.Close();

                goo_gl_Response resp = JsonHelper.DeserializeFromString<goo_gl_Response>(strResponse);

                shortUrl = resp == null ? default(string) : resp.id;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return shortUrl;
        }

        public string GetLinkTitle(string url)
        {
            string title = default(string);

            try
            {
                WebClient dlClient = new WebClient();
                byte[] abPage = dlClient.DownloadData(url);
                string strPage = System.Text.Encoding.UTF8.GetString(abPage);

                string rPattern = @"<title>(.*)<\/title>";
                MatchCollection myMatches = Regex.Matches(strPage, rPattern, RegexOptions.IgnoreCase);

                title = myMatches[0].Groups[1].Captures[0].Value;
            }
            catch { }

            return WebUtility.HtmlDecode(title);
        }

        public goo_gl_Settings LoadSettings()
        {
            string path = Path.Combine(DataDirectory, "goo.gl_ShortenerPlugin.json");
            goo_gl_Settings newSettings;

            if (!File.Exists(path))
            {
                newSettings = new goo_gl_Settings();

                JsonHelper.Serialize<goo_gl_Settings>(newSettings, path);
            }
            else
            {
                newSettings = JsonHelper.Deserialize<goo_gl_Settings>(path);
            }

            return newSettings;
        }
    }
}
