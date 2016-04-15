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
using TheBox.Movies.Models;
using TheBox.Movies.Settings;
using System.Threading;
using TheBox.Movies.FileCache;
using System.IO;
using TheBox.TVScheduleCommon;
using TheBox.TVScheduleCommon.PublicObjects;

namespace TheBox.Movies
{
    /// <summary>
    /// Interaction logic for MovieControl.xaml
    /// </summary>
    public partial class MovieControl : UserControl, IBoxComponent, IBoxKeyboardControl
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MovieControl"/> class.
        /// </summary>
        public MovieControl()
        {
            // set the model to page model
            this.DataContext = PageModel.GetInstance;

            // initialise the movie model
            new MovieControlModel(Dispatcher, this.ComponentName);
            

            // bind events
            MovieControlModel.GetInstance.FileCacheManager.CacheLoadingStarted += FileCacheManager_CacheLoadingStarted;
            MovieControlModel.GetInstance.FileCacheManager.CacheLoadingComplete += FileCacheManager_CacheLoadingComplete;

            InitializeComponent();


            // Create the interop host control.
            System.Windows.Forms.Integration.WindowsFormsHost host = new System.Windows.Forms.Integration.WindowsFormsHost();
            host.Focusable = false;

            // Create the ActiveX control.
            AxWMPLib.AxWindowsMediaPlayer axWmp = new AxWMPLib.AxWindowsMediaPlayer();

            // Assign the ActiveX control as the host control's child.
            host.Child = axWmp;

            // Add the interop host control to the Grid
            // control's collection of child controls.
            this.mediaPlayerGrid.Children.Add(host);

            // initialise media models
            new MediaPlayerModel(axWmp);
        }

        #endregion Constructors

        /// <summary>
        /// Handles the CacheLoadingComplete event of the FileCacheManager control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void FileCacheManager_CacheLoadingComplete(object sender, EventArgs e)
        {
            // The file cache finished loading so hide the "loading file cache" modal
            ModalModel.GetInstance.ModalUserControl = null;
        }

        /// <summary>
        /// Handles the CacheLoadingStarted event of the FileCacheManager control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void FileCacheManager_CacheLoadingStarted(object sender, EventArgs e)
        {
            // local file cache started to load so show the "loading file cache" modal
            ModalModel.GetInstance.ModalUserControl = new FileCacheLoadingModal();
        }

        #region IBoxComponent implementation

        /// <summary>
        /// Gets the activate command.  The activate command will be a delegate that should run when the component is activated.
        /// </summary>
        public void ActivateComponent()
        {
            // this is where we will populate the video files or media root
            PageModel.GetInstance.DoBreadCrumbs(this.ComponentName);

            // is the media setting ok?
            if (!MovieControlModel.GetInstance.CheckMediaPath())
            {
                ModalModel.GetInstance.ModalUserControl = new NoMediaPathSelectedModal();
                return;
            }

            // do we need to initialise the file cache?
            if (!MovieControlModel.GetInstance.FileCacheManager.Initialised)
            {
                // initialise the file cache
                Task.Run(async () =>
                {
                    await MovieControlModel.GetInstance.FileCacheManager.Initialise(MovieControlModel.GetInstance.MovieSettingsManager.MovieSettings.RootMediaPath);
                    Application.Current.Dispatcher.Invoke(ActivateComponentCallback);
                });
            }
            else
            {
                // no need to initialise file cache
                ActivateComponentCallback();
            }
        }

