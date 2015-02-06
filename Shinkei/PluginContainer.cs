using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using Shinkei.API;
using Shinkei.IRC;

namespace Shinkei
{
    public class PluginContainer
    {
        private readonly string _mPath;
        private CompositionContainer _mContainer;

        [ImportMany]
        private IEnumerable<Lazy<Plugin, IPluginData>> _plugins;

        public List<Plugin> Plugins = new List<Plugin>(); 

        private static readonly PluginContainer Instance = new PluginContainer("./plugins");
        public static PluginContainer GetInstance()
        {
            return Instance;
        }

        private PluginContainer(string path = "./plugins")
        {
            _mPath = path;
        }

        public void LoadPlugins()
        {
            if (!Directory.Exists(_mPath))
            {
                Directory.CreateDirectory(_mPath);
            }

            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new DirectoryCatalog(_mPath));
            _mContainer = new CompositionContainer(catalog);

            try
            {
                _mContainer.ComposeParts(this);
            }
            catch (CompositionException compositionException)
            {
                Console.WriteLine(compositionException.ToString());
            }

            Plugins.Add(ShinkeiPlugin.GetInstance());

            foreach (Lazy<Plugin, IPluginData> lazy in _plugins)
            {
                lazy.Value.Metadata = lazy.Metadata;
                Plugins.Add(lazy.Value);
            }

            DebugListener.Init();
        }
    }
}
