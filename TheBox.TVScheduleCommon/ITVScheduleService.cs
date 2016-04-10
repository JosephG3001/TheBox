using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheBox.TVScheduleCommon.PublicObjects;

namespace TheBox.TVScheduleCommon
{
    /// <summary>
    /// ITVScheduleService
    /// </summary>
    public interface ITVScheduleService
    {
        /// <summary>
        /// Gets the schedule - Implement the models of this library to use the TV Schedule movie/programme matching.
        /// </summary>
        /// <returns></returns>
        ITVSchedule GetSchedule();

        /// <summary>
        /// the tv schedule plugin name.  
        /// Your plugin will become selectable within TheBox application.  
        /// TheBox will show this property in a dropdownlist.
        /// </summary>
        string TVSchedulePluginName { get; }
    }
}
