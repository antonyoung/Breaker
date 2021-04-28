#region Using Statements

using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Input;

using Breaker.Wp7.Xna4.Managers;
using Breaker.Wp7.Xna4.Elements.Game;
using Breaker.Wp7.Xna4.Screens.Elements;
using Breaker.Wp7.Xna4.Screens.Input;

#endregion

namespace Breaker.Wp7.Xna4.Screens.Base
{
    /// <summary>
    /// Base class for screens that contain a menu of options. The user can
    /// move up and down to select an entry, or cancel to back out of the screen.
    /// </summary>
    abstract class TrophieScreen : GameScreen
    {
        #region Fields

        /// <summary>
        /// the number of pixels to pad above and below menu entries for touch input
        /// </summary>
        const int MENUITEMPADDING = 10;

        /// <summary>
        /// the number of pixels of spacing left / right side of the screen
        /// </summary>
        const int SCREENPADDING = 75;

        /// <summary>
        /// List of trophies to be drawn on the menu screen
        /// </summary>
        List<Trophie> _trophiesItems = new List<Trophie>();

        /// <summary>
        /// List of menu entries to be drawn on the men screen
        /// </summary>
        List<MenuItem> _menuItems = new List<MenuItem>();


        ScreenShader _shadingTop;
        ScreenShader _shadingBottom;

        ResourceManager.ResourceKeyName _resourceKeyName;
        /// <summary>
        /// Handle menu entry based on index
        /// </summary>
        int _selectedEntry = 0;
        
        /// <summary>
        /// Used as the title of the menu screen.
        /// </summary>
        string _menuTitle;

        float _offsetY = 0;

        /// <summary>
        /// used as positioning the trophies, by any Flick GestureType 
        /// </summary>
        Vector2 _velocityGestureFlick;

        /// <summary>
        /// write Diagnostic Info 
        /// </summary>
        bool _isDebug = false;
        
        /// <summary>
        /// used as trigger to make sure that the Update event has been called
        /// </summary>
        bool _isUpdated = false;

        Vector2 _position = new Vector2(100, 0);
        
        #endregion

        #region Properties


        /// <summary>
        /// Gets the list of menu entries, the derived classes can add
        /// or change the trophie contents.
        /// </summary>
        protected IList<Trophie> TrophiesItems
        {
            get { return _trophiesItems; }
        }


        /// <summary>
        /// Gets the list of menu entries, the derived classes can add
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
        public TrophieScreen(string menuTitle)
        {
            // menu entries only need Tap for menu selection
            // trophies are flickable.
            EnabledGestures = GestureType.Tap | GestureType.Flick; 

            // set menu title
            this._menuTitle = menuTitle;
            
            // set animaition time
            TransitionOnTime = TimeSpan.FromSeconds(2.0);
            TransitionOffTime = TimeSpan.FromSeconds(2.0);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public TrophieScreen(ResourceManager.ResourceKeyName resourceKeyName)
        {
            // menu entries only need Tap for menu selection
            // trophies are flickable.
            EnabledGestures = GestureType.Tap | GestureType.Flick;

            // set menu title
            _resourceKeyName = resourceKeyName;
            _menuTitle = ResourceManager.Text(_resourceKeyName);

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
                (int)entry.Position.X - MENUITEMPADDING, (int)entry.Position.Y - MENUITEMPADDING,
                entry.GetWidth(this.ScreenManager) + 2 * MENUITEMPADDING,
                entry.GetHeight(this.ScreenManager) + 2 * MENUITEMPADDING);
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

                // handle flick gesture as vector
                if (gesture.GestureType == GestureType.Flick)
                {
                    if (_isDebug) // debug infro
                        System.Diagnostics.Debug.WriteLine("Flick Delta [{0}, {1}]", gesture.Delta.X, gesture.Delta.Y);
                    
                    // set new _velocity of the flick gesture
                    _velocityGestureFlick += Vector2.Divide(gesture.Delta, 100);
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

        #region Update

        /// <summary>
        /// Calculates the new y offset, based on the current flick gesure
        /// </summary>
        protected virtual void HandleFlickGestureType()
        {
            // no flick gesture
            if(_velocityGestureFlick.Y == 0) return;

            // set new delta 
            float deltaY = _velocityGestureFlick.Y / 10;
         
            // slowly go back to zero
            _velocityGestureFlick.Y -= deltaY;

            // handle flick direction
            bool isFlickUp = _velocityGestureFlick.Y < 0 ? true : false;
       
            // reset up flick gesture
            if (isFlickUp && _velocityGestureFlick.Y > -1)
            {
                _velocityGestureFlick = Vector2.Zero;
            }

            // reset down flick gesture
            if (!isFlickUp && _velocityGestureFlick.Y < 1)
            {
                _velocityGestureFlick = Vector2.Zero;
            }

            // set new y offset position
            _offsetY += _velocityGestureFlick.Y;
       }

        /// <summary>
        /// Updates the menu entries and the trophies positions
        /// </summary>
        public override void Update(
            GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            _menuTitle = ResourceManager.Text(_resourceKeyName);

            // set position 
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            // update menu entrie locations
            UpdateMenuEntries(gameTime, transitionOffset);

            // update trophie locations
            UpdateTrophies(gameTime, transitionOffset);

            // set trigger that everything has been updated
            _isUpdated = true;
        }

        protected virtual void UpdateMenuEntries(GameTime gameTime, float transitionOffset)
        {
            // validate if any MenuEntries are been selected
            for (int i = 0; i < _menuItems.Count; i++)
            {
                bool isSelected = IsActive && (i == _selectedEntry);

                _menuItems[i].Update(this.ScreenManager, isSelected, gameTime);
            }


            // set menu entry start position         
            Vector2 position = new Vector2(0, 420);

            // loop all entries
            foreach (MenuItem a in MenuItems)
            {
                if (ScreenState == ScreenState.TransitionOn)
                {
                    position.Y += transitionOffset * 256;
                }
                else
                {
                    //position.Y += transitionOffset * 512;
                }


                // set fixed x position to the left of the screen
                position.X = ScreenManager.GraphicsDevice.Viewport.Width - a.GetWidth(this.ScreenManager) - SCREENPADDING;

                // set the entry's position
                a.Position = new Vector2(position.X, position.Y);

                if (_isDebug)
                {
                    System.Diagnostics.Debug.WriteLine("ScreenState Position [{0}, {1}]", position.X, position.Y);
                }
            }
        }

        protected virtual void UpdateTrophies(GameTime gameTime, float transitionOffset)
        {
            // Handles the flick velocity and y offset of the trophies, because of the flick 
            HandleFlickGestureType();

            // x offset, used for screen transition animation 
            float deltaX = transitionOffset * 600;

            // y offset, used to position the next trophie in the trophie collection
            float deltaY = 82.5f;

            // clamp 
            _offsetY = MathHelper.Clamp(_offsetY, (_trophiesItems.Count - 3) * -deltaY, 10);


            Vector2 trophiePosition = new Vector2(200 - deltaX, 150 + _offsetY);

            // loop trophies
            foreach (Trophie t in TrophiesItems)
            {
                if (_isDebug)
                {
                    System.Diagnostics.Debug.WriteLine("deltaY = {0}", _offsetY);
                    System.Diagnostics.Debug.WriteLine("deltaY.Clamped = {0}", _offsetY);
                    System.Diagnostics.Debug.WriteLine("Trophie Position [{0}, {1}]", t.Position.X, t.Position.Y);
                }
                t.Position = trophiePosition;
                trophiePosition.Y += deltaY;
            }
        }

        #endregion

        #region draw

        /// <summary>
        /// Draws the menu.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            if (!_isUpdated) return;

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            
            // start drawing
            spriteBatch.Begin();

            // draw trophies
            DrawTrophies(spriteBatch);

            // draw shade
            DrawScreenShades(spriteBatch);

            // draw menu entries
            DrawMenuEntries(spriteBatch, gameTime);
            
            // draw menu title
            DrawMenuTile(spriteBatch, gameTime);
            
            // end drawing
            spriteBatch.End();
        }


        protected virtual void DrawMenuTile(SpriteBatch spriteBatch, GameTime gameTime)
        {
            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            // Draw the menu title centered on the screen
            Vector2 titlePosition = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2, 80);
            Vector2 titleOrigin = ScreenManager.Font.MeasureString(_menuTitle) / 2;
            Color titleColor = new Color(192, 192, 192) * TransitionAlpha;
            float titleScale = 1.25f;

            titlePosition.Y -= transitionOffset * 100;

            spriteBatch.DrawString(ScreenManager.Font, _menuTitle, titlePosition, titleColor, 0,
                                   titleOrigin, titleScale, SpriteEffects.None, 0);
        }

