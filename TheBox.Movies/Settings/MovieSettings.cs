using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TheBox.Movies.Settings
{
    [Serializable]
    public class MovieSettings
    {
        [XmlElement("RootMediaPath")]
        public string RootMediaPath;
    }
}
