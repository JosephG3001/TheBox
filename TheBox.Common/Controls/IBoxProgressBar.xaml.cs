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

namespace TheBox.Common.Controls
{
    /// <summary>
    /// Interaction logic for IBoxProgressBar.xaml
    /// </summary>
    public partial class IBoxProgressBar : UserControl
    {
        public IBoxProgressBar()
        {
            InitializeComponent();
        }

        public int Max
        {
            get { return (int)GetValue(MaxProperty); }
            set { SetValue(MaxProperty, value); }
        }

        public int Min
        {
            get { return (int)GetValue(MinProperty); }
            set { SetValue(MinProperty, value); }
        }

        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty MaxProperty = DependencyProperty.Register("Max", typeof(int), typeof(IBoxProgressBar), new PropertyMetadata(100, MaxProperty_Changed));
        public static readonly DependencyProperty MinProperty = DependencyProperty.Register("Min", typeof(int), typeof(IBoxProgressBar), new PropertyMetadata(0, MinPropertyChanged));
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(int), typeof(IBoxProgressBar), new PropertyMetadata(0, ValuePropertyChanged));

        private static void ValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IBoxProgressBar obj = d as IBoxProgressBar;
            obj.RecalculateBarWidth();
        }

        private static void MinPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IBoxProgressBar obj = d as IBoxProgressBar;
            obj.RecalculateBarWidth();
        }

        private static void MaxProperty_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IBoxProgressBar obj = d as IBoxProgressBar;
            obj.RecalculateBarWidth();
        }

        private void RecalculateBarWidth()
        {
            double val = Value;
            double max = Max;

            double percent = (val / max) * 100;
            BarGrid.Width = ActualWidth * (int)percent / 100;
        }
    }
}
