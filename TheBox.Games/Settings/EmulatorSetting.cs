using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TheBox.Games.Settings
{
    /// <summary>
    /// Emulator Setting
    /// </summary>
    [Serializable]
    public class EmulatorSetting
    {
        [XmlElement("EmulatatedSystemName")]
        public string EmulatatedSystemName;

        [XmlElement("RomPath")]
        public string RomPath;

        [XmlElement("ConsoleImagePath")]
        public string ConsoleImagePath;

        [XmlElement("BootCommand")]
        public string BootCommand;

        [XmlElement("FileExt")]
        public string FileExt;

        [XmlElement("EmulatorPath")]
        public string EmulatorPath;

        [XmlElement("WinKawaks")]
        public bool WinKawaks;
    }
}
