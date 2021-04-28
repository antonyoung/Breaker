#region Using Statements

using System;
using System.Configuration;

using Microsoft.Xna.Framework;

using Breaker.Wp7.Xna4.Managers;
using Breaker.Wp7.Xna4.Screens.Input;
using Breaker.Wp7.Xna4.Screens.Elements;
using Breaker.Wp7.Xna4.Screens.Base;
using Breaker.Wp7.Xna4.Elements.Game;


#endregion

namespace Breaker.Wp7.Xna4.Screens
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    class CheatMenuScreen : MenuScreen
    {
        #region Fields

        MenuItem _menuEntryDoublePlayer;
        MenuItem _menuEntryLocked;

        MenuItem _menuEntryCheat2;
        MenuItem _menuEntryCheat3;

        MenuItem _menuEntryBack;
        
        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public CheatMenuScreen()
            : base(ResourceManager.ResourceKeyName.CheatsMenu)
        {

            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOnTime = TimeSpan.FromSeconds(1.0);

            _menuEntryLocked 
                = new MenuItem(ResourceManager.ResourceKeyName.CheatsMenuItemLocked);

            _menuEntryDoublePlayer
                = new MenuItem(ResourceManager.ResourceKeyName.CheatsMenuItemLocked);

            _menuEntryCheat2
                = new MenuItem(ResourceManager.ResourceKeyName.CheatsMenuItemLocked);

            _menuEntryCheat3
                = new MenuItem(ResourceManager.ResourceKeyName.CheatsMenuItemLocked);
            
            _menuEntryBack
                = new MenuItem(ResourceManager.ResourceKeyName.CheatsMenuItemLocked);

            _menuEntryBack
              = new MenuItem(ResourceManager.ResourceKeyName.MenuItemBack);

            _menuEntryBack.Selected 
                += new System.EventHandler<PlayerIndexEventArgs>(GameExitMenuEntrySelected);
            
            // Add entries to the menu.
            MenuItems.Add(_menuEntryLocked);
            MenuItems.Add(_menuEntryDoublePlayer);
            MenuItems.Add(_menuEntryCheat2);
            MenuItems.Add(_menuEntryCheat3);
            MenuItems.Add(_menuEntryBack);
        }

        #endregion

        
        #region Handle Input


        void GameRetryMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                               new GamePlayScreen(new Information()));
        }

        void GameExitMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            //Go back to main menu
            this.ExitScreen();
            ScreenManager.AddScreen(new MainMenuScreen(), ControllingPlayer); 
        }


        #endregion
    }
}
