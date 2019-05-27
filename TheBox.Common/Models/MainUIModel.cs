using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TheBox.Common.Models
{
    public class MainUIModel : ModelBase
    {
        private static MainUIModel instance;
        public static MainUIModel Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MainUIModel();
                }
                return instance;
            }
        }

        //private Task animateTask;        

        public string CurrentBackgroundImageUri { get; set; }

        private ImageSource backgroundImageSource;
        public ImageSource BackgroundImageSource
        {
            get { return backgroundImageSource; }
            set
            {
                backgroundImageSource = value;
                OnPropertyChanged();
            }            
        }

        private MainUIModel()
        {

        }

        //public void SetBackground(string uri)
        //{
        //    if (previousBackgroundImageSource != null)
        //    {
        //        backgroundImageSource = previousBackgroundImageSource;
        //        previousBackgroundImageSource = null;
        //    }
        //}

        

        public void SetBackGroundImage(string uri, string backupImage)
        {
            if (File.Exists(uri) && CurrentBackgroundImageUri != uri)
            {
                BitmapImage logo = new BitmapImage();
                logo.BeginInit();
                logo.UriSource = new Uri(uri, uriKind: UriKind.RelativeOrAbsolute);
                logo.EndInit();
                MainUIModel.Instance.BackgroundImageSource = logo;
                CurrentBackgroundImageUri = uri;
                return;
            }

            if (/*File.Exists(backupImage) &&*/ CurrentBackgroundImageUri != backupImage)
            {
                BitmapImage logo = new BitmapImage();
                logo.BeginInit();
                logo.UriSource = new Uri(backupImage, uriKind: UriKind.RelativeOrAbsolute);
                logo.EndInit();
                MainUIModel.Instance.BackgroundImageSource = logo;
                CurrentBackgroundImageUri = backupImage;
                return;
            }
        }
    }
}
