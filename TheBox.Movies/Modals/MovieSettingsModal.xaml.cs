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
using TheBox.Movies.Models;

namespace TheBox.Movies
{
    /// <summary>
    /// Interaction logic for MovieSettingsModal.xaml
    /// </summary>
    public partial class MovieSettingsModal : UserControl, IBoxKeyboardControl
    {
        public MovieSettingsModal()
        {
            InitializeComponent();

            // manually bind...
            txtMediaRoot.Text = MovieControlModel.GetInstance.MovieSettingsManager.MovieSettings.RootMediaPath;
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
                // emulate the cancel button click (hide modal and go back to menu)
                btnCancel_Click(this, new RoutedEventArgs());
            }

            //// enter
            //if (e.Key == Key.Enter)
            //{
            //    // emulate the save button click
            //    btnSave_Click(this, new RoutedEventArgs());
            //}
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // update the model and save
            MovieControlModel.GetInstance.MovieSettingsManager.MovieSettings.RootMediaPath = txtMediaRoot.Text.SafeTrim();
            MovieControlModel.GetInstance.MovieSettingsManager.SaveSettings();

            // remove the modal
            ModalModel.GetInstance.ModalUserControl = null;

            // change page buttons back to gold
            PageModel.GetInstance.SelectPageModel();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            // remove the modal
            ModalModel.GetInstance.ModalUserControl = null;

            // change page buttons back to gold
            PageModel.GetInstance.SelectPageModel();
        }
    }
}
