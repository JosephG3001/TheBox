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
using TheBox.Common.Menu;
using TheBox.Common.Models;
using TheBox.Games.Models;
using TheBox.Games.Settings;

namespace TheBox.Games
{
    /// <summary>
    /// Interaction logic for GameControl.xaml
    /// </summary>
    public partial class GameControl : UserControl, IBoxComponent, IBoxKeyboardControl
    {
        /// <summary>
        /// The _settings
        /// </summary>
        private GameSettingsManager _settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameControl"/> class.
        /// </summary>
        public GameControl()
        {
            InitializeComponent();

            // initialise the GameControlModel singleton.
            new GameControlModel(this.Dispatcher, this.ComponentName);

            // set this gameControl's datacontext to the singleton gameControlModel
            this.DataContext = GameControlModel.GetInstance;
        }

        #region IBoxComponent implementation

        /// <summary>
        /// Gets the activate command.  The activate command will be a delegate that should run when the component is activated.
        /// </summary>
        public void ActivateComponent()
        {
            
        }

        /// <summary>
        /// Navigates to settings.
        /// </summary>
        /// <param name="previousMenu"></param>
        /// <returns></returns>
        public void NavigateToSettings()
        {
            // menu items for new menu entity
            List<MenuItemModel> menuItems = new List<MenuItemModel>();

            // add the "Add Emulator" menu item
            menuItems.Add(new MenuItemModel()
            {
                DisplayText = "Add New Emulator",
                ParentSelected = true,
                RelayCommand = new RelayCommand(() =>
                {
                    ModalModel.GetInstance.ModalUserControl = new EmulatorSettingModal();
                    PageModel.GetInstance.UnSelectPageModel();
                })
            });

            // add menu items for each of the saved emulator settings
            if (_settings.EmulatorSettings.EmulatorSettingList != null)
            {
                foreach (var setting in _settings.EmulatorSettings.EmulatorSettingList)
                {
                    menuItems.Add(new MenuItemModel()
                    {
                        DisplayText = setting.EmulatatedSystemName,
                        ParentSelected = true,
                        RelayCommand = new RelayCommand(() =>
                        {

                        })
                    });
                }
            }

            // navigate to new menu
            PageModel.GetInstance.NavigateForwards(menuItems);
        }

        /// <summary>
        /// Gets the appbar_icon.
        /// </summary>
        /// <remarks>
        /// See http://modernuiicons.com/ for icons
        /// </remarks>
        public string appbar_icon
        {
            get { return "appbar_controller_xbox"; }
        }

        /// <summary>
        /// Gets the name of the component.
        /// </summary>
        public string ComponentName
        {
            get { return "Games"; }
        }

        /// <summary>
        /// Gets the underlying user control.
        /// </summary>
        /// <returns></returns>
        public UserControl GetUserControl()
        {
            return this;  
        }

        #endregion IBoxComponent implementation

        #region IBoxKeyboardControl implementation

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
                PageModel.GetInstance.NavigateBackwards();
            }
        }

        #endregion IBoxKeyboardControl implementation
    }
}
