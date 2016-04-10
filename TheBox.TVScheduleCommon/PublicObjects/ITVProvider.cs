using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheBox.TVScheduleCommon.PublicObjects
{
    /// <summary>
    /// ITVProvider - Implement the [Serializable] attribute on the concrete class.
    /// </summary>
    public interface ITVProvider
    {
        string ServiceId { get; set; }

        /// <summary>
        /// The provider name
        /// </summary>
        string ProviderName { get; set; }

        /// <summary>
        /// The channels
        /// </summary>
        List<IChannel> Channels { get; set; }
    }
}
