using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

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

        public void SetBackground(string uri)
        {
            
        }
    }
}
