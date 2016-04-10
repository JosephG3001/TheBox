using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using TheBox.Common;

namespace TheBox.Movies.FileCache
{
    /// <summary>
    /// FileCacheManager
    /// </summary>
    public class FileCacheManager
    {
        #region Events

        public event EventHandler CacheLoadingStarted;
        public event EventHandler CacheLoadingComplete;

        /// <summary>
        /// Called when [cache loading started].
        /// </summary>
        public void OnCacheLoadingStarted()
        {
            var handler = CacheLoadingStarted;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Called when [cache loading complete].
        /// </summary>
        public void OnCacheLoadingComplete()
        {
            var handler = CacheLoadingComplete;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        #endregion Events

        /// <summary>
        /// Gets or sets the file cache entities.
        /// </summary>
        public List<FileCacheEntity> FileCacheEntities
        {
            get; set;
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="FileCacheManager"/> is initialised.
        /// </summary>
        public bool Initialised
        {
            get; private set;
        }

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCacheManager" /> class.
        /// </summary>
        public FileCacheManager()
        {
            // new cache list
            FileCacheEntities = new List<FileCacheEntity>();
            Initialised = false;
        }

        #endregion Constructor

        /// <summary>
        /// Initialises the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        public async Task Initialise(string path)
        {
            // load files
            await LoadFileCache(path);
            Initialised = true;
        }

        /// <summary>
        /// Loads the file cache.
        /// </summary>
        /// <param name="path">The path.</param>
        private async Task LoadFileCache(string path)
        {
            await Task.Run(() =>
            {
                try
                {
                    // notify start
                    Application.Current.Dispatcher.Invoke(OnCacheLoadingStarted);

                    if (!string.IsNullOrEmpty(path) && path != @"C:\")
                    {
                        GetFilesFromDirectory(path);

                        // build cache arrays
                        Parallel.ForEach<FileCacheEntity>(this.FileCacheEntities, file => 
                        {
                            // Split titles into parts.
                            file.FileParts = RemoveBadCharacters(Path.GetFileNameWithoutExtension(file.FullPathAndName)).ToLower().Split(' ', ',', '.', '-').ToArray();

                            // Strip bad words out of the arrays
                            file.FileParts = file.FileParts.Where(m => !WordsToIgnore.Contains(m.ToLower().SafeTrim())).ToArray();

                            // Distinct the words.
                            file.FileParts = file.FileParts.Distinct().ToArray();
                        });
                    }

                    // notify complete
                    Application.Current.Dispatcher.Invoke(OnCacheLoadingComplete);
                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        throw ex;
                    });
                }
            });
        }

        /// <summary>
        /// Gets the files from directory.
        /// </summary>
        /// <param name="dir">The dir.</param>
        private void GetFilesFromDirectory(string dir)
        {
            // files for the current folder only
            List<string> files = new List<string>();

            try
            {
                // ignore the folder?
                if (File.Exists(Path.Combine(dir, "tv.ignore")))
                {
                    return;
                }

                // add files only
                files = Directory.GetFiles(dir).ToList();
            }
            catch
            {
                return;
            }

            // Add files to list.
            foreach (var file in files)
            {
                string extension = "*" + Path.GetExtension(file);

                if (extension != "*." && VideoFileExtensions.Contains(extension))
                {
                    FileCacheEntities.Add(new FileCacheEntity() {
                        FileNameOnly = Path.GetFileNameWithoutExtension(file),
                        FullPathAndName = file
                    });
                }
            }

            // Do same for child folders.
            foreach (var childDir in Directory.GetDirectories(dir))
            {
                GetFilesFromDirectory(childDir);
            }
        }

        /// <summary>
        /// Removes the bad characters.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        private string RemoveBadCharacters(string input)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9 -]");
            return rgx.Replace(input, " ");
        }

        /// <summary>
        /// The video extensions
        /// </summary>
        public static string[] VideoFileExtensions = { "*.avi", "*.mpg", "*.mp4", "*.mpeg", "*.mp3", "*.wav", "*.flv", "*.mkv", "*.m4v" };

        /// <summary>
        /// The words to ignore
        /// </summary>
        public string[] WordsToIgnore = new string[]
        {
            "the", "and", "is", "of", "n", "n'", "1080p", "720p", "a", "to", ""
        };

        /// <summary>
        /// The programmes to ignore
        /// </summary>
        public string[] ProgrammesToIgnore = new string[]
        {
            "news",
            "weather",
            "business",
            "documentary",
            "close",
            "off air",
            "friends",
            "obese: a year to save my life usa",
            "house",
            "play",
            "tennis",
            "fashion",
            "enjoying everyday life",
            "healthy life",
            "the private life of plants"
        };
    }
}
