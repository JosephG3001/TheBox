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
    /// Interaction logic for PlayPanelControl.xaml
    /// </summary>
    public partial class PlayPanelControl : UserControl, IBoxKeyboardControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayPanelControl"/> class.
        /// </summary>
        public PlayPanelControl()
        {
            this.DataContext = MovieControlModel.GetInstance;

            InitializeComponent();
        }

        /// <summary>
        /// Handles the keyboard events of the mini play panel.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="T:System.Windows.Input.KeyEventArgs" /> instance containing the event data.</param>
        public void HandleKeyDown(object sender, KeyEventArgs e)
        {
            // send keystrokes to the MediaPlayerModel? (controls the actual video)
            if (MediaPlayerModel.GetInstance.MediaPlayer.fullScreen ||
                !MediaPlayerModel.GetInstance.UserPressedStop)
            {
                MediaPlayerModel.GetInstance.HandleKeyDown(sender, e);
                return;
            }

            // we like the ability to skip through preview 
            if (//e.Key == Key.Left || 
                //e.Key == Key.Right ||
                e.Key == Key.MediaNextTrack || 
                e.Key == Key.N ||
                e.Key == Key.P || e.Key == (Key)177)
            {
                MediaPlayerModel.GetInstance.HandleKeyDown(this, e);
            }

            if (e.Key == Key.Back)
            {
                // move back to the main video menu
                MovieControlModel.GetInstance.PlayOptionsVisible = false;
                PageModel.GetInstance.SelectPageModel();

                // reset the play panel options menu
                MovieControlModel.GetInstance.PlayOptionsMenu.UnselectButtons();
                MovieControlModel.GetInstance.PlayOptionsMenu.CurrentPageButtonIndex = 0;
            }

            if (e.Key == Key.Up)
            {
                MovieControlModel.GetInstance.PlayOptionsMenu.MoveUp();
            }

            if (e.Key == Key.Down)
            {
                MovieControlModel.GetInstance.PlayOptionsMenu.MoveDown();
            }

            if (e.Key == Key.Left)
            {
                MovieControlModel.GetInstance.PlayOptionsMenu.MoveLeft();
            }

            if (e.Key == Key.Right)
            {
                MovieControlModel.GetInstance.PlayOptionsMenu.MoveRight();
            }

            if (e.Key == Key.Enter)
            {
                // play using the selected menu items play option
                MovieControlModel.GetInstance.PlayOptionsMenu.SelectedMenuItemModel.RelayCommand.action();
            }
        }
    }
}
