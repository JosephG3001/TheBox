using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using TheBox.Common.Menu;
using TheBox.Common.Models;
using TheBox.Games.Models;
using TheBox.Games.Settings;
using System.IO;

namespace TheBox.Games.Modals
{
    /// <summary>
    /// Interaction logic for GameOptionsModal.xaml
    /// </summary>
    public partial class GameOptionsModal : UserControl, IBoxKeyboardControl
    {
        /// <summary>
        /// The _rom file
        /// </summary>
        private string _consoleAndGameName;

        /// <summary>
        /// The _action
        /// </summary>
        private Action _action;

        /// <summary>
        /// Gets or sets the game options menu.
        /// </summary>
        public MenuEntity GameOptionsMenu
        {
            get;set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameOptionsModal"/> class.
        /// </summary>
        public GameOptionsModal(string consoleAndGameName, Action action)
        {
            _consoleAndGameName = consoleAndGameName;
            _action = action;
            InitializeComponent();
            CreateGameOptionsMenu();
            this.DataContext = this;
        }

        /// <summary>
        /// Creates the game options menu.
        /// </summary>
        private void CreateGameOptionsMenu()
        {
            this.GameOptionsMenu = new MenuEntity(12, 1);

            // Play game option
            GameOptionsMenu.AddMenuItemModel(new MenuItemModel() {
                DisplayText = "Play",
                IsSelected = true,
                ParentSelected = true,
                RelayCommand = new RelayCommand(() => {

                })
            });

            // Save current cover option
            GameOptionsMenu.AddMenuItemModel(new MenuItemModel()
            {
                DisplayText = "Save Current Cover",
                IsSelected = false,
                ParentSelected = true,
                RelayCommand = new RelayCommand(() => {

                })
            });

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

            // up
            if (e.Key == Key.Up)
            {
                GameOptionsMenu.MoveUp();
            }

            // down
            if (e.Key == Key.Down)
            {
                GameOptionsMenu.MoveDown();
            }

            // enter
            if (e.Key == Key.Enter)
            {
                switch (GameOptionsMenu.SelectedMenuItemModel.DisplayText)
                {
                    case "Play":
                        _action();
                        break;
                    case "Save Current Cover":
                        SaveCurrentImage();

                        // remove the modal
                        ModalModel.GetInstance.ModalUserControl = null;

                        // change page buttons back to gold
                        PageModel.GetInstance.SelectPageModel();
                        break;
                }
            }
        }

        private void SaveCurrentImage()
        {
            string imagePath = System.IO.Path.Combine(GameImageModel.GetInstance.ImageFolderPath, _consoleAndGameName) + ".png";

            using (FileStream stream = new FileStream(imagePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Delete))
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)GameControl.GetInstance.GameImageGrid.Source));
                encoder.Save(stream);
            }

            GameControlModel.GetInstance.UsingLocalImage = true;
        }
    }
}
