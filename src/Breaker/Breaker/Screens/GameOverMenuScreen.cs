#region Using Statements
using System;
using Microsoft.Xna.Framework;
using System.Configuration;

using Breaker.Wp7.Xna4.Managers;
using Breaker.Wp7.Xna4.Resources;
using Breaker.Wp7.Xna4.Screens.Base;
using Breaker.Wp7.Xna4.Screens.Elements;
using Breaker.Wp7.Xna4.Screens.Input;
using Breaker.Wp7.Xna4.Elements.Game;


#endregion

namespace Breaker.Wp7.Xna4.Screens
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    class GameOverMenuScreen : MenuScreen
    {
        #region Fields

        MenuItem gameRetryMenuItem;
        MenuItem gameExitMenuItem;
        
        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public GameOverMenuScreen()
            : base(ResourceManager.ResourceKeyName.GameOverMenu)
        {

            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            gameRetryMenuItem 

                = new MenuItem(ResourceManager.ResourceKeyName.GameOverMenuItemRetry);

            gameExitMenuItem 
                = new MenuItem(ResourceManager.ResourceKeyName.MenuItemExit);
            
            // add event handlers
            gameRetryMenuItem.Selected 
                += new System.EventHandler<PlayerIndexEventArgs>(MenuItemRetry_Selected);
            gameExitMenuItem.Selected 
                += new System.EventHandler<PlayerIndexEventArgs>(MenuItemExit_Selected);
            
            // Add entries to the menu.
            MenuItems.Add(gameRetryMenuItem);
            MenuItems.Add(gameExitMenuItem);
        }

        #endregion

        #region Handle Input


        void MenuItemRetry_Selected(object sender, PlayerIndexEventArgs e)
        {
            Information information = new Information();

            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                               new GamePlayScreen(information));
        }

        void MenuItemExit_Selected(object sender, PlayerIndexEventArgs e)
        {
            //Go back to main menu
            ScreenManager.RemoveScreen(this);
            ScreenManager.AddScreen(new MainMenuScreen(), ControllingPlayer); 
        }


        #endregion
    }
}
