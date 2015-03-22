using System;
using System.Xml.Serialization;

namespace SandraPlugin.GitHub
{
    [Serializable]
    [XmlRoot("feed")]
    public class Feed
    {
        [XmlArrayItem("id")]
        public String Id { get; set; }

        [XmlArrayItem("link", typeof(Link))]
        public Link[] Links { get; set; }

        [XmlArrayItem("entry", typeof(Entry))]
        public Entry[] Entries { get; set; }

        [XmlElement("title")]
        public String Title { get; set; }

        [XmlElement("updated")]
        public String Updated { get; set; }
    }

    [Serializable]
    public class Entry
    {
        [XmlElement("id")]
        public String Id { get; set; }

        [XmlElement("link", typeof(Link))]
        public Link Link { get; set; }

        [XmlElement("title")]
        public String Title { get; set; }

        [XmlElement("updated")]
        public String Updated { get; set; }

        [XmlElement("author", typeof(Author))]
        public Author Author { get; set; }

        [XmlElement("Content")]
        public String Content { get; set; }
    }

    [Serializable]
    public class Author
    {
        [XmlElement("name")]
        public String Name { get; set; }

        [XmlElement("uri")]
        public String Uri { get; set; }
    }

    [Serializable]
    public class Link
    {
        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlAttribute("rel")]
        public string Rel { get; set; }

        [XmlAttribute("href")]
        public string Href { get; set; }
    }
}