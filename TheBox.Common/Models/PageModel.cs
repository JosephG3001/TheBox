using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using TheBox.Common.Menu;

namespace TheBox.Common.Models
{
    public class PageModel : ModelBase
    {
        private static PageModel _instance;
        public static PageModel GetInstance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PageModel();
                }
                return _instance;
            }
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="PageModel"/> class from being created.
        /// </summary>
        private PageModel()
        {
            this.VisibleMenuItemModels = new ObservableCollection<MenuItemModel>();
        }

        // pagination variables
        private int _currentItem;
        private int _itemCount;
        private int _currentPage;
        private int _pageCount;

        /// <summary>
        /// The _previous menu entity models
        /// </summary>
        private List<MenuEntity> _previousMenuEntityModels = new List<MenuEntity>();


        public event EventHandler NoMoreMenuEntities;
        public event EventHandler NavigatedForwards;
        public event EventHandler NavigatedBackwards;
        public event EventHandler Moved;
        public event EventHandler MovedUp;
        public event EventHandler MovedDown;
        public event EventHandler MovedLeft;
        public event EventHandler MovedRight;

        /// <summary>
        /// The _bread crumbs
        /// </summary>
        private string _breadCrumbs;

        /// <summary>
        /// Gets the menu entity models.
        /// </summary>
        public List<MenuEntity> MenuEntityModels => _previousMenuEntityModels;

        /// <summary>
        /// Gets or sets the bread cumbs.
        /// </summary>
        public string BreadCumbs
        {
            get { return _breadCrumbs; }
            set
            {
                _breadCrumbs = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the showing item string.
        /// </summary>
        public string ShowingItemString
        {
            get
            {
                if (_itemCount == 0)
                {
                    return string.Empty;
                }
                return string.Format("Showing Item: {0} of {1}", _currentItem, _itemCount);
            }
        }

        /// <summary>
        /// Gets or sets the showing page string.
        /// </summary>
        public string ShowingPageString
        {
            get
            {
                if (_itemCount == 0)
                {
                    return string.Empty;
                }
                return string.Format("Showing Page: {0} of {1}", _currentPage, _pageCount);
            }
        }


        /// <summary>
        /// Gets the menu item models.  Only 1 copy of this please.
        /// </summary>
        public ObservableCollection<MenuItemModel> VisibleMenuItemModels { get; set; }

        /// <summary>
        /// Gets or sets the menu entity model.  This is current active menu.
        /// </summary>
        public MenuEntity MenuEntityModel
        {
            get { return _previousMenuEntityModels.LastOrDefault(); }
        }

        /// <summary>
        /// Gets the selected menu item model.
        /// </summary>
        public MenuItemModel SelectedMenuItemModel
        {
            get
            {
                if (MenuEntityModel != null)
                {
                    return MenuEntityModel.SelectedMenuItemModel;
                }
                return null;
            }
        }

        
        public int GridRows => MenuEntityModel == null ? 0 : MenuEntityModel.GridRows;
        public int GridColumns => MenuEntityModel == null ? 0 : MenuEntityModel.GridColumns;

        /// <summary>
        /// Binds the items. Gets 12 menu items from the main menuitem list.
        /// </summary>
        public void BindItems(bool updateTiles)
        {
            // use the same 12 items for binding to prevent memory leak
            if (VisibleMenuItemModels.Count == 0)
            {
                // create 12 menu items
                for (int i = 0; i < 40; i++)
                {
                    VisibleMenuItemModels.Add(new MenuItemModel()
                    {
                        IsVisible = false,
                        RelayCommand = new RelayCommand(() => { })
                    });
                }
            }
            else
            {
                for (int i = 0; i < 40; i++)
                {
                    // hide them all before we bind
                    VisibleMenuItemModels[i].IsVisible = false;

                    if (updateTiles)
                    {
                        VisibleMenuItemModels[i].TileImage = null;
                    }
                }
            }

            if (MenuEntityModel != null)
            {
                int index = 0;
                foreach (var menuItem in MenuEntityModel.GetVisibleMenuItems())
                {
                    if (updateTiles)
                    {
                        if (!string.IsNullOrEmpty(menuItem.TileFilePath))
                        {
                            VisibleMenuItemModels[index].DisplayText = string.Empty;
                            var vm = VisibleMenuItemModels[index];

                            Task.Run(() =>
                            {
                                BitmapImage logo = new BitmapImage();
                                logo.BeginInit();
                                logo.UriSource = new Uri(menuItem.TileFilePath, uriKind: UriKind.RelativeOrAbsolute);
                                logo.EndInit();
                                logo.Freeze();
                                return logo;
                            }).ContinueWith(r =>
                            {
                                vm.TileImage = r.Result;
                            });
                        }
                        else
                        {
                            VisibleMenuItemModels[index].DisplayText = menuItem.DisplayText.Length > 80 ? (menuItem.DisplayText.Substring(0, 79) + "..") : menuItem.DisplayText;
                        }
                    }

                    VisibleMenuItemModels[index].IsVisible = true;
                    VisibleMenuItemModels[index].IsSelected = menuItem.IsSelected;
                    VisibleMenuItemModels[index].ParentSelected = menuItem.ParentSelected;
                    VisibleMenuItemModels[index].RelayCommand.action = menuItem.RelayCommand.action;
                    VisibleMenuItemModels[index].FilePath = menuItem.FilePath;

                    index++;
                }
            }

            OnPropertyChanged(nameof(GridRows));
            OnPropertyChanged(nameof(GridColumns));
        }

        /// <summary>
        /// Navigates to a new menu.
        /// </summary>
        /// <param name="menuItems">The menu items.</param>
        public void NavigateForwards(List<MenuItemModel> menuItems, int gridRows, int gridColumns)
        {
            // new menuentity
            var newMenuEntity = new MenuEntity(gridRows, gridColumns);

            if (menuItems.Count == 0)
            {
                // add "No items" menu item
                menuItems.Add(new MenuItemModel() { DisplayText = "No Items", ParentSelected = true, RelayCommand = new RelayCommand(() => { }) });
            }

            // add the menu items
            newMenuEntity.AddMenuItemModelRange(menuItems);

            // set new menu as current menu
            _previousMenuEntityModels.Add(newMenuEntity);

            // update view
            BindItems(true);

            NavigatedForwards?.Invoke(this, EventArgs.Empty);
            Moved?.Invoke(this, EventArgs.Empty);

            UpdatePaginationLabels();
        }

        /// <summary>
        /// Navigates backwards to the last menuEntity.
        /// </summary>
        public void NavigateBackwards()
        {
            if (_previousMenuEntityModels.Count > 0)
            {
                _previousMenuEntityModels.RemoveAt(_previousMenuEntityModels.Count - 1);
                BindItems(true);
            }

            if (_previousMenuEntityModels.Count == 0 && NoMoreMenuEntities != null)
            {
                NoMoreMenuEntities(this, EventArgs.Empty);
                ClearPaginationLabels();
            }

            NavigatedBackwards?.Invoke(this, EventArgs.Empty);
            Moved?.Invoke(this, EventArgs.Empty);

            UpdatePaginationLabels();
        }

        /// <summary>
        /// Moves up within the current menu.
        /// </summary>
        public bool MoveUp()
        {
            bool paginationOccured = MenuEntityModel.MoveUp();
            MovedUp?.Invoke(this, EventArgs.Empty);
            Moved?.Invoke(this, EventArgs.Empty);
            return paginationOccured;
        }

        /// <summary>
        /// Moves down within the current menu.
        /// </summary>
        public bool MoveDown()
        {
            bool paginationOccured = MenuEntityModel.MoveDown();
            MovedDown?.Invoke(this, EventArgs.Empty);
            Moved?.Invoke(this, EventArgs.Empty);
            return paginationOccured;
        }

        /// <summary>
        /// Moves up within the current menu.
        /// </summary>
        public bool MoveLeft()
        {
            bool paginationOccured = MenuEntityModel.MoveLeft();
            MovedLeft?.Invoke(this, EventArgs.Empty);
            Moved?.Invoke(this, EventArgs.Empty);
            return paginationOccured;
        }

        /// <summary>
        /// Moves down within the current menu.
        /// </summary>
        public bool MoveRight()
        {
            bool paginationOccured = MenuEntityModel.MoveRight();
            MovedRight?.Invoke(this, EventArgs.Empty);
            Moved?.Invoke(this, EventArgs.Empty);
            return paginationOccured;
        }

        public bool IsAtEndOfAnyRow()
        {
            if ((MenuEntityModel.CurrentPageButtonIndex + 1) % GridColumns == 0 ||
                (MenuEntityModel.ListButtonIndex + 1) == MenuEntityModel.ItemCount)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Unselects the menuItems of the current menuEntity so that the buttons change to silver.
        /// </summary>
        public void UnSelectPageModel()
        {
            this.MenuEntityModel.SetSelectedMenuItemsFlag(false);
            foreach (var item in VisibleMenuItemModels)
            {
                item.ParentSelected = false;
            }
        }

        /// <summary>
        /// Selects the menuItems of the current menuEntity so that the buttons change to Gold.
        /// </summary>
        public void SelectPageModel()
        {
            this.MenuEntityModel.SetSelectedMenuItemsFlag(true);
            foreach (var item in VisibleMenuItemModels)
            {
                item.ParentSelected = true;
            }
        }

        /// <summary>
        /// Does the bread crumbs.
        /// </summary>
        /// <param name="componentName">Name of the component.</param>
        public void DoBreadCrumbs(string componentName)
        {
            // prepare a list for the breadcrumb parts
            List<string> parts = new List<string>();

            // the first breadcrumb will be the component name
            parts.Add(componentName);

            // Add all the selected menuItems display text
            parts.AddRange(_previousMenuEntityModels.Select(m => Regex.Replace(m.SelectedMenuItemModel.DisplayText, @"\s+", " ")));

            // join the parts
            string result = string.Join(" > ", parts);
            if (result.Length > 125)
            {
                result = result.Substring(0, 124) + "..";
            }

            BreadCumbs = result;
        }

        /// <summary>
        /// Updates the pagination labels.
        /// </summary>
        /// <param name="currentItem">The current item.</param>
        /// <param name="itemCount">The item count.</param>
        /// <param name="currentPage">The current page.</param>
        /// <param name="pageCount">The page count.</param>
        public void UpdatePaginationLabels()
        {
            MenuEntity menu = PageModel.GetInstance.MenuEntityModel;
            if (menu != null)
            {
                _currentItem = (menu.CurrentPageButtonIndex + 1) + (menu.PageIndex * menu.MaxMenuLabels);
                _itemCount = menu.ItemCount;
                _currentPage = (menu.PageIndex + 1);
                _pageCount = menu.FullPageCount + (menu.ItemsRemaining > 0 ? 1 : 0);

                OnPropertyChanged(nameof(ShowingItemString));
                OnPropertyChanged(nameof(ShowingPageString));

            }
        }

        /// <summary>
        /// Clears the pagination labels.
        /// </summary>
        public void ClearPaginationLabels()
        {
            _currentItem = 0;
            _itemCount = 0;
            _currentPage = 0;
            _pageCount = 0;

            OnPropertyChanged(nameof(ShowingItemString));
            OnPropertyChanged(nameof(ShowingPageString));
        }
    }
}
