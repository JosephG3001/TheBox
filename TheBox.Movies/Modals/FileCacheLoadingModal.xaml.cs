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
using TheBox.Common.Models;

namespace TheBox.Movies
{
    /// <summary>
    /// Interaction logic for FileCacheLoadingModal.xaml
    /// </summary>
    public partial class FileCacheLoadingModal : UserControl, IBoxKeyboardControl
    {
        public FileCacheLoadingModal()
        {
            InitializeComponent();
        }

        public void HandleKeyDown(object sender, KeyEventArgs e)
        {
            // do nothing - the modal will self close
        }
    }
}
