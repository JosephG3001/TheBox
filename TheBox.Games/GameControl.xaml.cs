using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TheBox.Common;
using TheBox.Common.Menu;
using TheBox.Common.Models;
using TheBox.Games.Models;
using TheBox.Games.Settings;
using TheBox.Games.Modals;
using System.IO;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using System.Reflection;

namespace TheBox.Games
{
    /// <summary>
    /// Interaction logic for GameControl.xaml
    /// </summary>
    public partial class GameControl : UserControl, IBoxComponent, IBoxKeyboardControl
    {
        #region Singleton

        /// <summary>
        /// The _instance
        /// </summary>
        private static GameControl _instance;

        /// <summary>
        /// Gets the get instance.
        /// </summary>
        public static GameControl GetInstance
        {
            get { return _instance; }
        }

        #endregion Singleton

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GameControl"/> class.
        /// </summary>
        public GameControl()
        {
            // set the model to page model
            this.DataContext = PageModel.GetInstance;

            // set this instance as the singleton
            _instance = this;

            // initialise the GameControlModel singleton.
            new GameControlModel(this.Dispatcher, this.ComponentName);

            InitializeComponent();

            // set this gameControl's datacontext to the singleton PageModel
            this.ImageGrid.DataContext = GameControlModel.GetInstance;
            this.GameImageGrid.DataContext = GameControlModel.GetInstance;
            this.EllipseGrid.DataContext = GameControlModel.GetInstance;
        }

        #endregion Constructors

        #region IBoxComponent implementation

        /// <summary>
        /// Gets the activate command.  The activate command will be a delegate that should run when the component is activated.
        /// </summary>
        public void ActivateComponent()
        {
            PageModel.GetInstance.DoBreadCrumbs(this.ComponentName);

            // check if we have emulators configured
            if (GameControlModel.GetInstance.GameSettingsManager.EmulatorSettings.EmulatorSettingList == null ||
                GameControlModel.GetInstance.GameSettingsManager.EmulatorSettings.EmulatorSettingList.Count() == 0)
            {
                ModalModel.GetInstance.ModalUserControl = new NoEmulatorsConfiguredModal();
                return;
            }

            // ** display list of emulators

            // new list of menuitems
            List<MenuItemModel> menuItems = new List<MenuItemModel>();

            // get configured list of emulators
            List<EmulatorSetting> settings = GameControlModel.GetInstance.GameSettingsManager.EmulatorSettings.EmulatorSettingList.ToList();

            foreach (EmulatorSetting setting in settings)
            {
                menuItems.Add(new MenuItemModel()
                {
                    DisplayText = setting.EmulatatedSystemName,
                    IsSelected = false,
                    IsVisible = true,
                    ParentSelected = true,
                    Tag = setting,
                    RelayCommand = new RelayCommand(() =>
                    {
                        NavigateToRoms(setting);
                    })
                });
            }

            // select the first item
            menuItems.First().IsSelected = true;

            // navigate to the new emulator list
            PageModel.GetInstance.NavigateForwards(menuItems);

            ShowSystemImage();

            PageModel.GetInstance.DoBreadCrumbs(this.ComponentName);
        }

        /// <summary>
        /// Navigates to settings.
        /// </summary>
        /// <param name="previousMenu"></param>
        /// <returns></returns>
        public void NavigateToSettings()
        {
            // menu items for new menu entity
            List<MenuItemModel> menuItems = new List<MenuItemModel>();

            // add the "Add Emulator" menu item
            menuItems.Add(new MenuItemModel()
            {
                DisplayText = "Add New Emulator",
                ParentSelected = true,
                RelayCommand = new RelayCommand(() =>
                {
                    ModalModel.GetInstance.ModalUserControl = new EmulatorSettingModal();
                    PageModel.GetInstance.UnSelectPageModel();
                })
            });

            // add menu items for each of the saved emulator settings
            if (GameControlModel.GetInstance.GameSettingsManager.EmulatorSettings.EmulatorSettingList != null)
            {
                foreach (var setting in GameControlModel.GetInstance.GameSettingsManager.EmulatorSettings.EmulatorSettingList)
                {
                    menuItems.Add(new MenuItemModel()
                    {
                        DisplayText = setting.EmulatatedSystemName,
                        ParentSelected = true,
                        RelayCommand = new RelayCommand(() =>
                        {
                            ModalModel.GetInstance.ModalUserControl = new EmulatorSettingModal();
                            PageModel.GetInstance.UnSelectPageModel();
                        })
                    });
                }
            }

            // navigate to new menu
            PageModel.GetInstance.NavigateForwards(menuItems);
        }

