using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheBox.TVScheduleCommon.PublicObjects
{
    /// <summary>
    /// IChannel - Implement the [Serializable] attribute on the concrete class.
    /// </summary>
    public interface IChannel
    {
        /// <summary>
        /// The channel number
        /// </summary>
        string ChannelNumber { get; set; }

        /// <summary>
        /// The channel name
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// the list of programmes for the parent channel.
        /// </summary>
        List<IProgramme> Programmes { get; set; }
    }
}
