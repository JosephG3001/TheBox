using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TheBox.Common;
using TheBox.Common.Models;

namespace TheBox.Games
{
    /// <summary>
    /// Interaction logic for EmulatorSettingModal.xaml
    /// </summary>
    public partial class EmulatorSettingModal : UserControl, IBoxKeyboardControl
    {
        public EmulatorSettingModal()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the key down.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="T:System.Windows.Input.KeyEventArgs" /> instance containing the event data.</param>
        public void HandleKeyDown(object sender, KeyEventArgs e)
        {
            // backspace
            if (e.Key == Key.Back)
            {
                // remove the modal
                ModalModel.GetInstance.ModalUserControl = null;

                // change page buttons back to gold
                PageModel.GetInstance.SelectPageModel();
            }
        }
    }
}