        /// <summary>
        /// Gets the appbar_icon.
        /// </summary>
        /// <remarks>
        /// See http://modernuiicons.com/ for icons
        /// </remarks>
        public string appbar_icon
        {
            get { return "appbar_controller_xbox"; }
        }

        /// <summary>
        /// Gets the name of the component.
        /// </summary>
        public string ComponentName
        {
            get { return "Games"; }
        }

        /// <summary>
        /// Gets the underlying user control.
        /// </summary>
        /// <returns></returns>
        public UserControl GetUserControl()
        {
            return this;
        }

        #endregion IBoxComponent implementation

        #region IBoxKeyboardControl implementation

        /// <summary>
        /// Handles the key down.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="T:System.Windows.Input.KeyEventArgs" /> instance containing the event data.</param>
        public void HandleKeyDown(object sender, KeyEventArgs e)
        {
            // backspace
            if (e.Key == Key.Back)
            {
                PageModel.GetInstance.NavigateBackwards();
                GameControlModel.GetInstance.CurrentGameImage = null;
            }

            // up
            if (e.Key == Key.Up)
            {
                PageModel.GetInstance.MoveUp();
                PageModel.GetInstance.BindItems();
                PageModel.GetInstance.UpdatePaginationLabels();

                if (PageModel.GetInstance.SelectedMenuItemModel.FilePath == null)
                {
                    ShowSystemImage();
                }
                else
                {
                    GameImageModel.GetInstance.ShowGameImage(true);
                }
            }

            // down
            if (e.Key == Key.Down)
            {
                PageModel.GetInstance.MoveDown();
                PageModel.GetInstance.BindItems();
                PageModel.GetInstance.UpdatePaginationLabels();

                if (PageModel.GetInstance.SelectedMenuItemModel.FilePath == null)
                {
                    ShowSystemImage();
                }
                else
                {
                    GameImageModel.GetInstance.ShowGameImage(true);
                }
            }

            // left
            if (e.Key == Key.Left)
            {
                MenuItemModel selectedItemModel = PageModel.GetInstance.VisibleMenuItemModels.Where(m => m.IsSelected).First();
                if (selectedItemModel.FilePath != null)
                {
                    // select the previuos image from bing
                    string consoleName = PageModel.GetInstance.MenuEntityModels[0].SelectedMenuItemModel.DisplayText.RemoveCommas();
                    string gameName = PageModel.GetInstance.SelectedMenuItemModel.DisplayText.RemoveCommas();

                    if (!GameImageModel.GetInstance.ImageOffsets.ContainsKey(consoleName + "_" + gameName))
                    {
                        // no need to do anything
                        return;                    
                    }

                    if (GameImageModel.GetInstance.ImageOffsets[consoleName + "_" + gameName] > 0)
                    {
                        GameImageModel.GetInstance.ImageOffsets[consoleName + "_" + gameName]--;
                        GameImageModel.GetInstance.SaveImageOffsets();
                        GameImageModel.GetInstance.ShowGameImage(false);
                    }
                }
            }

            // right
            if (e.Key == Key.Right)
            {
                MenuItemModel selectedItemModel = PageModel.GetInstance.VisibleMenuItemModels.Where(m => m.IsSelected).First();
                if (selectedItemModel.FilePath != null)
                {
                    // select the next image from bing
                    string consoleName = PageModel.GetInstance.MenuEntityModels[0].SelectedMenuItemModel.DisplayText.RemoveCommas();
                    string gameName = PageModel.GetInstance.SelectedMenuItemModel.DisplayText.RemoveCommas();

                    if (!GameImageModel.GetInstance.ImageOffsets.ContainsKey(consoleName + "_" + gameName))
                    {
                        GameImageModel.GetInstance.ImageOffsets.Add(consoleName + "_" + gameName, 1);
                    }
                    else
                    {
                        GameImageModel.GetInstance.ImageOffsets[consoleName + "_" + gameName]++;
                    }
                    GameImageModel.GetInstance.SaveImageOffsets();
                    GameImageModel.GetInstance.ShowGameImage(false);
                }
            }

            // enter
            if (e.Key == Key.Enter)
            {
                MenuItemModel selectedItemModel = PageModel.GetInstance.VisibleMenuItemModels.Where(m => m.IsSelected).First();
                if (selectedItemModel.FilePath == null)
                {
                    selectedItemModel.RelayCommand.action();

                    // diving into rom list to show the first game cover
                    GameImageModel.GetInstance.ShowGameImage(true);
                }
                else
                {
                    selectedItemModel.RelayCommand.action();
                }
            }
            PageModel.GetInstance.DoBreadCrumbs(this.ComponentName);
        }



