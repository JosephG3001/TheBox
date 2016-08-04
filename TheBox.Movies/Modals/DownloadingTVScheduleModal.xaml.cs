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

namespace TheBox.Movies
{
    /// <summary>
    /// Interaction logic for DownloadingTVScheduleModal.xaml
    /// </summary>
    public partial class DownloadingTVScheduleModal : UserControl, IBoxKeyboardControl
    {
        /// <summary>
        /// Gets or sets a value indicating whether [allow keystrokes].
        /// </summary>
        private bool AllowKeystrokes { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DownloadingTVScheduleModal"/> class.
        /// </summary>
        public DownloadingTVScheduleModal()
        {
            InitializeComponent();
            AllowKeystrokes = false;
        }

        /// <summary>
        /// Sets the fail.
        /// </summary>
        /// <param name="error">The error.</param>
        public void SetFail(string error)
        {
            progressRing1.Visibility = Visibility.Collapsed;
            tbkPleaseWait.Text = error;
            AllowKeystrokes = true;
        }

        /// <summary>
        /// Handles the key down.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="T:System.Windows.Input.KeyEventArgs" /> instance containing the event data.</param>
        public void HandleKeyDown(object sender, KeyEventArgs e)
        {
            if (!AllowKeystrokes)
            {
                return;
            }

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
