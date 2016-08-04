using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using TheBox.Common.Menu;
using TheBox.Common.Models;
using TheBox.Movies.Models;
using TheBox.TVScheduleCommon;
using TheBox.TVScheduleCommon.PublicObjects;
using System.Reflection;
using System.IO;
using TheBox.Common;
using TheBox.Movies.FileCache;
using System.Text.RegularExpressions;
using System.Threading;

namespace TheBox.Movies.TVSchedule
{
    /// <summary>
    /// TVScheduleManager
    /// </summary>
    public class TVScheduleManager
    {
        /// <summary>
        /// The _component name
        /// </summary>
        private string _componentName;

        /// <summary>
        /// The _scanning for matches modal
        /// </summary>
        private ScanningForMatchesModal _scanningForMatchesModal;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TVScheduleManager"/> class.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        public TVScheduleManager(Dispatcher dispatcher, string componentName)
        {
            this.Dispatcher = dispatcher;
            this._componentName = componentName;

            // See if a plugin is available
            LoadTVSchedulePlugin();
        }

        #endregion Constructors

        #region Public properties

        /// <summary>
        /// Gets the tv schedule service.
        /// </summary>
        public ITVScheduleService TVScheduleService
        {
            get; set;
        }

        /// <summary>
        /// Gets the dispatcher.
        /// </summary>
        public Dispatcher Dispatcher
        {
            get; private set;
        }

        /// <summary>
        /// Gets or sets the tv schedule cached results.
        /// </summary>
        public List<TVProviderCacheEntity> TVProviderCacheEntities
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the schedule.
        /// </summary>
        public ITVSchedule Schedule
        {
            get; set;
        }

        #endregion Public properties

        /// <summary>
        /// Loads the tv schedule plugin.
        /// </summary>
        private void LoadTVSchedulePlugin()
        {
            // get the path of this movie plguin DLL (not the TheBox.exe)
            DirectoryInfo dllPath = new DirectoryInfo(new Uri(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)).LocalPath);

            // get the DLL files
            List<FileInfo> files = dllPath.GetFiles().Where(m => m.Extension.ToLower() == ".dll").ToList();