        /// <summary>
        /// Helper draw method, to draw all menu entries.
        /// </summary>
        /// <param name="spriteBatch"></param>
        protected virtual void DrawMenuEntries(SpriteBatch spriteBatch, GameTime gameTime)
        {
            // Draw each menu entry in turn.
            for (int i = 0; i < _menuItems.Count; i++)
            {
                MenuItem menuEntry = _menuItems[i];
                bool isSelected = IsActive && (i == _selectedEntry);
                menuEntry.Draw(this.ScreenManager, isSelected, gameTime);
            }

        }


        /// <summary>
        /// Helper draw method, to draw all trophies
        /// </summary>
        /// <param name="spriteBatch"></param>
        protected virtual void DrawTrophies(SpriteBatch spriteBatch)
        {
            // loop trophies
            foreach (Trophie t in TrophiesItems)
            {
                // draw trophie
                t.Draw(spriteBatch);
                //System.Diagnostics.Debug.WriteLine("Draw trophie Position [{0}, {1}]", t.Position.X, t.Position.Y);
            }
        }


        /// <summary>
        /// Helper draw method, to draw shading 
        /// (top slowly the trophies disappear because of shading)
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <summary>
        /// Helper draw method, to draw shading 
        /// (top slowly the trophies disappear because of shading)
        /// </summary>
        /// <param name="spriteBatch"></param>
        void DrawScreenShades(SpriteBatch spriteBatch)
        {
            if (_shadingTop == null)
            {
                _shadingTop = new ScreenShader(ScreenManager);
                
                _shadingTop.StartOffsetShadePoint = 100;
                
                _shadingTop.ShadeDirection = ScreenShader.Direction.TopToBottom;

                _shadingTop.DestionationRectangle
                    = new Rectangle(0, 0, ScreenManager.GraphicsDevice.Viewport.Width, 200);
            }

            if (_shadingBottom == null)
            {
                _shadingBottom = new ScreenShader(ScreenManager); ;
                
                _shadingBottom.ShadeDirection = ScreenShader.Direction.BottomToTop;
                
                _shadingBottom.DestionationRectangle
                    = new Rectangle(
                        0, ScreenManager.GraphicsDevice.Viewport.Height - 100,
                        ScreenManager.GraphicsDevice.Viewport.Width, 100);
            }

            // draw menu shading
            _shadingTop.Draw(spriteBatch);
            _shadingBottom.Draw(spriteBatch);
        }
        #endregion
    }
}
