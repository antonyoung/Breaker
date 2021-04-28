#region Using Statements

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Input;

using Breaker.Wp7.Xna4.Managers;
using Breaker.Wp7.Xna4.Screens.Elements;
using Breaker.Wp7.Xna4.Screens.Input;

#endregion

namespace Breaker.Wp7.Xna4.Screens.Base
{
    /// <summary>
    /// Base class for screens that contain a menu of options. The user can
    /// move up and down to select an entry, or cancel to back out of the screen.
    /// </summary>
    abstract class AboutScreen : GameScreen
    {

        #region Fields


        const int MENUEITEMPADDING = 10;

        /// <summary>
        /// the number of pixels of spacing left / right side of the screen
        /// </summary>
        const int SCREENPADDING = 75;

        int _selectedEntry = 0;

        float _linesTextCount;

        string _menuTitle;
        string _aboutText;

        List<MenuItem> _menuItems = new List<MenuItem>();

        ScreenShader _shadingTop;
        ScreenShader _shadingBottom;

        bool _isDebug = false;
        bool _isUpdated = false;

        float _offsetY = 0;

        /// <summary>
        /// used as positioning the trophies, by any Flick GestureType 
        /// </summary>
        Vector2 _velocityGestureFlick;

        Vector2 _vectorAboutText;

        ResourceManager.ResourceKeyName _resourceKeyName;

        #endregion


        #region Properties


        /// <summary>
        /// Gets the list of menu entries, so derived classes can add
        /// or change the menu contents.
        /// </summary>
        protected IList<MenuItem> MenuEntries
        {
            get { return _menuItems; }
        }


        #endregion


        #region constructor


