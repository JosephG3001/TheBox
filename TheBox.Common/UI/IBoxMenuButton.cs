using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace TheBox.Common.UI
{
    public class IBoxMenuButton : Button
    {
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public bool ParentSelected
        {
            get { return (bool)GetValue(ParentSelectedProperty); }
            set { SetValue(ParentSelectedProperty, value); }
        }

        public bool Odd
        {
            get { return (bool)GetValue(OddProperty); }
            set { SetValue(OddProperty, value); }
        }

        public TextAlignment TextAlignment
        {
            get => (TextAlignment)GetValue(TextAlignmentProperty);
            set => SetValue(TextAlignmentProperty, value);
        }

        public static readonly DependencyProperty TextAlignmentProperty = DependencyProperty.Register(
            nameof(TextAlignment),
            typeof(TextAlignment),
            typeof(IBoxMenuButton),
            new UIPropertyMetadata(null)
            );

        public static readonly DependencyProperty OddProperty = DependencyProperty.Register(
            "Odd", 
            typeof(bool), 
            typeof(IBoxMenuButton), 
            new UIPropertyMetadata(null));

        public static readonly DependencyProperty ParentSelectedProperty = DependencyProperty.Register(
            "ParentSelected", 
            typeof(bool), 
            typeof(IBoxMenuButton),
            new UIPropertyMetadata(null));

        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
            "IsSelected", 
            typeof(bool),
            typeof(IBoxMenuButton),
            new UIPropertyMetadata(null));

    }
}
