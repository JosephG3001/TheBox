using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TheBox.Games.Settings
{
    [Serializable]
    public class EmulatorSettings
    {
        [XmlArray("EmulatorList")]
        [XmlArrayItem("Emulator")]
        public EmulatorSetting[] EmulatorSettingList;
    }
}
