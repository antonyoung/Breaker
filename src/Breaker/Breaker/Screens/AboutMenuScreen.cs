#region Using Statements

using System;
using System.Configuration;

using Microsoft.Xna.Framework;

using Breaker.Wp7.Xna4.Managers;
using Breaker.Wp7.Xna4.Screens.Base;
using Breaker.Wp7.Xna4.Screens.Input;
using Breaker.Wp7.Xna4.Screens.Elements;

#endregion

namespace Breaker.Wp7.Xna4.Screens
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    class AboutMenuScreen : AboutScreen
    {

        #region Fields

        MenuItem _menuEntryReview;
        MenuItem _menuEntryBack;
        
        #endregion


        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public AboutMenuScreen()
            : base(ResourceManager.ResourceKeyName.AboutMenu)
        {

            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(1.0);
                        
            _menuEntryReview
                = new MenuItem(ResourceManager.ResourceKeyName.AboutMenuItemReview);

            _menuEntryBack
                = new MenuItem(ResourceManager.ResourceKeyName.MenuItemBack);
            
            // add menu entry event handlers
            _menuEntryReview.Selected 
                 += new System.EventHandler<PlayerIndexEventArgs>(AboutMenuEntryReview_Selected);

            _menuEntryBack.Selected 
                += new System.EventHandler<PlayerIndexEventArgs>(AboutMenuEntryBack_Selected);
            
            // Add menu entries to the about screen
            MenuEntries.Add(_menuEntryReview);
            MenuEntries.Add(_menuEntryBack);
        }

        #endregion


        #region Handle Input


        void AboutMenuEntryBack_Selected(object sender, PlayerIndexEventArgs e)
        {
            //Go back to main menu
            this.ExitScreen();
        }

        void AboutMenuEntryReview_Selected(object sender, PlayerIndexEventArgs e)
        {
            // todo: go to review website 
        }
        #endregion

    }
}
