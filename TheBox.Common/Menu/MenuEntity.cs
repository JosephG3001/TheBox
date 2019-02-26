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

        /// <summary>
        /// The _button index (from 0 to 11)
        /// </summary>
        private int _buttonIndex = 0;

        /// <summary>
        /// Gets or sets the menu item models.
        /// </summary>
        public ObservableCollection<MenuItemModel> MenuItemModels { get; set; }

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
        /// ButtonIndex will only ever be 0-MaxMenuLabels (12)
        /// </summary>
        public int ButtonIndex
        {
            get { return _buttonIndex; }
            set
            {
                _buttonIndex = value;
                UnselectButtons();
                MenuItemModels[(value) + (PageIndex * PageModel.GetInstance.MaxMenuLabels)].IsSelected = true;
            }
        }


        /// <summary>
        /// Current page number within this menu
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// Remaining list items past max labels (could have 5 odd list items)
        /// </summary>
        public int ItemsRemaining { get; set; }

        /// <summary>
        /// Amount of full pages (how many lots of 12)
        /// </summary>
        public int FullPageCount { get; set; }

        /// <summary>
        /// Gets the item count.
        /// </summary>
        public int ItemCount => MenuItemModels.Count;



        /// <summary>
        /// Moves up.
        /// </summary>
        public void MoveUp()
        {
            //we are at the start of the list on the first row, go the end
            if (ButtonIndex < PageModel.GetInstance.GridColumns && PageIndex == 0)
            {
                //if there are no remainders show the max buttons
                if (ItemsRemaining == 0)
                {
                    PageIndex = (Math.Max(FullPageCount - 1, 0));
                    ButtonIndex = PageModel.GetInstance.MaxMenuLabels - (PageModel.GetInstance.GridColumns - ButtonIndex);
                }
                else
                {
                    int fullRowsInLastPage = ItemsRemaining / PageModel.GetInstance.GridColumns;
                    int remaindersInLastPage = ItemsRemaining % PageModel.GetInstance.GridColumns;

                    if (remaindersInLastPage == 0)
                    {
                        int lastIndex = (fullRowsInLastPage * PageModel.GetInstance.GridColumns) - (PageModel.GetInstance.GridColumns - ButtonIndex);
                        PageIndex = FullPageCount;
                        ButtonIndex = Math.Max(lastIndex > ItemsRemaining ? ItemsRemaining : lastIndex, 0);
                    }
                    else
                    {
                        // Start of list, move to the bottom
                        int lastIndex = (fullRowsInLastPage * PageModel.GetInstance.GridColumns) + ButtonIndex;
                        PageIndex = FullPageCount;
                        ButtonIndex = Math.Max(lastIndex > (ItemsRemaining -1) ? (ItemsRemaining - 1) : lastIndex, 0);
                    }
                }
            }
            else
            {
                //move up to previous page if button < a row
                if (ButtonIndex < PageModel.GetInstance.GridColumns)
                {
                    PageIndex--;
                    ButtonIndex = (PageModel.GetInstance.MaxMenuLabels - PageModel.GetInstance.GridColumns) + ButtonIndex;
                }
                else
                {
                    //move up page as normal
                    ButtonIndex -= PageModel.GetInstance.GridColumns;
                }
            }
        }

        /// <summary>
        /// Moves down.
        /// </summary>
        public void MoveDown()
        {
            int fullRowsInLastPage = ItemsRemaining / PageModel.GetInstance.GridColumns;
            //int remaindersInLastPageRow = ItemsRemaining % PageModel.GetInstance.GridColumns;

            // on last page and row?
            if (PageIndex == FullPageCount &&
                ButtonIndex >= (ItemsRemaining - PageModel.GetInstance.GridColumns))
            {
                // Within the remainders?
                if (ButtonIndex >= fullRowsInLastPage * PageModel.GetInstance.GridColumns)
                {
                    int indexOnRow = ButtonIndex - (fullRowsInLastPage * PageModel.GetInstance.GridColumns);
                    PageIndex = 0;
                    ButtonIndex = indexOnRow;
                }
                else
                {
                    int indexOnRow = ButtonIndex - ((fullRowsInLastPage - 1) * PageModel.GetInstance.GridColumns);
                    PageIndex = 0;
                    ButtonIndex = indexOnRow;
                }
            }
            else
            {
                // On the last row of any other page?
                if (ButtonIndex >= (PageModel.GetInstance.MaxMenuLabels - PageModel.GetInstance.GridColumns))
                {
                    int indexOnTheRow = (PageModel.GetInstance.MaxMenuLabels - PageModel.GetInstance.GridColumns) - ButtonIndex;

                    PageIndex++;
                    int newIndex = Math.Abs(indexOnTheRow);
                    if (PageIndex == FullPageCount && newIndex >= ItemsRemaining)
                    {
                        newIndex = ItemsRemaining - 1;
                    }
                    ButtonIndex = newIndex;
                }
                else
                {
                    // go to next row
                    ButtonIndex += PageModel.GetInstance.GridColumns;
                }
            }
        }

        public void MoveLeft()
        {
            int fullRowsInLastPage = ItemsRemaining / PageModel.GetInstance.GridColumns;
            int remaindersInLastPage = ItemsRemaining % PageModel.GetInstance.GridColumns;

            // Are we at the start of any full row?
            if ((ButtonIndex) % PageModel.GetInstance.GridColumns == 0)
            {
                // last page and row?
                if (PageIndex == FullPageCount && ButtonIndex >= (fullRowsInLastPage * PageModel.GetInstance.GridColumns))
                {
                    ButtonIndex = ItemsRemaining - 1;
                    return;
                }
                else
                {
                    ButtonIndex += (PageModel.GetInstance.GridColumns - 1);
                    return;
                }                
            }

            // Are we on the last item?
            if (PageIndex == FullPageCount && (ButtonIndex + 1) == ItemsRemaining &&
                remaindersInLastPage > 0) 
            {                
                ButtonIndex = fullRowsInLastPage * PageModel.GetInstance.GridColumns;
                return;
            }

            //move down page as normal
            ButtonIndex--;
        }

        public void MoveRight()
        {
            // Are we at the end of any full row?
            if ((ButtonIndex + 1) % PageModel.GetInstance.GridColumns == 0)
            {
                ButtonIndex -= (PageModel.GetInstance.GridColumns - 1);
                return;
            }

            // Are we on the last item?
            if (PageIndex == FullPageCount && (ButtonIndex + 1) == ItemsRemaining)
            {
                int fullRowsInLastPage = ItemsRemaining / PageModel.GetInstance.GridColumns;
                ButtonIndex = fullRowsInLastPage * PageModel.GetInstance.GridColumns;
                return;
            }

            //move down page as normal
            ButtonIndex++;
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
            return MenuItemModels
                .Skip(PageIndex * PageModel.GetInstance.MaxMenuLabels)
                .Take(PageModel.GetInstance.MaxMenuLabels)
                .ToList();
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

        /// <summary>
        /// Recalculates the menu variables.
        /// </summary>
        private void RecalculateVariables()
        {
            FullPageCount = MenuItemModels.Count / PageModel.GetInstance.MaxMenuLabels;
            ItemsRemaining = MenuItemModels.Count - (FullPageCount * PageModel.GetInstance.MaxMenuLabels);
        }
    }
}
