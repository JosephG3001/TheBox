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
        /// The _button index (from 0 to 11)
        /// </summary>
        private int _buttonIndex = 0;

        public ObservableCollection<MenuItemModel> MenuItemModels { get; set; }
        
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
        public int CurrentPageButtonIndex
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
        /// the button index within the entire list including all pages
        /// </summary>
        public int ListButtonIndex
        {
            get
            {
                return (PageIndex * MaxMenuLabels) + _buttonIndex;
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

        public int ItemsOnLastRowRemaining { get; set; }

        public int FullRowCount { get; set; }

        public int FullRowsInLastPage { get; set; }

        /// <summary>
        /// Amount of full pages (how many lots of 12)
        /// </summary>
        public int FullPageCount { get; set; }

        public int ItemCount => MenuItemModels.Count;

        public int MaxMenuLabels { get; set; }

        public int GridRows { get; set; }

        public int GridColumns { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuEntity"/> class.
        /// </summary>
        public MenuEntity(int gridRows, int gridColumns)
        {
            this.MaxMenuLabels = gridRows * gridColumns;
            this.GridRows = gridRows;
            this.GridColumns = gridColumns;
            this.MenuItemModels = new ObservableCollection<MenuItemModel>();
        }

        /// <summary>
        /// Moves up.
        /// </summary>
        public bool MoveUp()
        {
            if (FullRowCount == 0)            
                return false;
            
            if (FullRowCount == 1 && GridColumns == ItemCount)
                return false;

            //we are at the start of the list on the first row, go the end
            if (CurrentPageButtonIndex < GridColumns && PageIndex == 0)
            {
                //if there are no remainders show the max buttons
                if (ItemsRemaining == 0)
                {
                    PageIndex = (Math.Max(FullPageCount - 1, 0));
                    CurrentPageButtonIndex = MaxMenuLabels - (GridColumns - CurrentPageButtonIndex);
                }
                else
                {
                    if (ItemsRemaining == 0)
                    {
                        int lastIndex = (FullRowsInLastPage * GridColumns) - (GridColumns - CurrentPageButtonIndex);
                        PageIndex = FullPageCount;
                        CurrentPageButtonIndex = Math.Max(lastIndex > ItemsRemaining ? ItemsRemaining : lastIndex, 0);
                    }
                    else
                    {
                        // Start of list, move to the bottom
                        int lastIndex = (FullRowsInLastPage * GridColumns) + CurrentPageButtonIndex;
                        PageIndex = FullPageCount;
                        CurrentPageButtonIndex = Math.Max(lastIndex > (ItemsRemaining -1) ? (ItemsRemaining - 1) : lastIndex, 0);
                    }
                }
                return FullPageCount == 0 ? false : true;
            }
            else
            {
                //move up to previous page if button < a row
                if (CurrentPageButtonIndex < GridColumns)
                {
                    PageIndex--;
                    CurrentPageButtonIndex = (MaxMenuLabels - GridColumns) + CurrentPageButtonIndex;
                    return FullPageCount == 0 ? false : true;
                }
                else
                {
                    //move up page as normal
                    CurrentPageButtonIndex -= GridColumns;
                    return false;
                }
            }
        }

        /// <summary>
        /// Moves down.
        /// </summary>
        public bool MoveDown()
        {
            if (FullRowCount == 0)
            {
                return false;
            }

            // On last Page and Row?
            if (ListButtonIndex >= (ItemCount - GridColumns))
            {
                // Within the remainders?
                if (CurrentPageButtonIndex >= FullRowsInLastPage * GridColumns)
                {
                    int indexOnRow = CurrentPageButtonIndex - (Math.Max(FullRowsInLastPage, 1) * GridColumns);
                    int oldPageIndex = PageIndex;

                    if (indexOnRow > GridColumns)
                        indexOnRow -= GridColumns;

                    PageIndex = 0;
                    CurrentPageButtonIndex = Math.Max(indexOnRow, 0);

                    if (oldPageIndex == 0)
                        return false;
                }
                else
                {
                    int indexOnRow = CurrentPageButtonIndex - ((FullRowsInLastPage - 1) * GridColumns);
                    PageIndex = 0;
                    CurrentPageButtonIndex = indexOnRow;
                }
                return FullPageCount == 0 ? false : true;
            }
            else
            {
                // On the last row of any other page?
                if (CurrentPageButtonIndex >= (MaxMenuLabels - GridColumns))
                {
                    int indexOnTheRow = (MaxMenuLabels - GridColumns) - CurrentPageButtonIndex;

                    PageIndex++;
                    int newIndex = Math.Abs(indexOnTheRow);
                    if (PageIndex == FullPageCount && newIndex >= ItemsRemaining)
                    {
                        newIndex = ItemsRemaining - 1;
                    }
                    CurrentPageButtonIndex = newIndex;
                    return true;
                }
                else
                {
                    // go to next row
                    CurrentPageButtonIndex += GridColumns;
                    return false;
                }
            }
        }

        public bool MoveLeft()
        {
            // Are we at the start of any full row?
            if ((CurrentPageButtonIndex) % GridColumns == 0)
            {
                // last page and row?
                if (PageIndex == FullPageCount && CurrentPageButtonIndex >= (FullRowsInLastPage * GridColumns))
                {
                    CurrentPageButtonIndex = ItemsRemaining - 1;
                }
                else
                {
                    CurrentPageButtonIndex += (GridColumns - 1);
                    
                }
                return false;
            }

            // Are we on the last item on last page?
            if (PageIndex == FullPageCount && (CurrentPageButtonIndex + 1) == ItemsRemaining &&
                ItemsRemaining > 0) 
            {                
          //      CurrentPageButtonIndex = FullRowsInLastPage * GridColumns;
          //      return false;
            }

            //move down page as normal
            CurrentPageButtonIndex--;

            return false;
        }

        public bool MoveRight()
        {
            // Are we at the end of any full row?
            if ((CurrentPageButtonIndex + 1) % GridColumns == 0)
            {
                CurrentPageButtonIndex -= (GridColumns - 1);
                return false;
            }

            // Are we on the last item?
            if (PageIndex == FullPageCount && (CurrentPageButtonIndex + 1) == ItemsRemaining)
            {
                int fullRowsInLastPage = ItemsRemaining / GridColumns;
                CurrentPageButtonIndex = fullRowsInLastPage * GridColumns;
                return false;
            }

            //move down page as normal
            CurrentPageButtonIndex++;
            return false;
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
                .Skip(PageIndex * MaxMenuLabels)
                .Take(MaxMenuLabels)
                .ToList();
        }

        /// <summary>
        /// Goes the back to start of menu.
        /// </summary>
        public void GoBackToStartOfMenu()
        {
            UnselectButtons();
            PageIndex = 0;
            CurrentPageButtonIndex = 0;
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
            FullPageCount = MenuItemModels.Count / MaxMenuLabels;
            ItemsRemaining = MenuItemModels.Count - (FullPageCount * MaxMenuLabels);
            MaxMenuLabels = GridRows * GridColumns;
            FullRowCount = MenuItemModels.Count / GridColumns;
            ItemsOnLastRowRemaining = MenuItemModels.Count % GridColumns;
            FullRowsInLastPage = ItemsRemaining / GridColumns;
        }
    }
}
