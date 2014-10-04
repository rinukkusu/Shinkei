using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

namespace Shinkei
{
    class PluginContainer
    {
        private string m_Path;
        private CompositionContainer m_Container;

        [ImportMany]
        public IEnumerable<Lazy<IPlugin, IPluginData>> Plugins;

        public PluginContainer(string Path = "./plugins")
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
    }
}