        /// <summary>
        /// Constructor.
        /// </summary>
        public AboutScreen(string menuTitle)
        {
            // menus generally only need Tap for menu selection
            EnabledGestures = GestureType.Tap | GestureType.Flick;

            //System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
          
            this._menuTitle = menuTitle;

            // set animaition time
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(1.0);


            //  about text
            _aboutText 
                = Managers.ResourceManager.Text(ResourceManager.ResourceKeyName.AboutMenuText);

            // get height of text via regualar expression
            string expression = "#";

            Regex regex = new Regex(expression);
            _linesTextCount = ((MatchCollection)regex.Matches(_aboutText, 0)).Count;

            // replace expression with line feeds
            _aboutText = _aboutText.Replace(expression, "\n");

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public AboutScreen(ResourceManager.ResourceKeyName resourceKeyName)
        {
            // menus generally only need Tap for menu selection
            EnabledGestures = GestureType.Tap | GestureType.Flick;

            //System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            
            _resourceKeyName = resourceKeyName;
            _menuTitle = ResourceManager.Text(_resourceKeyName);

            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);


            //  about text
            _aboutText
                = Managers.ResourceManager.Text(ResourceManager.ResourceKeyName.AboutMenuText);

            // get height of text via regualar expression
            string expression = "#";

            Regex regex = new Regex(expression);
            _linesTextCount = ((MatchCollection)regex.Matches(_aboutText, 0)).Count;

            // replace expression with line feeds
            _aboutText = _aboutText.Replace(expression, "\n");

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
            //return new Rectangle(
            return new Rectangle(
                (int)entry.Position.X - MENUEITEMPADDING, (int)entry.Position.Y - MENUEITEMPADDING,
                entry.GetWidth(this.ScreenManager) + 2 * MENUEITEMPADDING,
                entry.GetHeight(this.ScreenManager) + 2 * MENUEITEMPADDING);
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
                // handle flick gesture to scroll threw text
                if (gesture.GestureType == GestureType.Flick)
                {
                    if (_isDebug) // debug infro
                        System.Diagnostics.Debug.WriteLine("Flick Delta [{0}, {1}]", gesture.Delta.X, gesture.Delta.Y);

                    // set new _velocity of the flick gesture
                    _velocityGestureFlick += Vector2.Divide(gesture.Delta, 100);
                }

                // handle tap gestures for menu entries
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

        protected virtual void HandleFlickGestureType()
        {
            // any flick gestures to handle?
            if (_velocityGestureFlick.Y == 0) return;

            // set new delta 
            float deltaY = _velocityGestureFlick.Y / 10;

            // slowly go back to zero
            _velocityGestureFlick.Y -= deltaY;

            // handle flick direction
            bool isFlickUp = _velocityGestureFlick.Y < 0 ? true : false;

            // reset up flick gesture
            if (isFlickUp && _velocityGestureFlick.Y > -.1)
            {
                _velocityGestureFlick = Vector2.Zero;
            }

            // reset down flick gesture
            if (!isFlickUp && _velocityGestureFlick.Y < .1)
            {
                _velocityGestureFlick = Vector2.Zero;
            }

            _offsetY += _velocityGestureFlick.Y;

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


        #region update

        /// <summary>
        /// Allows the screen the chance to position the menu entries. By default
        /// all menu entries are lined up in a vertical list, centered on the screen.
        /// </summary>
        protected virtual void UpdateMenuEntries(GameTime gameTime, float transitionOffset)
        {
            // Update each nested MenuEntry object.
            for (int i = 0; i < _menuItems.Count; i++)
            {
                bool isSelected = IsActive && (i == _selectedEntry);

                _menuItems[i].Update(this.ScreenManager, isSelected, gameTime);
            }

            // set menu title
            _menuTitle = ResourceManager.Text(_resourceKeyName);

            // set end position         
            Vector2 position = new Vector2(0, 420);

            // loop all entries
            foreach (MenuItem a in MenuEntries)
            {
                if (ScreenState == ScreenState.TransitionOn)
                {
                    position.Y += transitionOffset * 256;
                }
                else
                {
                    position.Y += transitionOffset * 512;
                }


                position.X
                    = _menuItems.IndexOf(a) == 0
                    ? SCREENPADDING
                    : ScreenManager.GraphicsDevice.Viewport.Width - a.GetWidth(this.ScreenManager) - SCREENPADDING;


                a.Position = new Vector2(position.X, position.Y);
                position.X += a.GetWidth(this.ScreenManager);

                // reset initial position
                position = new Vector2(0, 420);
            }

        }
        
        protected virtual void UpdateAboutText(GameTime gameTime, float transitionOffset)
        {
              // Handles the flick velocity and y offset of the trophies, because of the flick 
            HandleFlickGestureType();

            _offsetY = MathHelper.Clamp(_offsetY, this.ScreenManager.Font.LineSpacing * -_linesTextCount, 0);

            _vectorAboutText = new Vector2(SCREENPADDING - transitionOffset * 600, 150 + _offsetY);

            if (_isDebug)
            {
                System.Diagnostics.Debug.WriteLine("deltaY = {0}", _offsetY);
                System.Diagnostics.Debug.WriteLine("deltaY.Clamped = {0}", _offsetY);
                System.Diagnostics.Debug.WriteLine("AboutText = [{0}. {1}]", _vectorAboutText.X, _vectorAboutText.Y);
            }
        }

        /// <summary>
        /// Updates the menu.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            UpdateMenuEntries(gameTime, transitionOffset);

            UpdateAboutText(gameTime, transitionOffset);

            _isUpdated = true;
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
            
            // start draw
            spriteBatch.Begin();

            // draw about text
            DrawAboutText(spriteBatch);

            // draw shading
            DrawScreenShades(spriteBatch);

            // draw menu entries
            DrawMenuEntries(gameTime);            

            // draw menu title
            DrawMenuTitle(spriteBatch);

            // end draw
            spriteBatch.End();
        }


        /// <summary>
        /// Helper draw method, to draw the menu items
        /// </summary>
        /// <param name="gameTime"></param>
        protected virtual void DrawMenuEntries(GameTime gameTime)
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
        /// Helper draw method, to draw menu title.
        /// </summary>
        /// <param name="spriteBatch"></param>
        protected virtual void DrawMenuTitle(SpriteBatch spriteBatch)
        {
            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            // Draw the menu title centered on the screen
            Vector2 menuTitlePosition = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2, 80);
            Vector2 menuTitleOrigin = ScreenManager.Font.MeasureString(_menuTitle) / 2;
            Color menuTitleColor = new Color(192, 192, 192) * TransitionAlpha;
            float titleScale = 1.25f;

            menuTitlePosition.Y -= transitionOffset * 100;

            spriteBatch.DrawString(ScreenManager.Font, _menuTitle, menuTitlePosition, menuTitleColor, 0,
                                   menuTitleOrigin, titleScale, SpriteEffects.None, 0);

        }

        /// <summary>
        /// Helper draw method, to draw the about text.
        /// </summary>
        /// <param name="spriteBatch"></param>
        protected virtual void DrawAboutText(SpriteBatch spriteBatch)
        {
           

            Color aboutTextColor = new Color(192, 192, 192) * TransitionAlpha;
         

            spriteBatch.DrawString(ScreenManager.Font, _aboutText, _vectorAboutText, aboutTextColor, 0,
                                  Vector2.Zero, 1f, SpriteEffects.None, 0);
        }


        /// <summary>
        /// Helper draw method, to draw shading 
        /// (top slowly the trophies disappear because of shading)
        /// </summary>
        /// <param name="spriteBatch"></param>
        protected virtual void DrawScreenShades(SpriteBatch spriteBatch)
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
