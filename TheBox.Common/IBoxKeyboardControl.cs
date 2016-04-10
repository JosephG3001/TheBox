using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TheBox.Common
{
    /// <summary>
    /// IBoxKeyboardControl
    /// 
    /// Controls that handle key strokes should implement this interface
    /// </summary>
    public interface IBoxKeyboardControl
    {
        /// <summary>
        /// Handles the key down.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
        void HandleKeyDown(object sender, KeyEventArgs e);
    }
}
