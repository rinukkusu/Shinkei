using System;
using System.Xml.Serialization;

namespace SandraPlugin.GitHub
{
    [Serializable]
    [XmlTypeAttribute(AnonymousType = true)]
    [XmlRootAttribute("feed", Namespace="http://www.w3.org/2005/Atom")]
    public class Feed
    {
        [XmlElement("id")]
        public string Id { get; set; }

        [XmlElementAttribute("link")]
        //[XmlArrayItem("link", typeof(Link))]
        public Link[] Links { get; set; }

        [XmlElement("title")]
        public string Title { get; set; }

        [XmlElement("updated")]
        public string Updated { get; set; }

        [XmlElementAttribute("entry")]
        //[XmlArrayItem("entry", typeof(Entry))]
        public Entry[] Entries { get; set; }
    }

    [Serializable]
    [XmlTypeAttribute(AnonymousType = true)]
    public class Entry
    {
        [XmlElement("id")]
        public string Id { get; set; }

        [XmlElement("link", typeof(Link), IsNullable = true)]
        public Link Link { get; set; }

        [XmlElement("title")]
        public string Title { get; set; }

        [XmlElement("updated")]
        public string Updated { get; set; }

        [XmlElement("author", typeof(Author))]
        public Author Author { get; set; }

        [XmlElement("content")]
        public string Content { get; set; }
    }

    [Serializable]
    [XmlTypeAttribute(AnonymousType = true)]
    public class Author
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("uri")]
        public string Uri { get; set; }
    }

    [Serializable]
    [XmlTypeAttribute(AnonymousType = true)]
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