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
    public class TileButton : Button
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

        public ImageSource TileImage
        {
            get { return (ImageSource)GetValue(TileImageProperty); }
            set { SetValue(TileImageProperty, value); }
        }

        public ImageSource WallPaperImage
        {
            get { return (ImageSource)GetValue(WallPaperImageProperty); }
            set { SetValue(WallPaperImageProperty, value); }
        }

        public SolidColorBrush SelectedBorderColour
        {
            get => (SolidColorBrush)GetValue(SelectedBorderColourProperty);
            set => SetValue(SelectedBorderColourProperty, value);
        }

        public static readonly DependencyProperty SelectedBorderColourProperty = DependencyProperty.Register(
            nameof(SelectedBorderColour), 
            typeof(SolidColorBrush), 
            typeof(TileButton), 
            new UIPropertyMetadata(OnSelectedBorderColourProperty_Changed));

        private static void OnSelectedBorderColourProperty_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
        }

        public static readonly DependencyProperty TileImageProperty = DependencyProperty.Register(nameof(TileImage), typeof(ImageSource), typeof(TileButton), new UIPropertyMetadata(null));
        public static readonly DependencyProperty WallPaperImageProperty = DependencyProperty.Register(nameof(WallPaperImage), typeof(ImageSource), typeof(TileButton), new UIPropertyMetadata(null));
        public static readonly DependencyProperty ParentSelectedProperty = DependencyProperty.Register(nameof(ParentSelected), typeof(bool), typeof(TileButton), new UIPropertyMetadata(null));
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(TileButton), new UIPropertyMetadata(null));
    }
}
