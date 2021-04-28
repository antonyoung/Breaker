#region Using Statements
using System;

using Microsoft.Xna.Framework;

using Breaker.Wp7.Xna4.Managers;
using Breaker.Wp7.Xna4.Screens.Base;
using Breaker.Wp7.Xna4.Screens.Elements;
using Breaker.Wp7.Xna4.Elements.Game;
using Breaker.Wp7.Xna4.Screens.Input;

#endregion

namespace Breaker.Wp7.Xna4.Screens
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class MainMenuScreen : MenuScreen
    {
        #region Initialization

        GamePlayScreen _gamePlayScreen;

        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainMenuScreen()
            : base(ResourceManager.ResourceKeyName.MainMenu)
        {

            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOnTime = TimeSpan.FromSeconds(1.0);

            // Create our menu entries.
            MenuItem playGameMenuEntry 
                = new MenuItem(ResourceManager.ResourceKeyName.MainMenuItemGame);
            
            MenuItem optionsMenuEntry 
                = new MenuItem(ResourceManager.ResourceKeyName.MainMenuItemOptions);
            
            MenuItem aboutMenuEntry
                = new MenuItem(ResourceManager.ResourceKeyName.MainMenuItemAbout);

            MenuItem exitMenuEntry 
                = new MenuItem(ResourceManager.ResourceKeyName.MenuItemExit);

            // Hook up menu event handlers.
            playGameMenuEntry.Selected += PlayGameMenuEntrySelected;
            optionsMenuEntry.Selected += OptionsMenuEntrySelected;
            aboutMenuEntry.Selected += AboutMenu_Selected;
            exitMenuEntry.Selected += ExitMenuEntrySelected;

            // Add entries to the menu.
            MenuItems.Add(playGameMenuEntry);
            MenuItems.Add(optionsMenuEntry);
            MenuItems.Add(aboutMenuEntry);
            MenuItems.Add(exitMenuEntry);
        }


        #endregion

        #region Handle Input

        void AboutMenu_Selected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new AboutMenuScreen(), e.PlayerIndex);
        }

        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        void PlayGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            // reset score.
            Information information = new Information();

            _gamePlayScreen = new GamePlayScreen(information);
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                               _gamePlayScreen);
        }


        /// <summary>
        /// Event handler for when the Options menu entry is selected.
        /// </summary>
        void OptionsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new OptionsMenuScreen(), e.PlayerIndex);
        }

        /// <summary>
        /// Event handler for when the exit menu entry is selected.
        /// </summary>
        void ExitMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            // start aniamation.
            // exit application.
            ScreenManager.Game.Exit();
        }

        /// <summary>
        /// When the user cancels the main menu, we exit the game.
        /// </summary>
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            ScreenManager.Game.Exit();
        }


        #endregion
    }
}
