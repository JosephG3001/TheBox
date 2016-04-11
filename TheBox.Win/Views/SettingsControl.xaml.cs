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
using TheBox.Win.Models;
using TheBox.Common;
using TheBox.Common.Models;
using TheBox.Win.Classes;
using TheBox.Common.Menu;

namespace TheBox.Win.Views
{
    /// <summary>
    /// Interaction logic for SettingsControl.xaml
    /// </summary>
    public partial class SettingsControl : UserControl, IBoxComponent, IBoxKeyboardControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsControl"/> class.
        /// </summary>
        public SettingsControl()
        {
            InitializeComponent();

            // set this user control's datacontext to the model
            DataContext = PageModel.GetInstance;
        }

        #region IBoxComponent implementation

        /// <summary>
        /// Gets the activate command.  The activate command will be a delegate that should run when the component is activated.
        /// </summary>
        public void ActivateComponent()
        {
            PageModel.GetInstance.DoBreadCrumbs(this.ComponentName);

            // We've hit the settings screen so show the mouse cursor
            MainWindowModel.GetInstance.MouseCursor = Cursors.Arrow;

            // menu items for new menu entity
            List<MenuItemModel> menuItems = new List<MenuItemModel>();

            // Create setting menu item models for plugins
            foreach (var com in PluginFactory.GetInstance.Plugins)
            {
                // add new MenuItemModel for the plugin
                menuItems.Add(new MenuItemModel()
                {
                    DisplayText = com.ComponentName + " settings",
                    ParentSelected = true,
                    RelayCommand = new RelayCommand(() =>
                    {
                        com.NavigateToSettings();
                    })
                });
            }

            // navigate to new menu
            PageModel.GetInstance.NavigateForwards(menuItems);

            PageModel.GetInstance.DoBreadCrumbs(this.ComponentName);
        }

        public UserControl GetUserControl()
        {
            return this;
        }

        public string ComponentName
        {
            get { return "Settings"; }
        }

        public string appbar_icon
        {
            get { return "appbar_cog"; }
        }

        #endregion IBoxComponent implementation

        #region IBoxKeyboardControl implementation

        /// <summary>
        /// Raise this event when the backspace key is pressed and you wish to return the focus to the main application.
        /// </summary>
        //public event EventHandler ReturnFocusToMainWindow;

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

                if (PageModel.GetInstance.MenuEntityModel == null)
                {
                    // leaving the settings menu so hide the mouse cursor
                    MainWindowModel.GetInstance.MouseCursor = Cursors.None;
                }
            }

            // up
            if (e.Key == Key.Up)
            {
                PageModel.GetInstance.MoveUp();
                PageModel.GetInstance.BindItems();
                PageModel.GetInstance.UpdatePaginationLabels();
            }

            // down
            if (e.Key == Key.Down)
            {
                PageModel.GetInstance.MoveDown();
                PageModel.GetInstance.BindItems();
                PageModel.GetInstance.UpdatePaginationLabels();
            }

            // enter
            if (e.Key == Key.Enter)
            {
                PageModel.GetInstance.VisibleMenuItemModels.Where(m => m.IsSelected).First().RelayCommand.Execute(null);
            }

            PageModel.GetInstance.DoBreadCrumbs(this.ComponentName);
        }

        public void NavigateToSettings()
        {
            // implement this when we want to map keys and what not
        }

        #endregion IBoxKeyboardControl implementation
    }
}