        /// <summary>
        /// Called after the file cache finishes.
        /// </summary>
        private void ActivateComponentCallback()
        {
            // create menuItems for root
            GetMenuItemsForDirectory(MovieControlModel.GetInstance.MovieSettingsManager.MovieSettings.RootMediaPath, true);

            // fix WMP crash - don't show it until the window has finished rendering
            MediaPlayerModel.GetInstance.MediaPlayer.uiMode = "none";
            mediaPlayerGrid.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Gets the menu items for directory.
        /// </summary>
        /// <param name="dir">The dir.</param>
        /// <param name="root">if set to <c>true</c> [root].</param>
        private void GetMenuItemsForDirectory(string dir, bool root)
        {
            // prepare list of menuItems for the new menu we're going to navigate to.
            List<MenuItemModel> menuItems = new List<MenuItemModel>();

            // files
            foreach (var filter in FileCacheManager.VideoFileExtensions)
            {
                // Get files.
                foreach (var file in Directory.GetFiles(dir, filter))
                {
                    // Create the video file menu item
                    menuItems.Add(new MenuItemModel()
                    {
                        DisplayText = System.IO.Path.GetFileNameWithoutExtension(file),
                        ParentSelected = true,
                        FilePath = file,
                        RelayCommand = new RelayCommand(() =>
                        {
                            // move focus to the mini playpanel
                            MovieControlModel.GetInstance.PlayOptionsVisible = true;
                            PageModel.GetInstance.UnSelectPageModel();
                        })
                    });
                }
            }

            // child folders
            foreach (var folder in Directory.GetDirectories(dir))
            {
                // Create the child folder menu item
                menuItems.Add(new MenuItemModel()
                {
                    DisplayText = new DirectoryInfo(folder).Name,
                    ParentSelected = true,
                    RelayCommand = new RelayCommand(() =>
                    {
                        // directory so run this method again using the child directory as the parameter
                        GetMenuItemsForDirectory(folder, false);
                    })
                });
            }

            // sort by displayText so files and folders will be mixed
            menuItems = menuItems.OrderBy(m => m.DisplayText).ToList();

            // If this directory is the root then add the "What's on TV" button.
            if (root == true && MovieControlModel.GetInstance.TVScheduleManager.TVScheduleService != null)
            {
                // add the "What's on TV" menu item
                menuItems.Insert(0, new MenuItemModel()
                {
                    DisplayText = "What's on TV",
                    ParentSelected = true,
                    RelayCommand = new RelayCommand(() =>
                    {
                        // navigate to the TV schedule menu
                        MovieControlModel.GetInstance.TVScheduleManager.NavigateToTVSchedules();
                    })
                });
            }

            // navigate to new menu
            PageModel.GetInstance.NavigateForwards(menuItems);

            // update the bread crumbs label
            PageModel.GetInstance.DoBreadCrumbs(this.ComponentName);
        }

        /// <summary>
        /// Navigates to settings.
        /// </summary>
        public void NavigateToSettings()
        {
            // get the settings for the movie component

            // menu items for new menu entity
            List<MenuItemModel> menuItems = new List<MenuItemModel>();

            // add the "Add Emulator" menu item
            menuItems.Add(new MenuItemModel()
            {
                DisplayText = "Root Media Path",
                ParentSelected = true,
                RelayCommand = new RelayCommand(() =>
                {
                    ModalModel.GetInstance.ModalUserControl = new MovieSettingsModal();
                    PageModel.GetInstance.UnSelectPageModel();
                })
            });

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
            get { return "appbar_tv"; }
        }

        /// <summary>
        /// Gets the name of the component.
        /// </summary>
        public string ComponentName
        {
            get { return "Movies"; }
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
            // send keystrokes to the mini play panel?
            if (MovieControlModel.GetInstance.PlayOptionsVisible)
            {
                PlayPanel.HandleKeyDown(sender, e);
                return;
            }

            // we like the ability to skip through preview 
            if (e.Key == Key.Left ||
                e.Key == Key.Right ||
                e.Key == Key.MediaNextTrack ||
                e.Key == Key.N ||
                e.Key == Key.P || e.Key == (Key)177)
            {
                MediaPlayerModel.GetInstance.HandleKeyDown(this, e);
            }

            // backspace
            if (e.Key == Key.Back)
            {
                PageModel.GetInstance.NavigateBackwards();
                MediaPlayerModel.GetInstance.PreviewMedia(true);
            }

            // up
            if (e.Key == Key.Up)
            {
                PageModel.GetInstance.MoveUp();
                PageModel.GetInstance.BindItems();
                PageModel.GetInstance.UpdatePaginationLabels();
                MediaPlayerModel.GetInstance.PreviewMedia(false);
            }

            // down
            if (e.Key == Key.Down)
            {
                PageModel.GetInstance.MoveDown();
                PageModel.GetInstance.BindItems();
                PageModel.GetInstance.UpdatePaginationLabels();
                MediaPlayerModel.GetInstance.PreviewMedia(false);
            }

            // enter
            if (e.Key == Key.Enter)
            {
                // Are we pressing enter whislt a video file is selected?
                MenuItemModel selectedItemModel = PageModel.GetInstance.VisibleMenuItemModels.Where(m => m.IsSelected).First();
                if (selectedItemModel.FilePath != null)
                {
                    // go to the play panel
                    PageModel.GetInstance.VisibleMenuItemModels.Where(m => m.IsSelected).First().RelayCommand.Execute(null);
                }
                else
                {
                    // go to the next sub folder
                    PageModel.GetInstance.VisibleMenuItemModels.Where(m => m.IsSelected).First().RelayCommand.Execute(null);

                    MediaPlayerModel.GetInstance.PreviewMedia(false);
                }
            }
            PageModel.GetInstance.DoBreadCrumbs(this.ComponentName);
        }

        #endregion IBoxKeyboardControl implementation
    }
}
