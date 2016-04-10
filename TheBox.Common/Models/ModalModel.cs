using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TheBox.Common.Models
{
    /// <summary>
    /// Modal Model
    /// </summary>
    /// <seealso cref="TheBox.Common.Models.ModelBase" />
    public class ModalModel : ModelBase
    {
        #region Singleton

        /// <summary>
        /// The _instance
        /// </summary>
        private static ModalModel _instance;

        /// <summary>
        /// Gets the get instance.
        /// </summary>
        public static ModalModel GetInstance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ModalModel();
                }
                return _instance;
            }
        }

        #endregion Singleton

        #region Constructor

        /// <summary>
        /// Prevents a default instance of the <see cref="ModalModel"/> class from being created.
        /// </summary>
        private ModalModel()
        {
            _modalVisible = false;
        }

        #endregion Constructor

        private bool _modalVisible;
        private UserControl _modalUserControl;

        public bool ModalVisible
        {
            get { return _modalVisible; }
            set
            {
                _modalVisible = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the modal user control.
        /// </summary>
        public UserControl ModalUserControl
        {
            get { return _modalUserControl; }
            set
            {
                _modalUserControl = value;
                OnPropertyChanged();
                if (value != null)
                {
                    ModalVisible = true;
                }
                else
                {
                    ModalVisible = false;
                }
            }
        }
    }
}
