#region Using Statements\

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Breaker.Wp7.Xna4.Managers;
using Breaker.Wp7.Xna4.Screens;
using Breaker.Wp7.Xna4.Screens.Input;

#endregion

namespace Breaker.Wp7.Xna4.Screens.Elements
{
    /// <summary>
    /// </summary>
    class MenuItem
    {
        #region Fields

        /// <summary>
        /// The text rendered for this entry.
        /// </summary>
        string _text;

        /// <summary>
        /// Tracks a fading selection effect on the entry.
        /// </summary>
        /// <remarks>
        /// The entries transition out of the selection effect when they are deselected.
        /// </remarks>
        float _selectionFade;

        /// <summary>
        /// The position at which the entry is drawn. This is set by the MenuScreen
        /// each frame in Update.
        /// </summary>
        Vector2 _position;

        /// <summary>
        /// The used resourcekey, language text
        /// </summary>
        ResourceManager.ResourceKeyName _resourceKey;

        /// <summary>
        /// Used as to write debug information
        /// </summary>
        bool _isDebug = false;

        #endregion

        #region Properties


        /// <summary>
        /// Gets or sets the text of this menu entry.
        /// </summary>
        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }


        /// <summary>
        /// Gets or sets the position at which to draw this menu entry.
        /// </summary>
        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }


        #endregion

        #region Events


        /// <summary>
        /// Event raised when the menu entry is selected.
        /// </summary>
        public event EventHandler<PlayerIndexEventArgs> Selected;


        /// <summary>
        /// Method for raising the Selected event.
        /// </summary>
        protected internal virtual void OnSelectEntry(PlayerIndex playerIndex)
        {
            if (Selected != null)
                Selected(this, new PlayerIndexEventArgs(playerIndex));
        }


        #endregion

        #region Initialization


        /// <summary>
        /// Constructs a new menu entry with the specified text.
        /// </summary>
        public MenuItem(string text)
        {
            this._text = text;
        }

        public MenuItem(ResourceManager.ResourceKeyName resourceKeyName)
        {
            _resourceKey = resourceKeyName;
            this._text = ResourceManager.Text(_resourceKey);
        }
        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the menu entry.
        /// </summary>
        public virtual void Update(ScreenManager screenManager, bool isSelected, GameTime gameTime)
        {
            // there is no such thing as a selected item on Windows Phone, so we always
            // force isSelected to be false
#if WINDOWS_PHONE
            isSelected = false;
#endif

            // When the menu selection changes, entries gradually fade between
            // their selected and deselected appearance, rather than instantly
            // popping to the new state.
            float fadeSpeed = (float)gameTime.ElapsedGameTime.TotalSeconds * 4;

             _selectionFade =  isSelected 
                 ? Math.Min(_selectionFade + fadeSpeed, 1) 
                 : Math.Max(_selectionFade - fadeSpeed, 0);
            
            if(_isDebug) // debug information
                System.Diagnostics.Debug.WriteLine("[{0}]:{1}", ResourceManager.LanguageIndex, _resourceKey);

            if (_resourceKey == ResourceManager.ResourceKeyName.SettingsLanguages)
            {

                string[] languages = ResourceManager.Text(_resourceKey).Split(',');

                _text = string.Format("{0}: {1}", ResourceManager.Text(ResourceManager.ResourceKeyName.SettingsMenuItemLanguages),
                                languages[ResourceManager.LanguageIndex]);
                
            }
            else
            {
                _text = ResourceManager.Text(_resourceKey);
            }
        }


        /// <summary>
        /// Draws the menu entry. This can be overridden to customize the appearance.
        /// </summary>
        public virtual void Draw(ScreenManager screenManager, bool isSelected, GameTime gameTime)
        {
            // there is no such thing as a selected item on Windows Phone, so we always
            // force isSelected to be false
#if WINDOWS_PHONE
            isSelected = false;
#endif

             
            // Pulsate the size of the selected menu entry.
            double time = gameTime.TotalGameTime.TotalSeconds;
            
            float pulsate = (float)Math.Sin(time * 6) + 1;

            float scale = 1 + pulsate * 0.05f * _selectionFade;

            // Modify the alpha to fade text out during transitions.
           
            /// TODO SET TransitionAlpha
            //color *= ScreenManager.TransitionAlpha;

            // Draw text, centered on the middle of each line.
            //ScreenManager screenManager = screen.ScreenManager;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            SpriteFont font = screenManager.Font;

            Vector2 origin = new Vector2(0, font.LineSpacing / 2);

            // Draw the selected entry in yellow, otherwise white.
            Color color = !isSelected ? Color.Yellow : Color.White;
           
            spriteBatch.DrawString(font, _text, _position, color, 0,
                                   origin, scale, SpriteEffects.None, 0);
        }


        /// <summary>
        /// Queries how much space this menu entry requires.
        /// </summary>
        public virtual int GetHeight(ScreenManager screenManager)
        {
            return 
                screenManager.Font.LineSpacing;
        }


        /// <summary>
        /// Queries how wide the entry is, used for centering on the screen.
        /// </summary>
        public virtual int GetWidth(ScreenManager screenManager)
        {
            return 
                (int)screenManager.Font.MeasureString(Text).X;
        
        }


        public virtual void UpdateText()
        {
            this._text 
                = Managers.ResourceManager.Text(_resourceKey);
        }
        #endregion
    }
}
