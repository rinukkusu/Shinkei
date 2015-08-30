using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace TwitterPlugin
{
    [DataContract]
    public class TwitterSettings
    {
        [DataContract]
        public class TwitterAccount
        {
            [DataMember]
            public string AuthToken;

            [DataMember]
            public string AuthSecret;

            [DataMember]
            public Dictionary<string, List<string>> Channels;
        }

        [DataMember]
        public List<TwitterAccount> Accounts;

        [DataMember]
        public string ConsumerKey;

        [DataMember]
        public string ConsumerSecret;
    }
}
