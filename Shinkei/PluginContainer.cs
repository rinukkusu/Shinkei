using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;

namespace Shinkei
{
    public class PluginContainer
    {
        private string m_Path;
        private CompositionContainer m_Container;

        [ImportMany]
        public IEnumerable<Lazy<IPlugin, IPluginData>> Plugins;

        private static PluginContainer Instance = new PluginContainer("./plugins");
        public static PluginContainer GetInstance()
        {
            return Instance;
        }

        private PluginContainer(string Path = "./plugins")
        {
            m_Path = Path;
        }

        public void LoadPlugins()
        {
            if (!Directory.Exists(m_Path))
            {
                Directory.CreateDirectory(m_Path);
            }

            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new DirectoryCatalog(m_Path));
            m_Container = new CompositionContainer(catalog);

            try
            {
                this.m_Container.ComposeParts(this);
            }
            catch (CompositionException compositionException)
            {
                Console.WriteLine(compositionException.ToString());
            }
        }

        public List<CommandDescription> GetAllCommands()
        {
            List<CommandDescription> AllCommands = new List<CommandDescription>();

            foreach (Lazy<IPlugin, IPluginData> Plugin in this.Plugins)
            {
                AllCommands.AddRange(Plugin.Value.GetCommands());
            }

            AllCommands.Sort(new CommandComparer());

            return AllCommands;
        }
    }
}
