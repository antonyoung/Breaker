#region Using Statements

using System;
using System.Configuration;

using Microsoft.Xna.Framework;

using Breaker.Wp7.Xna4.Managers;
using Breaker.Wp7.Xna4.Screens.Base;
using Breaker.Wp7.Xna4.Screens.Elements;
using Breaker.Wp7.Xna4.Screens.Input;

#endregion

namespace Breaker.Wp7.Xna4.Screens
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    class OptionsMenuScreen : MenuScreen
    {

        #region Fields

        MenuItem _menuItemCheats;
        MenuItem _menuItemTrophies;
        MenuItem _menuItemSettings;
        MenuItem _menuItemBack;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public OptionsMenuScreen()
            : base(ResourceManager.ResourceKeyName.OptionsMenu)
        {

            // transition between screens times in seconds
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(1.0);

            // menu items
            _menuItemSettings 
                = new MenuItem(ResourceManager.ResourceKeyName.OptionsMenuItemSettings);
            
            _menuItemCheats
                = new MenuItem(ResourceManager.ResourceKeyName.OptionsMenuItemCheats);

            _menuItemTrophies 
                = new MenuItem(ResourceManager.ResourceKeyName.OptionsMenuItemTrophies);
            
            _menuItemBack 
                = new MenuItem(ResourceManager.ResourceKeyName.MenuItemBack);

            // event handlers menu items
            _menuItemSettings.Selected += MenuItemSettings_Selected;
            _menuItemCheats.Selected += MenuItemCheats_Selected;
            _menuItemTrophies.Selected += TrophiesMenu_Selected;
            _menuItemBack.Selected += BackMenu_Selected;
 
            // add to the list<MenuItem>
            MenuItems.Add(_menuItemSettings);
            MenuItems.Add(_menuItemCheats);
            MenuItems.Add(_menuItemTrophies);
            MenuItems.Add(_menuItemBack);
        }


        /// <summary>
        /// Add culture based text to the menu entries
        /// </summary>
        void SetMenuEntryText()
        {
            _menuItemCheats.Text 
                = ResourceManager.Text(
                        ResourceManager.ResourceKeyName.OptionsMenuItemCheats);
            
            _menuItemTrophies.Text 
                = ResourceManager.Text(
                        ResourceManager.ResourceKeyName.OptionsMenuItemTrophies);
            
            _menuItemSettings.Text 
                = ResourceManager.Text(
                        ResourceManager.ResourceKeyName.OptionsMenuItemSettings);
            
            _menuItemBack.Text 
                = ResourceManager.Text(
                        ResourceManager.ResourceKeyName.MenuItemBack);
        }


        #endregion

        #region Handle Input


        /// <summary>
        /// Navigate back to the Main menu screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void BackMenu_Selected(object sender, PlayerIndexEventArgs e)
        {
            // Remove current screen from the collection of the ScreenManger
            //ScreenManager.RemoveScreen(this);
            this.ExitScreen();
        }


        /// <summary>
        /// Navigate to the Cheat menu screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MenuItemCheats_Selected(object sender, PlayerIndexEventArgs e)
        {
            // add CheatMenuScreen to the collection of screens of the ScreenManager
            ScreenManager.AddScreen(new CheatMenuScreen(), e.PlayerIndex);
        }

        /// <summary>
        /// Navigate to the Settings menu screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MenuItemSettings_Selected(object sender, PlayerIndexEventArgs e)
        {
            // add SettingsMenuScreen to the collection of screens of the ScreenManager
            ScreenManager.AddScreen(new SettingsMenuScreen(), e.PlayerIndex);

        }

        /// <summary>
        /// Navigate to the Trophie menu screen 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TrophiesMenu_Selected(object sender, PlayerIndexEventArgs e)
        {
            // add TrophieMenuScreen to the collection of screens of the ScreenManager
            ScreenManager.AddScreen(new TrophieMenuScreen(), e.PlayerIndex);
        }

        #endregion
    }
}
