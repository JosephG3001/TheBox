using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using TheBox.Common.Menu;

namespace TheBox.Common
{
    /// <summary>
    /// IBox 3rd party components should implment this interface
    /// </summary>
    public interface IBoxComponent
    {
        /// <summary>
        /// Gets the name of the component.
        /// </summary>
        string ComponentName { get; }

        /// <summary>
        /// Gets the appbar_icon.
        /// </summary>
        /// <remarks>
        /// See http://modernuiicons.com/ for icons
        /// </remarks>
        string appbar_icon { get; }

        /// <summary>
        /// Gets the activate command.  The activate command will be a delegate that should run when the component is activated.
        /// </summary>
        void ActivateComponent();

        /// <summary>
        /// Gets the underlying user control.
        /// </summary>
        UserControl GetUserControl();

        /// <summary>
        /// Gets the component settings.
        /// </summary>
        //Dictionary<string, List<KeyValuePair<string, string>>> ComponentSettings { get; }

        /// <summary>
        /// Navigates to settings.
        /// </summary>
        /// <returns></returns>
        void NavigateToSettings();


        string BackgroundImageUri { get; }
    }
}
