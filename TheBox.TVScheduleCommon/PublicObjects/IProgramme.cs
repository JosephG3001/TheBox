using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheBox.TVScheduleCommon.PublicObjects
{
    /// <summary>
    /// IProgramme - Implement the [Serializable] attribute on the concrete class.
    /// </summary>
    public interface IProgramme
    {
        /// <summary>
        /// the name of the program.
        /// </summary>
        string ProgramName { get; set; }

        /// <summary>
        /// the name of the channel.
        /// </summary>
        string ChannelName { get; set; }

        /// <summary>
        /// the channel number.
        /// </summary>
        string ChannelNumber { get; set; }

        /// <summary>
        /// the showing time.
        /// </summary>
        string ShowingTime { get; set; }

        /// <summary>
        /// The programme name parts should contain a lower case array of words taken from the ProgrammeName property. 
        /// All non alpha numeric characters should be removed and the programmeName should be Split by [space], [comma], [hyphen] and [period].
        /// Remove duplicate and blank array members.
        /// 
        /// Alternatively, leave this property as null and the main application will do the work for you on the fly (will affect performance)
        /// </summary>
        string[] ProgrammeNameParts { get; set; }
    }
}
