using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;

namespace Shinkei
{
    public class PluginContainer
    {
        private string _mPath;
        private CompositionContainer _mContainer;

        [ImportMany]
        public IEnumerable<Lazy<IPlugin, IPluginData>> Plugins;

        private static PluginContainer _instance = new PluginContainer("./plugins");
        public static PluginContainer GetInstance()
        {
            return _instance;
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
                this._mContainer.ComposeParts(this);
            }
            catch (CompositionException compositionException)
            {
                Console.WriteLine(compositionException.ToString());
            }
        }

        public List<CommandDescription> GetAllCommands()
        {
            List<CommandDescription> allCommands = new List<CommandDescription>();

            foreach (Lazy<IPlugin, IPluginData> plugin in this.Plugins)
            {
                allCommands.AddRange(plugin.Value.GetCommands());
            }

            allCommands.Sort(new CommandComparer());

            return allCommands;
        }
    }
}
