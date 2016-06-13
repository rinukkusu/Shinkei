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
            public string OAuthToken;

            [DataMember]
            public string OAuthSecret;

            [DataMember]
            public Dictionary<string, List<string>> Channels;

			[DataMember]
			public string Highlight;
        }

        [DataMember]
        public List<TwitterAccount> Accounts;

        [DataMember] 
        public List<String> Follows;
        
        [DataMember]
        public string ConsumerKey;

        [DataMember]
        public string ConsumerSecret;
    }
}
