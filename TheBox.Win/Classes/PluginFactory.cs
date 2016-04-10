using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TheBox.Common;

namespace TheBox.Win.Classes
{
    /// <summary>
    /// Plugin Factory
    /// </summary>
    public class PluginFactory
    {
        #region Singleton

        /// <summary>
        /// The _plugin factory
        /// </summary>
        private static PluginFactory _pluginFactory;

        /// <summary>
        /// Gets the get instance.
        /// </summary>
        public static PluginFactory GetInstance
        {
            get
            {
                if (_pluginFactory == null)
                {
                    _pluginFactory = new PluginFactory();
                    _pluginFactory.Plugins = _pluginFactory.GetPlugins();
                }
                return _pluginFactory;
            }
        }

        #endregion Singleton

        /// <summary>
        /// Prevents a default instance of the <see cref="PluginFactory"/> class from being created.
        /// </summary>
        private PluginFactory()
        {

        }

        /// <summary>
        /// Gets the plugins.
        /// </summary>
        public List<IBoxComponent> Plugins
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the plugins.
        /// </summary>
        /// <returns></returns>
        private List<IBoxComponent> GetPlugins()
        {
            // get or create the plugin directory
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase), "Plugin");
            DirectoryInfo pluginDir = new DirectoryInfo(new Uri(path).LocalPath);
            if (!pluginDir.Exists)
            {
                pluginDir.Create();
            }

            // get dll files
            List<FileInfo> files = pluginDir.GetFiles().Where(m => m.Extension.ToLower() == ".dll").ToList();

            // prepare plugin result
            List<IBoxComponent> result = new List<IBoxComponent>();

            foreach (FileInfo file in files)
            {
                try
                {
                    Type[] types = Assembly.LoadFile(file.FullName).GetTypes();
                    Type iBoxComponent = typeof(IBoxComponent);

                    foreach (Type t in types)
                    {
                        if (t != iBoxComponent && iBoxComponent.IsAssignableFrom(t))
                        {
                            result.Add((IBoxComponent)Activator.CreateInstance(t));
                            ((IBoxComponent)result.Last()).GetUserControl().Focusable = false;

                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return result;
        }
    }
}
