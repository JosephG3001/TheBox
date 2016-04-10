using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheBox.TVScheduleCommon.PublicObjects
{
    /// <summary>
    /// ITVSchedule - Implement the [Serializable] attribute on the concrete class.
    /// </summary>
    public interface ITVSchedule
    {
        /// <summary>
        /// The schedule date
        /// </summary>
        DateTime ScheduleDate { get; set; }

        /// <summary>
        /// The providers
        /// </summary>
        List<ITVProvider> Providers { get; set; }
    }
}
