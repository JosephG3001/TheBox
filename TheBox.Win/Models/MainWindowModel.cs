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
    public class MainWindowModel : ModelBase
    {
        private static MainWindowModel _instance;
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

        private bool _isFullScreen;
        private bool _isTopMenuSelected;
        private UserControl _activeUserControl;
        private Cursor _mouseCursor;

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

        public bool IsFullScreen
        {
            get { return _isFullScreen; }
            set
            {
                _isFullScreen = value;
                OnPropertyChanged();
            }
        }

        public bool IsTopMenuSelected
        {
            get { return _isTopMenuSelected; }
            set
            {
                _isTopMenuSelected = value;
                OnPropertyChanged();
            }
        }

        public UserControl ActiveUserControl
        {
            get { return _activeUserControl; }
            set
            {
                _activeUserControl = value;
                OnPropertyChanged();
            }
        }

        public Cursor MouseCursor
        {
            get { return _mouseCursor; }
            set
            {
                _mouseCursor = value;
                OnPropertyChanged();
            }
        }

        public string CurrentTime
        {
            get  {  return DateTime.Now.ToLongTimeString();  }
        }

        public string CurrentDate
        {
            get { return DateTime.Now.ToLongDateString(); }
        }

        /// <summary>
        /// Gets the top menu items.
        /// </summary>
        public ObservableCollection<TopMenuItemModel> TopMenuItemModels
        {
            get; private set;
        }

        public MainUIModel MainUIModel => MainUIModel.Instance;

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

            // full screen by default
            model.IsFullScreen = true;

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

            // Add the toggle fullscreen button
            model.TopMenuItemModels.Add(new TopMenuItemModel(null, "Fullscreen", () => { model.IsFullScreen = !model.IsFullScreen; }, "appbar_fullscreen_box", model.TopMenuItemModels.Count + 1));

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
