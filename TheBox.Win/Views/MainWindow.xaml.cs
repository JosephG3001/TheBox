﻿using System;
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
using TheBox.Win.Models;
using TheBox.Common;
using TheBox.Common.Models;

namespace TheBox.Win
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            // set the windows's data context to it's MainWindowModel
            this.DataContext = MainWindowModel.GetInstance;

            // set the pagination stackPanels datacontext to the Pagemodel singleton
            this.PaginationStackPanel.DataContext = PageModel.GetInstance;

            // bind property changed of the model
            MainWindowModel.GetInstance.PropertyChanged += GetInstance_PropertyChanged;
        }

        /// <summary>
        /// Handles the PropertyChanged event of the main window model.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void GetInstance_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsFullScreen")
            {
                if (MainWindowModel.GetInstance.IsFullScreen)
                {
                    // Full screen
                    this.WindowState = WindowState.Maximized;
                    this.WindowStyle = WindowStyle.None;
                    this.ResizeMode = ResizeMode.NoResize;
                    MainWindowModel.GetInstance.MouseCursor = Cursors.None;
                }
                else
                {
                    // Window
                    this.WindowState = WindowState.Normal;
                    this.WindowStyle = WindowStyle.SingleBorderWindow;
                    this.ResizeMode = ResizeMode.CanResize;
                    MainWindowModel.GetInstance.MouseCursor = Cursors.Arrow;
                }
            }
        }

        /// <summary>
        /// Handles the KeyDown event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            // send key strokes to top menu?
            if (MainWindowModel.GetInstance.IsTopMenuSelected)
            {
                HandleKeyDown(sender, e);
                return;
            }

            // send keystroke to the modal?
            if (ModalModel.GetInstance.ModalUserControl != null)
            {
                ((IBoxKeyboardControl)ModalModel.GetInstance.ModalUserControl).HandleKeyDown(sender, e);
                return;
            }

            // ** send key strokes to the selected component

            // get the selected top menu item 
            TopMenuItemModel selected = MainWindowModel.GetInstance.TopMenuItemModels.Where(m => m.IsSelected).First();

            // get the underlying usercontrol
            IBoxKeyboardControl control = selected.Component.GetUserControl() as IBoxKeyboardControl;
            if (control != null)
            {
                control.HandleKeyDown(sender, e);
            }
        }

        /// <summary>
        /// Handles the key down.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="T:System.Windows.Input.KeyEventArgs" /> instance containing the event data.</param>
        public void HandleKeyDown(object sender, KeyEventArgs e)
        {
            // ** controlling the top menu items

            // right arrow
            if (e.Key == Key.Right)
            {
                // Get the index of the selected top menu item
                int index = MainWindowModel.GetInstance.TopMenuItemModels.Where(m => m.IsSelected).First().Index;

                // De-select all top menu items
                foreach (var topMenuItem in MainWindowModel.GetInstance.TopMenuItemModels)
                {
                    topMenuItem.IsSelected = false;
                }

                // Right arrow so either move right or go to index zero
                if (index == MainWindowModel.GetInstance.TopMenuItemModels.Count)
                {
                    MainWindowModel.GetInstance.TopMenuItemModels[0].IsSelected = true;
                }
                else
                {
                    MainWindowModel.GetInstance.TopMenuItemModels.Where(m => m.Index == index + 1).First().IsSelected = true;
                }
                return;
            }

            // left arrow
            if (e.Key == Key.Left)
            {
                // Get the index of the selected top menu item
                int index = MainWindowModel.GetInstance.TopMenuItemModels.Where(m => m.IsSelected).First().Index;

                // De-select all top menu items
                foreach (var topMenuItem in MainWindowModel.GetInstance.TopMenuItemModels)
                {
                    topMenuItem.IsSelected = false;
                }

                // Left arrow so either move left or select the last top menu item
                if (index == 1)
                {
                    MainWindowModel.GetInstance.TopMenuItemModels[MainWindowModel.GetInstance.TopMenuItemModels.Count - 1].IsSelected = true;
                }
                else
                {
                    MainWindowModel.GetInstance.TopMenuItemModels.Where(m => m.Index == index - 1).First().IsSelected = true;
                }
                return;
            }

            // enter key
            if (e.Key == Key.Enter)
            {
                // Get the index of the selected top menu item
                TopMenuItemModel selected = MainWindowModel.GetInstance.TopMenuItemModels.Where(m => m.IsSelected).First();

                // does this top menu item hold an underlying usercontrol?
                if (selected.Component != null)
                {
                    // Show the plugins usercontrol in the content of the mainWindow
                    MainWindowModel.GetInstance.ActiveUserControl = selected.Component.GetUserControl();
                    MainWindowModel.GetInstance.IsTopMenuSelected = false;
                    selected.Component.ActivateComponent();
                    return;
                }

                // No underlying userControl.. Just execute what ever RelayCommand action is present on the button
                selected.RelayCommand.Execute(null);
                return;
            }
        }
    }
}