            // loop dll's to find ITVScheduleService
            foreach (FileInfo file in files)
            {
                try
                {
                    Type[] types = Assembly.LoadFile(file.FullName).GetTypes();
                    Type ITVScheduleService = typeof(ITVScheduleService);

                    foreach (Type type in types)
                    {
                        if (type != ITVScheduleService && ITVScheduleService.IsAssignableFrom(type))
                        {
                            this.TVScheduleService = (ITVScheduleService)Activator.CreateInstance(type);
                            return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Navigates to tv schedules menu.
        /// </summary>
        public void NavigateToTVSchedules()
        {
            // prepare list of menuItems for the new menu we're going to navigate to.
            List<MenuItemModel> menuItems = new List<MenuItemModel>();

            // Show the "Downloading TV Schedule" loading modal
            DownloadingTVScheduleModal modal = new DownloadingTVScheduleModal();
            ModalModel.GetInstance.ModalUserControl = modal;

            Task.Run(() =>
            {
                try
                {
                    // download schedule from TVSchedule plugin
                    this.Schedule = TVScheduleService.GetSchedule();

                }
                catch (Exception ex)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        modal.SetFail(ex.Message);
                    });
                    return;
                }

                this.Dispatcher.Invoke(() =>
                {
                    if (this.Schedule.Providers.Count == 0)
                    {
                        // show "No Items to display"
                        menuItems.Add(new MenuItemModel() { DisplayText = "No TV Providers Found..", IsSelected = true, ParentSelected = true });
                    }
                    else
                    {
                        foreach (var item in this.Schedule.Providers)
                        {
                            // create a new menu item for each of the TV providers found
                            menuItems.Add(new MenuItemModel()
                            {
                                DisplayText = item.ProviderName,
                                IsSelected = false,
                                ParentSelected = true,
                                IsVisible = true,
                                RelayCommand = new RelayCommand(() =>
                                {
                                    NavigateToChannelListings(item);
                                })
                            });
                        }

                        // Select the first menuItem on the list
                        menuItems.First().IsSelected = true;
                    }

                    // navigate to the new menuEntity
                    PageModel.GetInstance.NavigateForwards(menuItems);

                    // remove the "Downloading TV Schedule" modal
                    ModalModel.GetInstance.ModalUserControl = null;

                    // update the bread crumbs label
                    PageModel.GetInstance.DoBreadCrumbs(this._componentName);
                });
            });
        }

        /// <summary>
        /// Navigates to channel listings.
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        public void NavigateToChannelListings(ITVProvider provider)
        {
            // prepare list of menuItems for the new menu we're going to navigate to.
            List<MenuItemModel> menuItems = new List<MenuItemModel>();

            // Show the "Scanning for Matches" loading modal
            ModalModel.GetInstance.ModalUserControl = new ScanningForMatchesModal();
            _scanningForMatchesModal = ModalModel.GetInstance.ModalUserControl as ScanningForMatchesModal;
            _scanningForMatchesModal.progressBar1.Min = 0;
            _scanningForMatchesModal.progressBar1.Max = provider.Channels.Count;
            _scanningForMatchesModal.progressBar1.Value = 0;

            Task.Run(() =>
            {
                // Create new list?
                if (this.TVProviderCacheEntities == null)
                {
                    this.TVProviderCacheEntities = new List<TVProviderCacheEntity>();
                }

                // Do we have cached result we can use?
                TVProviderCacheEntity providerCache = this.TVProviderCacheEntities.Where(m => m.providerName == provider.ProviderName).FirstOrDefault();

                // Check if we have a cache and that the cache is not old (earlier than the TVSchedule object)
                if (providerCache == null || providerCache.LastScanTime < this.Schedule.ScheduleDate)
                {
                    // Create a new cache for the provider
                    providerCache = new TVProviderCacheEntity();
                    providerCache.providerName = provider.ProviderName;
                    providerCache.MenuItemModels = ScanForMatches(provider);
                    providerCache.LastScanTime = DateTime.Now;

                    // store provider cache in the cache
                    this.TVProviderCacheEntities.Add(providerCache);

                    menuItems = providerCache.MenuItemModels;
                }
                else
                {
                    // Cache is good so just navigate to the menuItemModels
                    menuItems = providerCache.MenuItemModels;
                }

                this.Dispatcher.Invoke(() =>
                {
                    // navigate to the new menuEntity
                    PageModel.GetInstance.NavigateForwards(menuItems);

                    // remove the "Scanning for Matches" modal
                    ModalModel.GetInstance.ModalUserControl = null;

                    // update the bread crumbs label
                    PageModel.GetInstance.DoBreadCrumbs(this._componentName);
                });
            });
        }

        private object obj = new object();

        /// <summary>
        /// Scans for matches for a provider.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <returns></returns>
        private List<MenuItemModel> ScanForMatches(ITVProvider provider)
        {
            // Cap the channel name lengths
            Parallel.ForEach<IChannel>(provider.Channels, channel =>
            {
                // Cap the name length?
                if (channel.Name.Length > 19)
                {
                    channel.Name = channel.Name.Substring(0, 18) + "..";
                }
            });

            // Get longest channel name for the padright
            int longestChannelName = provider.Channels
            .Select(m => m.Name)
            .OrderByDescending(m => m.Length).First().Length;

            int longestChannelNumber = provider.Channels
                .Select(m => m.ChannelNumber)
                .OrderByDescending(m => m.Length).First().Length;

            // prepare results
            List<MenuItemModel> result = new List<MenuItemModel>();

            Parallel.ForEach<IChannel>(provider.Channels, channel =>
            {
                Parallel.ForEach<IProgramme>(channel.Programmes, programme =>
                {
                    // Attempt to get files that match the TV show name.
                    List<MenuItemModel> matches = GetMatchesForProgramme(programme);

                    if (matches.Count > 0)
                    {
                        lock (obj)
                        {
                            result.Add(new MenuItemModel
                            {
                                DisplayText =
                                    channel.ChannelNumber.PadRight(longestChannelNumber + 2) +
                                    channel.Name.PadRight(longestChannelName + 2) +
                                    Convert.ToDateTime(programme.ShowingTime).ToShortTimeString().PadRight(8) +
                                    programme.ProgramName,
                                Tag = channel.ChannelNumber,
                                IsSelected = false,
                                FilePath = null,
                                ParentSelected = true,
                                IsVisible = true,
                                RelayCommand = new RelayCommand(() =>
                                {

                                    // navigate to the list of files
                                    PageModel.GetInstance.NavigateForwards(matches);

                                    // update the bread crumbs label
                                    PageModel.GetInstance.DoBreadCrumbs(this._componentName);
                                })
                            });
                        }
                    }
                });

                this.Dispatcher.Invoke(() =>
                {
                    _scanningForMatchesModal.progressBar1.Value++;
                });
            });

            // Show 100% progress for a 10th of a second
            AutoResetEvent e = new AutoResetEvent(false);
            this.Dispatcher.Invoke(() =>
            {
                _scanningForMatchesModal.progressBar1.Value = _scanningForMatchesModal.progressBar1.Max;
                e.Set();
            });
            e.WaitOne();
            Thread.Sleep(100);

            // No matches found so add a blank button stating "No Matches found"
            if (result.Count == 0)
            {
                result.Add(new MenuItemModel()
                {
                    DisplayText = "No matches found..",
                    IsSelected = true,
                    IsVisible = true,
                    ParentSelected = true,
                    RelayCommand = new RelayCommand(() => { })
                });
                return result;
            }
            else
            {
                // return the results sorted by the channel number
                return result.OrderBy(m => Convert.ToInt32(m.Tag)).ToList();
            }
        }

        /// <summary>
        /// Gets the matches.
        /// </summary>
        /// <param name="programme">The programme.</param>
        /// <returns></returns>
        public List<MenuItemModel> GetMatchesForProgramme(IProgramme Programme)
        {
            // Prepare result.
            List<MenuItemModel> matches = new List<MenuItemModel>();

            // Ignore one word titles such as "news".
            if (Programme.ProgramName == null ||
                MovieControlModel.GetInstance.FileCacheManager.ProgrammesToIgnore.Contains(Programme.ProgramName.ToLower().SafeTrim()))
            {
                return matches;
            }

            if (Programme.ProgramName.ToLower().Contains("james may"))
            {
                int breakpoint = 1;
            }

            string[] programmeParts = GetPartsForProgrammeName(Programme);

            // Loop local files.
            Parallel.ForEach<FileCacheEntity>(MovieControlModel.GetInstance.FileCacheManager.FileCacheEntities, fileEntity =>
            {
                int matchCount = 0;

                foreach (string f in fileEntity.FileParts)
                {
                    foreach (string p in programmeParts)
                    {
                        if (f.ToLower().SafeTrim() == p.ToLower().SafeTrim())
                        {
                            matchCount++;
                        }
                    }
                }

                // Accept the file?
                if (matchCount >= programmeParts.Count() || matchCount >= fileEntity.FileParts.Count())
                {
                    lock (obj)
                    {
                        matches.Add(new MenuItemModel()
                        {
                            DisplayText = Path.GetFileNameWithoutExtension(fileEntity.FullPathAndName),
                            FilePath = fileEntity.FullPathAndName,
                            IsVisible = true,
                            ParentSelected = true,
                            IsSelected = false,
                            RelayCommand = new RelayCommand(() =>
                            {

                                // move focus to the mini playpanel
                                MovieControlModel.GetInstance.PlayOptionsVisible = true;
                                PageModel.GetInstance.UnSelectPageModel();
                            })
                        });
                    }
                }
            });

            // Return result.
            return matches;
        }

        /// <summary>
        /// Gets the name of the parts for programme.
        /// </summary>
        /// <param name="programme">The programme.</param>
        /// <returns></returns>
        private string[] GetPartsForProgrammeName(IProgramme programme)
        {
            // If the TVSchedule plugin already did this then use there results.
            if (programme.ProgrammeNameParts != null)
            {
                return programme.ProgrammeNameParts;
            }

            // Remove the ' because regex isnt picking it up..
            programme.ProgramName = programme.ProgramName.Replace("'", "");

            // Split titles into parts.
            string[] programmeParts = RemoveBadCharacters(programme.ProgramName).ToLower().Split(' ', ',', '.', '-').ToArray();

            // Strip bad words out of the arrays
            programmeParts = programmeParts.Where(m => !MovieControlModel.GetInstance.FileCacheManager.WordsToIgnore.Contains(m.ToLower().SafeTrim())).ToArray();

            // Distinct the words.
            return programmeParts.Distinct().ToArray();
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
    }
}
