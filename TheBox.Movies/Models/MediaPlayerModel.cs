using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using TheBox.Common.Models;
using TheBox.Movies.FileCache;
using System.Reflection;
using System.Windows;
using TheBox.Common;
using System.Windows.Input;

namespace TheBox.Movies.Models
{
    /// <summary>
    /// MediaPlayerModel
    /// </summary>
    public class MediaPlayerModel : IBoxKeyboardControl
    {
        #region Singleton

        /// <summary>
        /// The _instance
        /// </summary>
        private static MediaPlayerModel _instance;

        /// <summary>
        /// Gets the get instance.
        /// </summary>
        public static MediaPlayerModel GetInstance
        {
            get { return _instance; }
        }

        #endregion Singleton

        #region Properties / Members

        /// <summary>
        /// The _media preview model
        /// </summary>
        private MediaPreviewModel _mediaPreviewModel;

        /// <summary>
        /// The _media player
        /// </summary>
        private AxWMPLib.AxWindowsMediaPlayer _mediaPlayer;

        /// <summary>
        /// Gets the media player.
        /// </summary>
        public AxWMPLib.AxWindowsMediaPlayer MediaPlayer
        {
            get { return _mediaPlayer; }
        }

        /// <summary>
        /// Flag to indicate whether to ignore user input
        /// </summary>
        public bool BlockKeyPresses { get; private set; }

        /// <summary>
        /// The current play option selected - will be set back to play when a user presses stop to prevent
        /// the auto play sequence from previewing another file
        /// </summary>
        private PlayOptions _playOptions;

        #endregion Properties / Members

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaPlayerModel"/> class.
        /// </summary>
        /// <param name="mediaPlayer">The media player.</param>
        public MediaPlayerModel(AxWMPLib.AxWindowsMediaPlayer mediaPlayer)
        {
            _instance = this;
            this._mediaPlayer = mediaPlayer;
            this._mediaPlayer.PlayStateChange += _mediaPlayer_PlayStateChange;

            this._mediaPreviewModel = new MediaPreviewModel(mediaPlayer);
        }

        #endregion Constructor


        #region Skip Methods

        /// <summary>
        /// Skips backwards.
        /// </summary>
        /// <param name="amount">The amount.</param>
        private void SkipBackwards(int amount)
        {
            try
            {
                //if (isPlayingMediaPlayer())
                {
                    MediaPlayer.Ctlcontrols.pause();
                    MediaPlayer.Ctlcontrols.currentPosition -= amount;
                    int i = 100;
                    while (i > 0)
                    {
                        Thread.Sleep(10);
                        i--;
                    }
                    MediaPlayer.Ctlcontrols.play();
                }
            }
            catch { }
        }

        /// <summary>
        /// Skips forward.
        /// </summary>
        /// <param name="amount">The amount.</param>
        private void SkipForward(int amount)
        {
            try
            {
               // if (isPlayingMediaPlayer())
                {
                    // Get duration.
                    double duration = MediaPlayer.currentMedia.duration;

                    // Can we skip forward?
                    if (duration > (duration + amount))
                    {
                        // Can't skip forward.
                        MediaPlayer.Ctlcontrols.currentPosition = (MediaPlayer.currentMedia.duration + 1);
                    }
                    else
                    {
                        // Do the skip.
                        //frmMain.MediaPlayer1.Ctlcontrols.pause();
                        MediaPlayer.Ctlcontrols.currentPosition += amount;
                        int i = 100;
                        while (i > 0)
                        {
                            Thread.Sleep(10);
                            i--;
                        }
                    }
                }
            }
            catch { }
        }

        #endregion Skip Methods

        #region Play Media Full Screen

        /// <summary>
        /// Play media full screen
        /// Some movies (DIVX) take a long time to load so this is required
        /// </summary>
        public void PlayMediaFullScreen(PlayOptions playOption)
        {
            _mediaPreviewModel.CancelPreviews();
            _playOptions = playOption;

            Thread playthread = new Thread(new ThreadStart(delegate
            {
                bool crashing = true;
                while (crashing)
                {
                    try
                    {
                        this.MediaPlayer.URL = PageModel.GetInstance.SelectedMenuItemModel.FilePath;
                        this.MediaPlayer.settings.volume = 100;

                        this.MediaPlayer.Ctlcontrols.currentPosition = 0;
                        this.MediaPlayer.Ctlcontrols.play();
                        crashing = false;
                    }
                    catch
                    {
                        Thread.Sleep(20);
                    }
                }

                SetToFullScreen();

                // Playing full screen, give focus back to form1.
                Application.Current.Dispatcher.Invoke(() => 
                {
                    Application.Current.MainWindow.Focus();
                });


                // Fix bug where pressing play too quick goes back to non-full screen.

                // Sleep for a bit.
                Thread.Sleep(1000);

                SetToFullScreen();

                // Playing full screen, give focus back to form1.
                Application.Current.Dispatcher.Invoke(() => {
                    Application.Current.MainWindow.Focus();
                });
            }));

            playthread.Name = "playThread";
            playthread.IsBackground = true;
            playthread.Start();
        }

