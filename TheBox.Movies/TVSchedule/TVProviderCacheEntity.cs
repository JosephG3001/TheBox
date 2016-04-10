using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheBox.Common.Menu;

namespace TheBox.Movies.TVSchedule
{
    /// <summary>
    /// TVProviderCacheEntity - Represents a result set for a TV provider (eg. Sky)
    /// </summary>
    public class TVProviderCacheEntity
    {
        /// <summary>
        /// Gets or sets the name of the provider.
        /// </summary>
        public string providerName { get; set; }

        /// <summary>
        /// the Last Scan time indicates the time that the last video file to TV programme comparison scan was made.
        /// </summary>
        public DateTime LastScanTime { get; set; }

        /// <summary>
        /// Gets or sets the menu item models.
        /// </summary>
        public List<MenuItemModel> MenuItemModels { get; set; }
    }
}
