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
using TheBox.Common.Models;
using TheBox.Games.Models;
using TheBox.Games.Settings;

namespace TheBox.Games
{
    /// <summary>
    /// Interaction logic for EmulatorSettingModal.xaml
    /// </summary>
    public partial class EmulatorSettingModal : UserControl, IBoxKeyboardControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmulatorSettingModal"/> class.
        /// </summary>
        public EmulatorSettingModal()
        {
            InitializeComponent();

            // check the text of the selected page menu button.  If it is not "Add New Emulator" then get the setting
            if (GameControlModel.GetInstance.GameSettingsManager.EmulatorSettings.EmulatorSettingList != null)
            {
                string selectedButtonText = PageModel.GetInstance.SelectedMenuItemModel.DisplayText;
                EmulatorSetting setting = GameControlModel.GetInstance.GameSettingsManager.EmulatorSettings.EmulatorSettingList.Where(m => m.EmulatatedSystemName == selectedButtonText).FirstOrDefault();

                if (setting != null)
                {
                    txtImagePath.Text = setting.ConsoleImagePath;
                    txtRomPath.Text = setting.RomPath;
                    txtRunCommand.Text = setting.BootCommand;
                    txtSystemName.Text = setting.EmulatatedSystemName;
                    txtFileExt.Text = setting.FileExt;
                    txtEmulatorPath.Text = setting.EmulatorPath;
                }
            }
        }

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
                // emulate the cancle button click (go back to menu and close modal)
                btnCancel_Click(this, new RoutedEventArgs());
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // get settings as a list
            List<EmulatorSetting> settings = new List<EmulatorSetting>();

            if (GameControlModel.GetInstance.GameSettingsManager.EmulatorSettings.EmulatorSettingList != null)
            {
                settings = GameControlModel.GetInstance.GameSettingsManager.EmulatorSettings.EmulatorSettingList.ToList();
            }

            EmulatorSetting setting = new EmulatorSetting();

            // try to get the existing emulator setting
            if (settings.Count > 0)
            {
                setting = GameControlModel.GetInstance.GameSettingsManager.EmulatorSettings.EmulatorSettingList
                    .Where(m => m.EmulatatedSystemName.ToLower() == txtSystemName.Text.ToLower())
                    .FirstOrDefault();

                if (setting == null)
                {
                    setting = new EmulatorSetting();
                    settings.Add(setting);
                }
            }
            else
            {
                settings.Add(setting);
            }

            // assign to view fields
            setting.BootCommand = txtRunCommand.Text;
            setting.ConsoleImagePath = txtImagePath.Text;
            setting.EmulatatedSystemName = txtSystemName.Text;
            setting.RomPath = txtRomPath.Text;
            setting.FileExt = txtFileExt.Text;
            setting.EmulatorPath = txtEmulatorPath.Text;

            // convert the settings back to array sorted by system name
            GameControlModel.GetInstance.GameSettingsManager.EmulatorSettings.EmulatorSettingList = settings.OrderBy(m => m.EmulatatedSystemName).ToArray();

            // save the xml
            GameControlModel.GetInstance.GameSettingsManager.SaveSettings();

            // remove the modal
            ModalModel.GetInstance.ModalUserControl = null;

            // change page buttons back to gold
            PageModel.GetInstance.SelectPageModel();

            // reload the emulator setting list by going back twice and navigating to settings again
            PageModel.GetInstance.NavigateBackwards();
            GameControl.GetInstance.NavigateToSettings();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (GameControlModel.GetInstance.GameSettingsManager.EmulatorSettings.EmulatorSettingList == null)
            {
                // TODO disabled button when no setting exist
                return;
            }

            // get settings
            List<EmulatorSetting> settings = GameControlModel.GetInstance.GameSettingsManager.EmulatorSettings.EmulatorSettingList.ToList();

            // get the setting to be deleted
            EmulatorSetting setting = settings.Where(m => m.EmulatatedSystemName.ToLower() == txtSystemName.Text.ToLower()).FirstOrDefault();

            if (setting == null)
            {
                // TODO disabled button when no setting exist
                return;
            }

            settings.Remove(setting);
            GameControlModel.GetInstance.GameSettingsManager.EmulatorSettings.EmulatorSettingList = settings.ToArray();

            // save the xml
            GameControlModel.GetInstance.GameSettingsManager.SaveSettings();

            // remove the modal
            ModalModel.GetInstance.ModalUserControl = null;

            // change page buttons back to gold
            PageModel.GetInstance.SelectPageModel();

            // reload the emulator setting list by going back twice and navigating to settings again
            PageModel.GetInstance.NavigateBackwards();
            GameControl.GetInstance.NavigateToSettings();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            // remove the modal
            ModalModel.GetInstance.ModalUserControl = null;

            // change page buttons back to gold
            PageModel.GetInstance.SelectPageModel();
        }
    }
}