        #endregion IBoxKeyboardControl implementation


        /// <summary>
        /// Shows the system image.
        /// </summary>
        private void ShowSystemImage()
        {
            // Show the game system image
            GameControlModel.GetInstance.CurrentEmulatorImage = ((EmulatorSetting)PageModel.GetInstance.SelectedMenuItemModel.Tag).ConsoleImagePath;
        }

        /// <summary>
        /// Navigates to roms.
        /// </summary>
        /// <param name="setting">The setting.</param>
        private void NavigateToRoms(EmulatorSetting setting)
        {
            // menu items for new menu entity
            List<MenuItemModel> menuItems = new List<MenuItemModel>();

            // get file extensions
            string[] extensions = setting.FileExt.Split(',').ToArray();

            foreach (string ext in extensions)
            {
                // get a list of roms from the directory
                List<string> files = Directory.GetFiles(setting.RomPath, "*." + ext).ToList();

                foreach (string file in files)
                {
                    menuItems.Add(new MenuItemModel()
                    {
                        DisplayText = System.IO.Path.GetFileNameWithoutExtension(file),
                        ParentSelected = true,
                        FilePath = file,
                        RelayCommand = new RelayCommand(() =>
                        {
                            string consoleName = PageModel.GetInstance.MenuEntityModels[0].SelectedMenuItemModel.DisplayText.RemoveCommas();
                            string gameName = PageModel.GetInstance.SelectedMenuItemModel.DisplayText.RemoveCommas();

                            // navigate to game options modal
                            ModalModel.GetInstance.ModalUserControl = new GameOptionsModal(consoleName + "_" +  gameName, () => { RunEmulator(setting, file); } );
                        })
                    });
                }
            }

            // sort the games
            menuItems = menuItems.OrderBy(m => m.DisplayText).ToList();

            // select the first menu item
            if (menuItems.Count > 0)
            {
                menuItems.First().IsSelected = true;
            }

            // navigate to new menu
            PageModel.GetInstance.NavigateForwards(menuItems);
        }

        /// <summary>
        /// Runs the emulator.
        /// </summary>
        /// <param name="setting">The setting.</param>
        /// <param name="file">The file.</param>
        private void RunEmulator(EmulatorSetting setting, string file)
        {

            // ** example: this starts goldeneye
            //p.StartInfo.FileName = @"D:\emu\1964_085_60fps\originals\1964_ultrafast_v3\1964_ultrafast original.exe";
            //p.StartInfo.Arguments = "-g \"D:\\emu\\Games_N64\\007_GoldenEye.v64\"";

            try
            {
                Process p = new Process();
                p.StartInfo.FileName = setting.EmulatorPath;
                p.StartInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(setting.EmulatorPath);
                p.StartInfo.Arguments = string.Format(setting.BootCommand, file);
                p.StartInfo.UseShellExecute = true;
                p.Start();
            }
            catch (Exception ex)
            {
                // TODO: show Modal error loading the emulator
                //throw;
            }
        }
    }
}
