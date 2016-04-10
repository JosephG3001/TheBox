using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using TheBox.Common.Models;

namespace TheBox.Common.Menu
{
    /// <summary>
    /// MenuModel - represents a single menu (movies, settings ect)
    /// </summary>
    public class MenuEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MenuEntity"/> class.
        /// </summary>
        public MenuEntity()
        {
            this.MenuItemModels = new ObservableCollection<MenuItemModel>();
        }

        #region Private members

        /// <summary>
        /// The _button index (from 0 to 11)
        /// </summary>
        private int _buttonIndex = 0;

        #endregion Private members

        #region Public properties

        /// <summary>
        /// Gets or sets the menu item models.
        /// </summary>
        public ObservableCollection<MenuItemModel> MenuItemModels
        {
            get; set;
        }

        /// <summary>
        /// Gets the selected menu item model.
        /// </summary>
        public MenuItemModel SelectedMenuItemModel
        {
            get
            {
                return MenuItemModels.Where(m => m.IsSelected).FirstOrDefault();
            }
        }

        /// <summary>
        /// Gets or sets the index of the button.
        /// </summary>
        public int ButtonIndex
        {
            get { return _buttonIndex; }
            set
            {
                _buttonIndex = value;
                UnselectButtons();
                MenuItemModels[(value) + (PageIndex * MaxMenuLabels)].IsSelected = true;
            }
        }


        /// <summary>
        /// Current page number within this menu
        /// </summary>
        public int PageIndex
        {
            get; set;
        }

        /// <summary>
        /// Remaining list items past max labels (could have 5 odd list items)
        /// </summary>
        public int ItemsRemaining
        {
            get; set;
        }

        /// <summary>
        /// Amount of full pages (how many lots of 12)
        /// </summary>
        public int FullPageCount
        {
            get; set;
        }

        /// <summary>
        /// Gets the item count.
        /// </summary>
        public int ItemCount
        {
            get { return MenuItemModels.Count; }
        }

        /// <summary>
        /// The maximum menu labels
        /// </summary>
        public const int MaxMenuLabels = 12;

        #endregion Public properties

        #region Public methods

        /// <summary>
        /// Moves up.
        /// </summary>
        public void MoveUp()
        {
            //we are at the start of the list, go the end
            if (ButtonIndex == 0 && PageIndex == 0)
            {
                //if there are no remainders show the max buttons
                if (ItemsRemaining == 0)
                {
                    PageIndex = FullPageCount - 1;
                    ButtonIndex = MaxMenuLabels - 1;
                }
                else
                {
                    PageIndex = FullPageCount;
                    ButtonIndex = ItemsRemaining - 1;
                }
            }
            else
            {
                //move up to previous page if button = 0
                if (ButtonIndex == 0)
                {
                    PageIndex--;
                    ButtonIndex = MaxMenuLabels - 1;
                }
                else
                {
                    //move up page as normal
                    ButtonIndex--;
                }
            }
        }

        /// <summary>
        /// Moves down.
        /// </summary>
        public void MoveDown()
        {
            //we are at the end of the list, return to the begining
            if (PageIndex * MaxMenuLabels + ButtonIndex == MenuItemModels.Count - 1)
            {
                PageIndex = 0;
                ButtonIndex = 0;
            }
            else
            {
                //move down to next page if button = 12
                if (ButtonIndex == MaxMenuLabels - 1)
                {
                    PageIndex++;
                    ButtonIndex = 0;
                }
                else
                {
                    //move down page as normal
                    ButtonIndex++;
                }
            }
        }

        /// <summary>
        /// Adds the menu item model.
        /// </summary>
        /// <param name="menuItemModel">The menu item model.</param>
        public void AddMenuItemModel(MenuItemModel menuItemModel)
        {
            if (this.MenuItemModels.Count == 0)
            {
                menuItemModel.IsSelected = true;
            }
            this.MenuItemModels.Add(menuItemModel);
            RecalculateVariables();
        }

        public void AddMenuItemModelRange(List<MenuItemModel> menuItemModels)
        {
            menuItemModels.First().IsSelected = true;
            foreach (var item in menuItemModels)
            {
                MenuItemModels.Add(item);
            }
            RecalculateVariables();
        }

        /// <summary>
        /// Gets the visible menu items.  Takes 11 items from the main list of menuItems.
        /// </summary>
        /// <returns></returns>
        public List<MenuItemModel> GetVisibleMenuItems()
        {
            return MenuItemModels.Skip(PageIndex * MaxMenuLabels).Take(MaxMenuLabels).ToList();
        }

        /// <summary>
        /// Goes the back to start of menu.
        /// </summary>
        public void GoBackToStartOfMenu()
        {
            UnselectButtons();
            PageIndex = 0;
            ButtonIndex = 0;
        }

        /// <summary>
        /// Sets the parent selected menu items flag sp that they change from Gold to Silver.
        /// </summary>
        /// <param name="selected">if set to <c>true</c> [selected].</param>
        public void SetSelectedMenuItemsFlag(bool selected)
        {
            foreach (MenuItemModel item in this.MenuItemModels)
            {
                item.ParentSelected = selected;
            }
        }

        /// <summary>
        /// Unselects the buttons.
        /// </summary>
        public void UnselectButtons()
        {
            foreach (MenuItemModel item in MenuItemModels)
            {
                item.IsSelected = false;
            }
        }

        #endregion Public methods

        #region Private methods

        /// <summary>
        /// Recalculates the menu variables.
        /// </summary>
        private void RecalculateVariables()
        {
            FullPageCount = MenuItemModels.Count / MaxMenuLabels;
            ItemsRemaining = MenuItemModels.Count - (FullPageCount * MaxMenuLabels);
        }

        #endregion Private methods
    }
}
