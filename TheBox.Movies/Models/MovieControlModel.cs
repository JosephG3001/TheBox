using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TheBox.Common;
using TheBox.Common.Menu;
using TheBox.Common.Models;
using TheBox.Movies.Settings;
using System.IO;
using TheBox.Movies.FileCache;
using TheBox.TVScheduleCommon;
using TheBox.TVScheduleCommon.PublicObjects;
using TheBox.Movies.TVSchedule;
using System.Windows.Threading;

namespace TheBox.Movies.Models
{
    /// <summary>
    /// MovieControlModel
    /// </summary>
    /// <seealso cref="TheBox.Common.ModelBase" />
    public class MovieControlModel : ModelBase
    {
        #region Singleton

        /// <summary>
        /// The _instance
        /// </summary>
        private static MovieControlModel _instance;

        /// <summary>
        /// Gets the get instance.
        /// </summary>
        public static MovieControlModel GetInstance
        {
            get { return _instance; }
        }

        #endregion Singleton

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MovieControlModel"/> class.
        /// </summary>
        /// <param name="componentName">Name of the component.</param>
        public MovieControlModel(Dispatcher dispatcher, string componentName)
        {
            // set the singleton member to this instance
            _instance = this;

            // Initialise the movie settings manager and load the XML settings for this movie plugin.
            this.MovieSettingsManager = new MovieSettingsManager(componentName);

            // Create new file cache manager (used for TVSchedule movie/programme matching)
            this.FileCacheManager = new FileCacheManager();

            // Create the TVScheduleManager
            this.TVScheduleManager = new TVScheduleManager(dispatcher, componentName);

            // creates the mini play options menu and menuItems.
            CreatePlayOptionsMenu();

            // viewing main menu by default
            PlayOptionsVisible = false;
        }

        #endregion Constructor

        /// <summary>
        /// The _play options visible flag
        /// </summary>
        private bool _playOptionsVisible;

        #region Public properties

        /// <summary>
        /// If Media Sub Menu Selected is true then we are controlling the play
        /// </summary>
        public bool PlayOptionsVisible
        {
            get { return _playOptionsVisible; }
            set
            {
                _playOptionsVisible = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the play options page model.
        /// </summary>
        public MenuEntity PlayOptionsMenu
        {
            get; set;
        }

        /// <summary>
        /// Gets the movie settings manager.
        /// </summary>
        public MovieSettingsManager MovieSettingsManager
        {
            get; private set;
        }

        /// <summary>
        /// Gets the file cache manager.
        /// </summary>
        public FileCacheManager FileCacheManager
        {
            get; private set;
        }

        /// <summary>
        /// Gets the tv schedule manager.
        /// </summary>
        public TVScheduleManager TVScheduleManager
        {
            get; private set;
        }

        #endregion Public properties

        /// <summary>
        /// Checks the media path.  If it's not found then the modal will show informing the user. 
        /// </summary>
        /// <returns>true if found otherwise false</returns>
        public bool CheckMediaPath()
        {
            // Check if a setting is not an empty or white space string
            if (string.IsNullOrWhiteSpace(this.MovieSettingsManager.MovieSettings.RootMediaPath))
            {
                return false;
            }

            // Check if the directory exists and is accessable
            if (!Directory.Exists(this.MovieSettingsManager.MovieSettings.RootMediaPath))        
            {
                return false;
            }

            // Directory is good.
            return true;
        }

        /// <summary>
        /// Creates the play options menu.
        /// </summary>
        private void CreatePlayOptionsMenu()
        {
            // new menuEntity for play options
            PlayOptionsMenu = new MenuEntity(1, 4);

            // Play button
            PlayOptionsMenu.AddMenuItemModel(new MenuItemModel()
            {
                DisplayText = "Play",
                IsSelected = true,
                ParentSelected = true,
                IsVisible = true,
                RelayCommand = new RelayCommand(() => { MediaPlayerModel.GetInstance.PlayMediaFullScreen(PlayOptions.Play); })
            });

            // Play All button
            PlayOptionsMenu.AddMenuItemModel(new MenuItemModel()
            {
                DisplayText = "Play All",
                IsSelected = false,
                ParentSelected = true,
                IsVisible = true,
                RelayCommand = new RelayCommand(() => { MediaPlayerModel.GetInstance.PlayMediaFullScreen(PlayOptions.PlayAll); })
            });

            // Suffle Button
            PlayOptionsMenu.AddMenuItemModel(new MenuItemModel()
            {
                DisplayText = "Shuffle",
                IsSelected = false,
                ParentSelected = true,
                IsVisible = true,
                RelayCommand = new RelayCommand(() => { MediaPlayerModel.GetInstance.PlayMediaFullScreen(PlayOptions.Shuffle); })
            });

            // Repeat Button
            PlayOptionsMenu.AddMenuItemModel(new MenuItemModel()
            {
                DisplayText = "Repeat",
                IsSelected = false,
                ParentSelected = true,
                IsVisible = true,
                RelayCommand = new RelayCommand(() => { MediaPlayerModel.GetInstance.PlayMediaFullScreen(PlayOptions.Repeat); })
            });
        }
    }
}
