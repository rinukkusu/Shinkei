using System.Diagnostics;
using System.Reflection;
using Shinkei.API;

namespace Shinkei.IRC
{
    public class ShinkeiPlugin : Plugin
    {
        protected ShinkeiPlugin()
        {
            Metadata = new ShinkeiPluginData();
        }

        private static ShinkeiPlugin _instance;
        internal static ShinkeiPlugin GetInstance()
        {
            return _instance ?? (_instance = new ShinkeiPlugin());
        }
    }

    public class ShinkeiPluginData : IPluginData
    {
        public string Name
        {
            get { return "Shinkei"; }
        }

        public string Version
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version.ToString();  }
        }

        public string Author
        {
            get
            {
                var versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location);
                return versionInfo.CompanyName;
            }
        }

        public string Description
        {
            get { return "Shinkei Core Plugin"; }
        }
    }
}