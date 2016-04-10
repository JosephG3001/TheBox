using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TheBox.Games.Settings
{
    /// <summary>
    /// GameSettingsManager 
    /// </summary>
    public class GameSettingsManager
    {
        /// <summary>
        /// The component name (needed for the xml file path)
        /// </summary>
        private string _componentName;

        /// <summary>
        /// The serialiser (static readonly prevents memory leak)
        /// </summary>
        public static readonly XmlSerializer serialiser = new XmlSerializer(typeof(EmulatorSettings));

        /// <summary>
        /// Gets or sets the emulator settings.
        /// </summary>
        public EmulatorSettings EmulatorSettings
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameSettingsManager"/> class.
        /// </summary>
        public GameSettingsManager(string componentName)
        {
            this._componentName = componentName;
            this.EmulatorSettings = LoadSettings();
        }

        /// <summary>
        /// Loads the settings.
        /// </summary>
        /// <param name="componentName">Name of the component.</param>
        /// <returns></returns>
        public EmulatorSettings LoadSettings()
        {
            string path = GetXMLPath();
            if (!File.Exists(path))
            {
                return new EmulatorSettings();
            }

            using (StreamReader reader = new StreamReader(path))
            {
                return (EmulatorSettings)serialiser.Deserialize(reader);
            }
        }

        /// <summary>
        /// Saves the settings.
        /// </summary>
        public void SaveSettings()
        {
            string path = GetXMLPath();
            using (TextWriter writer = new StreamWriter(path))
            {
                serialiser.Serialize(writer, EmulatorSettings);
            }
        }

        /// <summary>
        /// Gets the XML path.
        /// </summary>
        /// <returns></returns>
        private string GetXMLPath()
        {
            string dir = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            string path = dir + "\\" + _componentName + "_setting.xml";
            return path;
        }
    }
}
