using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using TheBox.Common.Models;

namespace TheBox.Common.Menu
{
    /// <summary>
    /// MenuItemModel
    /// </summary>
    /// <seealso cref="TheBox.Common.ModelBase" />
    public class MenuItemModel : ModelBase
    {
        private bool _isVisible;
        private bool _isSelected;
        private bool _parentSelected;
        private bool _odd;
        private string _displayText;
        private ImageSource tileImage;

        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        public object Tag
        {
            get;set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is visible.
        /// </summary>
        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                _isVisible = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        public bool Odd
        {
            get { return _odd; }
            set
            {
                _odd = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        public bool ParentSelected
        {
            get { return _parentSelected; }
            set
            {
                _parentSelected = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the relay command.  The command that will execute when the menu button is clicked or enter is pressed while selected
        /// </summary>
        public RelayCommand RelayCommand { get; set; }

        /// <summary>
        /// Gets or sets the display text.
        /// </summary>
        public string DisplayText
        {
            get { return _displayText; }
            set
            {
                _displayText = value;
                OnPropertyChanged();
            }
        }

        public ImageSource TileImage
        { 
            get { return tileImage; }
            set
            {
                tileImage = value;
                OnPropertyChanged();
            }
        }

        public string TileFilePath { get; set; }

        /// <summary>
        /// Gets or sets the file path for videos, roms ect.
        /// </summary>
        public string FilePath { get; set; }        
    }
}