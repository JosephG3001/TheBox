using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Reflection;
using System.IO;

namespace TheBox.Movies.Settings
{
    /// <summary>
    /// MovieSettingsManager
    /// </summary>
    public class MovieSettingsManager
    {
        /// <summary>
        /// The component name (needed for the xml file path)
        /// </summary>
        private string _componentName;

        /// <summary>
        /// Initializes a new instance of the <see cref="MovieSettingsManager"/> class.
        /// </summary>
        public MovieSettingsManager(string componentName)
        {
            this._componentName = componentName;
            this.MovieSettings = LoadSettings();
        }

        /// <summary>
        /// Gets or sets the movie settings.
        /// </summary>
        public MovieSettings MovieSettings
        {
            get; set;
        }

        /// <summary>
        /// The serialiser (static read only for memory leak)
        /// </summary>
        public static readonly XmlSerializer serialiser = new XmlSerializer(typeof(MovieSettings));

        /// <summary>
        /// Loads the settings.
        /// </summary>
        /// <returns></returns>
        public MovieSettings LoadSettings()
        {
            string path = GetXMLPath();
            if (!File.Exists(path))
            {
                return new MovieSettings();
            }

            using (StreamReader reader = new StreamReader(path))
            {
                return (MovieSettings)serialiser.Deserialize(reader);
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
                serialiser.Serialize(writer, MovieSettings);
            }
        }

        /// <summary>
        /// Gets the XML path.
        /// </summary>
        /// <returns></returns>
        private string GetXMLPath()
        {
            string dir = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            string path = dir + "\\" + _componentName + "_Settings.xml";
            return path;
        }
    }
}
