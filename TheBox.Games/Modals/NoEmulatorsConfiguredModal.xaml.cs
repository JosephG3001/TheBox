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

namespace TheBox.Games.Modals
{
    /// <summary>
    /// Interaction logic for NoEmulatorsConfiguredModal.xaml
    /// </summary>
    public partial class NoEmulatorsConfiguredModal : UserControl, IBoxKeyboardControl
    {
        public NoEmulatorsConfiguredModal()
        {
            InitializeComponent();
        }

        public void HandleKeyDown(object sender, KeyEventArgs e)
        {
            // backspace & enter
            if (e.Key == Key.Back || e.Key == Key.Enter)
            {
                // remove the modal
                ModalModel.GetInstance.ModalUserControl = null;

                PageModel.GetInstance.NavigateBackwards();
            }
        }
    }
}
