using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TheBox.Common.UI
{
    public class MetroButtonWithIcon : Button
    {
        public bool ParentSelected
        {
            get { return (bool)GetValue(ParentSelectedProperty); }
            set { SetValue(ParentSelectedProperty, value); }
        }

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public Visual Visual
        {
            get { return (Visual)GetValue(VisualProperty); }
            set { SetValue(VisualProperty, value); }
        }

        public SolidColorBrush IconColour
        {
            get { return (SolidColorBrush)GetValue(IconColourProperty); }
            set { SetValue(IconColourProperty, value); }
        }

        public static readonly DependencyProperty ParentSelectedProperty = DependencyProperty.Register("ParentSelected", typeof(bool), typeof(MetroButtonWithIcon), new UIPropertyMetadata(null));
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(MetroButtonWithIcon), new UIPropertyMetadata(null));
        public static readonly DependencyProperty VisualProperty = DependencyProperty.Register("Visual", typeof(Visual), typeof(MetroButtonWithIcon), new UIPropertyMetadata(null));
        public static readonly DependencyProperty IconColourProperty = DependencyProperty.Register("IconColour", typeof(SolidColorBrush), typeof(MetroButtonWithIcon), new UIPropertyMetadata(null));


    }
}
