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
    class PauseMenuScreen : MenuScreen
    {
        #region Fields

        MenuItem _menuItemContinue;
        MenuItem _menuItemSettings;
        MenuItem _menuItemExit;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public PauseMenuScreen()
            : base(ResourceManager.ResourceKeyName.PauseMenu)
        {

            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(1.0);


            _menuItemContinue = new MenuItem(ResourceManager.ResourceKeyName.PauseMenuItemContinue);
            _menuItemSettings = new MenuItem(ResourceManager.ResourceKeyName.PauseMenuItemSettings);
         
            _menuItemExit = new MenuItem(ResourceManager.ResourceKeyName.MenuItemExit);

            //SetMenuEntryText();

            _menuItemContinue.Selected += MenuItemContinue_Selected;
            _menuItemSettings.Selected += MenuItemSettings_Selected;

            _menuItemExit.Selected += MenuItemExit_Selected;
 
            MenuItems.Add(_menuItemContinue);
            MenuItems.Add(_menuItemSettings);
            MenuItems.Add(_menuItemExit);
        }

        #endregion

        #region Handle Input


        /// <summary>
        /// Navigate back to the Main menu screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MenuItemExit_Selected(object sender, PlayerIndexEventArgs e)
        {
            foreach (GameScreen screen in ScreenManager.GetScreens())
            {
                screen.ExitScreen();
            }

            ScreenManager.AddScreen(new MainMenuScreen(), ControllingPlayer);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MenuItemContinue_Selected(object sender, PlayerIndexEventArgs e)
        {
            // remove and go back to the game
            ScreenManager.RemoveScreen(this);
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

        #endregion
    }
}
