﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using TheBox.Common;
using TheBox.Common.Models;
using TheBox.Games;
using TheBox.Games.Settings;

namespace TheBox.Games.Models
{
    /// <summary>
    /// GameControlModel - 
    /// </summary>
    /// <seealso cref="TheBox.Common.Models.ModelBase" />
    public class GameControlModel : ModelBase
    {
        #region Singleton

        /// <summary>
        /// The _instance
        /// </summary>
        private static GameControlModel _instance;

        /// <summary>
        /// Gets the get instance.
        /// </summary>
        public static GameControlModel GetInstance
        {
            get { return _instance; }
        }

        #endregion Singleton

        /// <summary>
        /// The _component name
        /// </summary>
        private string _componentName;

        /// <summary>
        /// The _dispatcher
        /// </summary>
        private Dispatcher _dispatcher;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GameControlModel"/> class.
        /// </summary>
        public GameControlModel(Dispatcher dispatcher, string componentName)
        {
            // set the singleton to this instance
            _instance = this;

            // store parameters
            this._componentName = componentName;
            this._dispatcher = dispatcher;

            // new game settings manager
            this.GameSettingsManager = new GameSettingsManager(componentName);
        }

        #endregion Constructors

        /// <summary>
        /// Gets or sets the game settings manager.
        /// </summary>
        public GameSettingsManager GameSettingsManager
        {
            get; set;
        }


        private string _currentEmulatorImage;
        public string CurrentEmulatorImage
        {
            get { return _currentEmulatorImage; }
            set
            {
                _currentEmulatorImage = value;
                OnPropertyChanged();
            }
        }

        private string _currentGameImage;
        public string CurrentGameImage
        {
            get { return _currentGameImage; }
            set
            {
                _currentGameImage = value;
                OnPropertyChanged();
            }
        }

        private bool _usingLocalImage;
        public bool UsingLocalImage
        {
            get { return _usingLocalImage; }
            set
            {
                _usingLocalImage = value;
                OnPropertyChanged();
            }
        }
    }
}
