#region Using Statements

using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Input;

using Breaker.Wp7.Xna4.Managers;
using Breaker.Wp7.Xna4.Screens.Input;
using Breaker.Wp7.Xna4.Screens.Elements;

#endregion

namespace Breaker.Wp7.Xna4.Screens.Base
{
    /// <summary>
    /// Base class for screens that contain a menu of options. The user can
    /// move up and down to select an entry, or cancel to back out of the screen.
    /// </summary>
    abstract class PauseScreen : GameScreen
    {
        #region Fields

        // the number of pixels to pad above and below menu entries for touch input
        const int MENUITEMYPADDING = 10;

        List<MenuItem> _menuItems = new List<MenuItem>();
        //List<Controls.SliderControl> _sliderControls = new List<Controls.SliderControl>();

        int _selectedEntry = 0;
        string _title;

        ResourceManager.ResourceKeyName _resourceKeyName;

        #endregion

        #region Properties


        /// <summary>
        /// Gets the list of menu entries, so derived classes can add
        /// or change the menu contents.
        /// </summary>
        protected IList<MenuItem> MenuItems
        {
            get { return _menuItems; }
        }


        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public PauseScreen(string menuTitle)
        {
            // menus generally only need Tap for menu selection
            EnabledGestures = GestureType.Tap;

            this._title = menuTitle;

            // set animaition time
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(1.0);
        }

        public PauseScreen(ResourceManager.ResourceKeyName resourceKeyName)
        {
            // menus generally only need Tap for menu selection
            EnabledGestures = GestureType.Tap;

            _resourceKeyName = resourceKeyName;
            _title = ResourceManager.Text(_resourceKeyName);

            // set animaition time
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(1.0);
        }

        #endregion

        #region Handle Input

        /// <summary>
        /// Allows the screen to create the hit bounds for a particular menu entry.
        /// </summary>
        protected virtual Rectangle GetMenuItemBounds(MenuItem entry)
        {
            // the hit bounds are the entire width of the screen, and the height of the entry
            // with some additional padding above and below.
            return new Rectangle(
                    (int)entry.Position.X - MENUITEMYPADDING, (int)entry.Position.Y - MENUITEMYPADDING,
                    entry.GetWidth(this.ScreenManager) + 2 * MENUITEMYPADDING,
                    entry.GetHeight(this.ScreenManager) + 2 * MENUITEMYPADDING);
        }

        /// <summary>
        /// Responds to user input, changing the selected entry and accepting
        /// or cancelling the menu.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            // we cancel the current menu screen if the user presses the back button
            PlayerIndex player;
            if (input.IsNewButtonPress(Buttons.Back, ControllingPlayer, out player))
            {
                OnCancel(player);
            }

            // look for any taps that occurred and select any entries that were tapped
            foreach (GestureSample gesture in input.Gestures)
            {
                if (gesture.GestureType == GestureType.Tap)
                {
                    // convert the position to a Point that we can test against a Rectangle
                    Point tapLocation = new Point((int)gesture.Position.X, (int)gesture.Position.Y);

                    // iterate the entries to see if any were tapped
                    for (int i = 0; i < _menuItems.Count; i++)
                    {
                        MenuItem menuEntry = _menuItems[i];

                        if (GetMenuItemBounds(menuEntry).Contains(tapLocation))
                        {
                            // select the entry. since gestures are only available on Windows Phone,
                            // we can safely pass PlayerIndex.One to all entries since there is only
                            // one player on Windows Phone.
                            OnSelectEntry(i, PlayerIndex.One);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Handler for when the user has chosen a menu entry.
        /// </summary>
        protected virtual void OnSelectEntry(int entryIndex, PlayerIndex playerIndex)
        {
            _menuItems[entryIndex].OnSelectEntry(playerIndex);
        }


        /// <summary>
        /// Handler for when the user has cancelled the menu.
        /// </summary>
        protected virtual void OnCancel(PlayerIndex playerIndex)
        {
            ExitScreen();
        }


        /// <summary>
        /// Helper overload makes it easy to use OnCancel as a MenuEntry event handler.
        /// </summary>
        protected void OnCancel(object sender, PlayerIndexEventArgs e)
        {
            OnCancel(e.PlayerIndex);
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the menu.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            // Update each nested MenuEntry object.
            for (int i = 0; i < _menuItems.Count; i++)
            {
                bool isSelected = IsActive && (i == _selectedEntry);

                _menuItems[i].Update(this.ScreenManager, isSelected, gameTime);
            }

            _title = ResourceManager.Text(_resourceKeyName);

            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            // start at Y = 175; each X value is generated per entry
            Vector2 position = new Vector2(0f, 175f);

            // update each menu entry's location in turn
            for (int i = 0; i < _menuItems.Count; i++)
            {
                MenuItem menuEntry = _menuItems[i];
                // each entry is to be centered horizontally
                position.X = ScreenManager.GraphicsDevice.Viewport.Width / 2 - menuEntry.GetWidth(this.ScreenManager) / 2;

                if (ScreenState == ScreenState.TransitionOn)
                {
                    position.X -= transitionOffset * 256;
                }
                else
                {
                    position.X += transitionOffset * 512;
                }
                // set the entry's position
                menuEntry.Position = position;

                // move down for the next entry the size of this entry plus our padding
                position.Y += menuEntry.GetHeight(this.ScreenManager) + (MENUITEMYPADDING * 2);
            }
        }


        /// <summary>
        /// Draws the menu.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // make sure our entries are in the right place before we draw them
            //UpdateMenuEntryLocations();

            GraphicsDevice graphics = ScreenManager.GraphicsDevice;
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;

            spriteBatch.Begin();

            // Draw each menu entry in turn.
            for (int i = 0; i < _menuItems.Count; i++)
            {
                MenuItem menuEntry = _menuItems[i];

                bool isSelected = IsActive && (i == _selectedEntry);

                menuEntry.Draw(this.ScreenManager, isSelected, gameTime);
            }

            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            // Draw the menu title centered on the screen
            Vector2 titlePosition = new Vector2(graphics.Viewport.Width / 2, 80);
            Vector2 titleOrigin = font.MeasureString(_title) / 2;
            Color titleColor = new Color(192, 192, 192) * TransitionAlpha;
            float titleScale = 1.25f;

            titlePosition.Y -= transitionOffset * 100;

            spriteBatch.DrawString(font, _title, titlePosition, titleColor, 0,
                                   titleOrigin, titleScale, SpriteEffects.None, 0);

            spriteBatch.End();
        }

  
        #endregion
    }
}
