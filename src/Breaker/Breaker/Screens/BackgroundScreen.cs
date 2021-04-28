#region File Description
//-----------------------------------------------------------------------------
// BackgroundScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Breaker.Wp7.Xna4.Managers;
using Breaker.Wp7.Xna4.Screens.Base;
#endregion

namespace Breaker.Wp7.Xna4.Screens
{
    /// <summary>
    /// The background screen sits behind all the other menu screens.
    /// It draws a background image that remains fixed in place regardless
    /// of whatever transitions the screens on top of it may be doing.
    /// </summary>
    class BackgroundScreen : GameScreen, IDisposable
    {
        #region Fields

        ContentManager content;
        Texture2D backgroundTexture;
        TimeSpan _elapsedAnimationTime;
        Color _backGroundColor;

        float _amount = .01f;
        float _lerpStep = .01f;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public BackgroundScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(.5);
            TransitionOffTime = TimeSpan.FromSeconds(.5);

            _elapsedAnimationTime = new TimeSpan();
            _backGroundColor = Color.Lerp(Color.Black, Color.Blue, _amount);
            
        }


        /// <summary>
        /// Loads graphics content for this screen. The background texture is quite
        /// big, so we use our own local ContentManager to load it. This allows us
        /// to unload before going from the menus into the game itself, wheras if we
        /// used the shared ContentManager provided by the Game class, the content
        /// would remain loaded forever.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            backgroundTexture = content.Load<Texture2D>("Textures/blank");
        }


        /// <summary>
        /// Unloads graphics content for this screen.
        /// </summary>
        public override void UnloadContent()
        {
            content.Unload();
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the background screen. Unlike most screens, this should not
        /// transition off even if it has been covered by another screen: it is
        /// supposed to be covered, after all! This overload forces the
        /// coveredByOtherScreen parameter to false in order to stop the base
        /// Update method wanting to transition off.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            _elapsedAnimationTime += _elapsedAnimationTime.Add(gameTime.ElapsedGameTime);

            
            if (_amount <= 0 || _amount >= 1)
            {
                _lerpStep *= -1;
            }

            if (_elapsedAnimationTime.Milliseconds >= 500)
            {
                _elapsedAnimationTime = new TimeSpan();
                
                _amount += _lerpStep;
                _backGroundColor = Color.Lerp(Color.Black, Color.Blue, _amount);
               
            }

            _backGroundColor = new Color(0, 0, 0, 0);
            base.Update(gameTime, otherScreenHasFocus, false);
        }


        /// <summary>
        /// Draws the background screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Rectangle fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);

            spriteBatch.Begin();

            
           
            //spriteBatch.Draw(backgroundTexture, fullscreen,
            //                 new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));

            spriteBatch.Draw(backgroundTexture, fullscreen,
                             _backGroundColor);

            spriteBatch.End();
        }


        #endregion
        
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
