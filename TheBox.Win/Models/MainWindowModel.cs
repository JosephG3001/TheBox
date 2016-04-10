using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows;
using System.Reflection;
using System.IO;
using TheBox.Common;
using TheBox.Win.Classes;
using System.Windows.Controls;
using TheBox.Win.Views;
using System.Threading;
using TheBox.Common.Models;
using TheBox.Common.Menu;
using System.Windows.Input;

namespace TheBox.Win.Models
{
    /// <summary>
    /// MainWindowModel
    /// </summary>
    /// <seealso cref="TheBox.Win.Models.ModelBase" />
    public class MainWindowModel : ModelBase
    {
        #region Singleton

        /// <summary>
        /// The _instance
        /// </summary>
        private static MainWindowModel _instance;

        /// <summary>
        /// Gets the get instance.
        /// </summary>
        public static MainWindowModel GetInstance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = MainWindowModel.GetModel();
                }
                return _instance;
            }
        }

        #endregion Singleton

        #region Private Members

        /// <summary>
        /// The _is selected
        /// </summary>
        private bool _isTopMenuSelected;

        /// <summary>
        /// The _active user control
        /// </summary>
        private UserControl _activeUserControl;

        /// <summary>
        /// The _mouse cursor
        /// </summary>
        private Cursor _mouseCursor;

        #endregion Private Members

        #region Contructors

        /// <summary>
        /// Prevents a default instance of the <see cref="MainWindowModel"/> class from being created.
        /// </summary>
        private MainWindowModel()
        {
            // Start an endless tash that will update the time and date on the UI
            Task.Run(() => 
            {
                while (true)
                {
                    OnPropertyChanged("CurrentTime");
                    OnPropertyChanged("CurrentDate");
                    Thread.Sleep(1000);
                }
            });

            // Bind PageModel events
            PageModel.GetInstance.NoMoreMenuEntities += PageModel_NoMoreMenuEntities;

            // Hide the mouse cursor
            MouseCursor = Cursors.None;
        }

        #endregion Contructors

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        public bool IsTopMenuSelected
        {
            get { return _isTopMenuSelected; }
            set
            {
                _isTopMenuSelected = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the active user control.
        /// </summary>
        public UserControl ActiveUserControl
        {
            get { return _activeUserControl; }
            set
            {
                _activeUserControl = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the mouse cursor.
        /// </summary>
        public Cursor MouseCursor
        {
            get { return _mouseCursor; }
            set
            {
                _mouseCursor = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the current time.
        /// </summary>
        public string CurrentTime
        {
            get  {  return DateTime.Now.ToLongTimeString();  }
        }

        /// <summary>
        /// Gets the current date.
        /// </summary>
        public string CurrentDate
        {
            get { return DateTime.Now.ToLongDateString(); }
        }

        /// <summary>
        /// Gets the top menu items.
        /// </summary>
        public ObservableCollection<TopMenuItemModel> TopMenuItemModels
        {
            get;
            private set;
        }

        #endregion Public Properties



        #region PageModel Bound Events

        /// <summary>
        /// Handles the NoMoreMenuEntities event of the PageModel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void PageModel_NoMoreMenuEntities(object sender, EventArgs e)
        {
            this.IsTopMenuSelected = true;
            this.ActiveUserControl = null;
        }

        #endregion PageModel Bound Events

        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <returns></returns>
        private static MainWindowModel GetModel()
        {
            // new model
            MainWindowModel model = new MainWindowModel();

            // new topMenuItems observable collection
            model.TopMenuItemModels = new ObservableCollection<TopMenuItemModel>();

            // get plugins from folder
            foreach (var com in PluginFactory.GetInstance.Plugins)
            {
                // add new topMenuItemModel for the plugin
                model.TopMenuItemModels.Add(new TopMenuItemModel(com, com.ComponentName, com.ActivateComponent, com.appbar_icon, model.TopMenuItemModels.Count + 1));
            }

            // Add the settings button & initialise settings control
            IBoxKeyboardControl settingsControl = new SettingsControl();
            model.TopMenuItemModels.Add(new TopMenuItemModel((IBoxComponent)settingsControl, "Settings", () => { }, "appbar_cog", model.TopMenuItemModels.Count + 1));

            // Add the close button
            model.TopMenuItemModels.Add(new TopMenuItemModel(null, "Close", () => { Application.Current.Shutdown(); }, "appbar_door_leave", model.TopMenuItemModels.Count + 1));

            // select the first top menu item
            model.TopMenuItemModels.FirstOrDefault().IsSelected = true;

            // select the top menu panel by default
            model.IsTopMenuSelected = true;

            // return model
            return model;
        }
    }
}
