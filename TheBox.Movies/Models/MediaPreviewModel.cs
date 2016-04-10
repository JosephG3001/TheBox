using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using TheBox.Common.Models;
using TheBox.Movies.FileCache;

namespace TheBox.Movies.Models
{
    /// <summary>
    /// MediaPreviewModel
    /// </summary>
    public class MediaPreviewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MediaPreviewModel"/> class.
        /// </summary>
        /// <param name="mediaPlayer">The media player.</param>
        public MediaPreviewModel(AxWMPLib.AxWindowsMediaPlayer mediaPlayer)
        {
            this._mediaPlayer = mediaPlayer;
        }

        /// <summary>
        /// The _media player
        /// </summary>
        private AxWMPLib.AxWindowsMediaPlayer _mediaPlayer;

        /// <summary>
        /// The _previewlock - to ensure that one thread can preview
        /// </summary>
        private object _previewlock = new object();

        /// <summary>
        /// The tokens - used to cancel video preview tasks by other threads
        /// </summary>
        private List<CancellationTokenSource> Tokens = new List<CancellationTokenSource>();

        /// <summary>
        /// Previews the media of the current selected menu item.
        /// </summary>
        /// <remarks>
        /// Implemented in such as way that only one thread should be loading a video file.
        /// Subsequent threads will cancel all previous threads as we don;t want to thrash the hard drive.
        /// </remarks>
        public void PreviewMedia(bool stopPreview)
        {
            lock (_previewlock)
            {
                CancellationTokenSource ts = new CancellationTokenSource();
                CancellationToken ct = ts.Token;

                if (Tokens.Count > 0)
                {
                    Tokens.ForEach(m => m.Cancel());
                    Tokens.Clear();
                }

                if (stopPreview)
                {
                    try
                    {
                        _mediaPlayer.Ctlcontrols.stop();
                    }
                    catch
                    {
                        int breakpoint = 1;
                    }
                    return;
                }
                Tokens.Add(ts);

                Task t = Task.Run(() =>
                {
                    // no selected item?
                    if (PageModel.GetInstance.SelectedMenuItemModel == null)
                    {
                        return;
                    }

                    // no file?
                    string filePath = PageModel.GetInstance.SelectedMenuItemModel.FilePath;
                    if (filePath == null)
                    {
                        return;
                    }

                    // get file extension
                    string ext = "*" + System.IO.Path.GetExtension(filePath).ToLower();

                    if (FileCacheManager.VideoFileExtensions.Contains(ext))
                    {
                        if (CheckPreviewCancel(ct))
                        {
                            return;
                        }

                        // load file
                        AutoResetEvent e = new AutoResetEvent(false);
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            _mediaPlayer.URL = filePath;
                            e.Set();
                        });
                        e.WaitOne();

                        // set to position
                        int retries = 300;
                        double length = 0;
                        while (retries > 0 && length == 0)
                        {
                            if (CheckPreviewCancel(ct))
                            {
                                return;
                            }

                            try
                            {
                                length = Convert.ToDouble(_mediaPlayer.currentMedia.duration);
                                if (length > 0)
                                {
                                    _mediaPlayer.Ctlcontrols.currentPosition = length / 4;
                                }
                            }
                            catch { }
                        }

                        lock (_previewlock)
                        {
                            // delete the token
                            Tokens.Remove(ts);
                        }
                    }
                }, ct);
            }
        }

        /// <summary>
        /// Cancels the previews.
        /// </summary>
        public void CancelPreviews()
        {
            lock (_previewlock)
            {
                if (Tokens.Count > 0)
                {
                    Tokens.ForEach(m => m.Cancel());
                    Tokens.Clear();
                }
            }
        }

        /// <summary>
        /// Checks whether to cancel the preview as user might be holding down the down arrow triggering many previews.
        /// </summary>
        /// <param name="ct">The ct.</param>
        /// <returns></returns>
        private bool CheckPreviewCancel(CancellationToken ct)
        {
            Thread.Sleep(100);
            if (ct.IsCancellationRequested)
            {
                return true;
            }
            return false;
        }
    }
}
