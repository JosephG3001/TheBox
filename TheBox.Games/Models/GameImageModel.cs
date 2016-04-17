using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using TheBox.Common.Models;
using System.Threading;
using System.Reflection;

namespace TheBox.Games.Models
{
    /// <summary>
    /// GameImageModel
    /// </summary>
    public class GameImageModel
    {
        #region Singleton

        /// <summary>
        /// The _instance
        /// </summary>
        private static GameImageModel _instance;

        /// <summary>
        /// Gets the get instance.
        /// </summary>
        public static GameImageModel GetInstance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameImageModel();
                }
                return _instance;
            }
        }

        #endregion Singleton

        /// <summary>
        /// Initializes a new instance of the <see cref="GameImageModel"/> class.
        /// </summary>
        private GameImageModel()
        {
            _instance = this;

            LoadImageOffsets();
        }

        /// <summary>
        /// The image offsets
        /// </summary>
        public Dictionary<string, int> ImageOffsets = new Dictionary<string, int>();

        /// <summary>
        /// Gets the _game offsets path.
        /// </summary>
        private string _gameOffsetsPath
        {
            get
            {
                return Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath) + "\\game_image_offsets.ini";
            }
        }

        /// <summary>
        /// The _tokens
        /// </summary>
        private List<CancellationTokenSource> _tokens = new List<CancellationTokenSource>();

        /// <summary>
        /// The _image lock
        /// </summary>
        private object _imageLock = new object();

        /// <summary>
        /// Shows the game image by searching Bing.
        /// </summary>
        public void ShowGameImage()
        {
            lock (_imageLock)
            {
                if (_tokens.Count > 0)
                {
                    _tokens.ForEach(m => m.Cancel());
                    _tokens.Clear();
                }

                CancellationTokenSource ts = new CancellationTokenSource();
                CancellationToken ct = ts.Token;
                _tokens.Add(ts);

                Task.Run(() =>
                {
                    // create the request url
                    string consoleName = PageModel.GetInstance.MenuEntityModels[0].SelectedMenuItemModel.DisplayText.RemoveCommas();
                    string gameName = PageModel.GetInstance.SelectedMenuItemModel.DisplayText.RemoveCommas();
                    string url = string.Format("https://www.bing.com/images/search?q={0}&go=Submit+Query&qs=bs&form=QBIR", string.Join(" ", consoleName, RemoveBadWordsFromGameName(gameName)/*, "-cd -disc -cart -cartridge"*/));

                    // cancel?
                    if (ct.IsCancellationRequested)
                    {
                        return;
                    }

                    // get the response
                    string data = "";
                    WebRequest request = HttpWebRequest.Create(url);
                    WebResponse response = request.GetResponse();
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            data = reader.ReadToEnd();
                        }
                    }

                    // cancel?
                    if (ct.IsCancellationRequested)
                    {
                        return;
                    }

                    // get the image
                    MatchCollection matches = Regex.Matches(data, "<img.*?src=\"http(.*?)/>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                    List<string> imageUrls = new List<string>();
                    foreach (Match match in matches)
                    {
                        string rurl = Regex.Match(match.Value, "http(.*?)\"").Value;
                        imageUrls.Add(rurl.Replace("\"", ""));
                    }

                    // cancel?
                    if (ct.IsCancellationRequested)
                    {
                        return;
                    }

                    // show image on screen
                    int offSet = 0;
                    if (ImageOffsets.ContainsKey(consoleName + "_" + gameName))
                    {
                        offSet = ImageOffsets[consoleName + "_" + gameName];
                    }

                    AutoResetEvent e = new AutoResetEvent(false);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        GameControlModel.GetInstance.CurrentGameImage = imageUrls[offSet];
                        e.Set();
                    });
                    e.WaitOne();

                    lock (_imageLock)
                    {
                        _tokens.Remove(ts);
                    }
                }, ct);
            }
        }

        private string RemoveBadWordsFromGameName(string gameName)
        {
            gameName = gameName.Replace("CD1", "");
            gameName = gameName.Replace("CD2", "");
            gameName = gameName.Replace("CD3", "");
            gameName = gameName.Replace("CD4", "");
            gameName = gameName.Replace("Disc 1", "");
            gameName = gameName.Replace("Disc 2", "");
            gameName = gameName.Replace("Disc 3", "");
            gameName = gameName.Replace("Disc 4", "");
            gameName = gameName.Replace(".img", "");
            return gameName;
        }

        /// <summary>
        /// Loads the image offsets.
        /// </summary>
        private void LoadImageOffsets()
        {
            // load image offsets
            if (File.Exists(_gameOffsetsPath))
            {
                using (StreamReader reader = new StreamReader(_gameOffsetsPath))
                {
                    while (!reader.EndOfStream)
                    {
                        try
                        {
                            string[] game = reader.ReadLine().Split(',').ToArray();
                            ImageOffsets.Add(game[0], Convert.ToInt32(game[1]));
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Saves the image offsets.
        /// </summary>
        public void SaveImageOffsets()
        {
            using (StreamWriter writer = new StreamWriter(_gameOffsetsPath))
            {
                foreach (var offset in this.ImageOffsets)
                {
                    writer.WriteLine(offset.Key + "," + offset.Value);
                }
            }
        }
    }
}
