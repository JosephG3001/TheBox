using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using TheBox.Common;
using TheBox.Common.Models;

namespace TheBox.Win.Models
{
    /// <summary>
    /// TopMenuItem
    /// </summary>
    public class TopMenuItemModel : ModelBase
    {
        private string _iconString;
        private bool _isSelected;
        private string _topMenuItemText;

        /// <summary>
        /// Gets the top menu item text.
        /// </summary>
        public string TopMenuItemText
        {
            get { return _topMenuItemText; }
            set
            {
                _topMenuItemText = value;
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
        /// Gets the action.
        /// </summary>
        public RelayCommand RelayCommand { get; private set; }

        /// <summary>
        /// Gets the icon.
        /// </summary>
        public Visual IconVisual
        {
            get
            {
                return (Visual)Application.Current.Resources[_iconString];
            }  
        }

        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Gets the component.
        /// </summary>
        public IBoxComponent Component { get; private set; }

        public string BackgroundImageUri { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TopMenuItemModel" /> class.
        /// </summary>
        /// <param name="topMenuItemText">The top menu item text.</param>
        /// <param name="action">The action.</param>
        /// <param name="icon">The icon.</param>
        /// <param name="index">The index.</param>
        public TopMenuItemModel(
            IBoxComponent component,
            string topMenuItemText, 
            Action action, 
            string icon, 
            int index,
            string backgroundImageUri)
        {
            this.Component = component;
            this.TopMenuItemText = topMenuItemText;
            this.RelayCommand = new RelayCommand(action);
            this._iconString = icon;
            this.Index = index;
            this.BackgroundImageUri = backgroundImageUri;
        }
    }
}