        /// <summary>
        /// Sets to full screen.  Repeats until it's actually fullscreen. 
        /// </summary>
        private void SetToFullScreen()
        {
            bool crashing = true;
            while (crashing || this.MediaPlayer.fullScreen == false)
            {
                try
                {
                    this.MediaPlayer.fullScreen = true; //TODO
                    crashing = false;
                }
                catch
                {
                    Thread.Sleep(100);
                }
            }
        }

        #endregion Play Media Full Screen

        /// <summary>
        /// Previews the media.
        /// </summary>
        /// <param name="stopPreview">if set to <c>true</c> [stop preview].</param>
        public void PreviewMedia(bool stopPreview)
        {
            this._mediaPreviewModel.PreviewMedia(stopPreview);
        }

        #region Auto Play Methods

        /// <summary>
        /// Runs the automatic play sequence.
        /// </summary>
        private void RunAutoPlaySequence()
        {
            switch (_playOptions)
            {
                case PlayOptions.Play: return;
                case PlayOptions.PlayAll:
                    MoveToNextFile();
                    break;
                case PlayOptions.Shuffle:
                    MoveToNextRandomFile();
                    break;
                default:
                    return;
            }

            // Play the next item.
            PlayMediaFullScreen(_playOptions);
        }

        /// <summary>
        /// Moves to next file in play all mode.
        /// </summary>
        private void MoveToNextFile()
        {
            // Emulate the keydown key press on the list box.
            PageModel.GetInstance.MoveDown();
            PageModel.GetInstance.BindItems();
            PageModel.GetInstance.UpdatePaginationLabels();

            // Keep presing down key if item is not a file.
            while (PageModel.GetInstance.MenuEntityModel.SelectedMenuItemModel.FilePath == null)
            {
                MoveToNextFile();
                Thread.Sleep(20);
            }
        }

        /// <summary>
        /// Moves to next random file in shuffle mode.
        /// </summary>
        private void MoveToNextRandomFile()
        {
            // If there is only 1 file then just play again.
            if (PageModel.GetInstance.MenuEntityModel.ItemCount == 1)
            {
                return;
            }

            // New random genrator.
            Random rnd = new Random((int)DateTime.Now.Ticks);

            // Get random number of moves
            int moves = rnd.Next(1, PageModel.GetInstance.MenuEntityModel.ItemCount - 2);

            for (int i = 0; i < moves; i++)
            {
                PageModel.GetInstance.MoveDown();
                PageModel.GetInstance.BindItems();
                PageModel.GetInstance.UpdatePaginationLabels();
                Thread.Sleep(10);
            }

            // If the selected is not a file then just move to next file like play all mode.
            if (PageModel.GetInstance.MenuEntityModel.SelectedMenuItemModel.FilePath != null)
            {
                MoveToNextFile();
            }
        }

        #endregion Auto Play Methods

        /// <summary>
        /// _medias the player_ play state change.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void _mediaPlayer_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            switch (e.newState)
            {
                case 0: break;   // Undefined
                case 1:    // Stopped
                    Task.Run(() => 
                    {
                        Thread.Sleep(1000);
                        if (_playOptions == PlayOptions.PlayAll || _playOptions == PlayOptions.Shuffle)
                        {
                            Application.Current.Dispatcher.Invoke(RunAutoPlaySequence);
                        }
                    });
                    break;
                case 2: break;   // Paused 
                case 3: break;   // Playing
                case 4: break;   // ScanForward
                case 5: break;   // ScanReverse
                case 6: break;   // Buffering
                case 7: break;   // Waiting
                case 8:        // MediaEnded
                    break;
                case 9: break;   // Transitioning
                case 10: break;   // Ready
                case 11: break;   // Reconnecting
                case 12: break;   // Last
                default: break;   // unknown state
            }
        }

        /// <summary>
        /// Handles Windows media player key strokes
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="T:System.Windows.Input.KeyEventArgs" /> instance containing the event data.</param>
        public void HandleKeyDown(object sender, KeyEventArgs e)
        {
            if (BlockKeyPresses)
            {
                //e.IsInputKey = false;
                return;
            }
            BlockKeyPresses = true;

            // stop
            if (e.Key == Key.MediaStop || e.Key == Key.V)
            {
                //UserPressedStop = true;
                _playOptions = PlayOptions.Play;
                //IsFullScreen = false;
                //IsPlaying = false;
                MediaPlayer.Ctlcontrols.stop();
                MediaPlayer.fullScreen = false;
                Application.Current.MainWindow.Focus();
            }

            // Skip forwards
            if (e.Key == Key.MediaNextTrack || e.Key == Key.N)
            {
                SkipForward(40);
            }

            // skip backwards
            if (e.Key == Key.P || e.Key == (Key)177)
            {
                SkipBackwards(40);
            }

            // Skip forwards (long)
            if (e.Key == Key.Right)
            {
                SkipForward(700);
            }

            // skip backwards (long)
            if (e.Key == Key.Left)
            {
                SkipBackwards(700);
            }

            // play, pause. note: this is required both here and in playPanel 
            if (e.Key == Key.MediaPlayPause || e.Key == (Key)179 || e.Key == Key.Play)
            {
                if (MediaPlayer.playState == WMPLib.WMPPlayState.wmppsPaused)
                {
                    MediaPlayer.Ctlcontrols.play();
                }
                else
                {
                    MediaPlayer.Ctlcontrols.pause();
                }
            }

            BlockKeyPresses = false;
        }
    }
}





