#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements

using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
//using Microsoft.Phone.Info;
using Microsoft.Phone.Info;

using TmxContentLibrary;

using Breaker.Wp7.Xna4.Managers;
using Breaker.Wp7.Xna4.Screens.Base;
using Breaker.Wp7.Xna4.Elements.Game;
using Breaker.Wp7.Xna4.Screens.Elements;
using Breaker.Wp7.Xna4.Screens.Input;


#endregion

namespace Breaker.Wp7.Xna4.Screens
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    class GamePlayScreen : GameScreen
    {
        #region Fields
        
        // content textures
        ContentManager _content;
        SpriteFont _gameFont;
        Texture2D _texture;
        
        // tileLib library
        Map _gameMap;
        Camera _gameCamera;

        // game elements
        Ball _gameBall;
        Player[] _gamePlayers;
        Information _gameInformation;
        Debug _gameDebug;

        ScreenShader _screenTopShadow;
        ScreenShader _screenBottomShadow;

        // draw debug info
        bool _isDebug = false;

        #endregion

        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public GamePlayScreen(Information information)
        {
            TransitionOnTime = TimeSpan.FromSeconds(3.5);
            TransitionOffTime = TimeSpan.FromSeconds(3.5);

            _gameInformation = information;
            EnabledGestures = GestureType.Tap;

        }


        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (_content == null)
                _content = new ContentManager(ScreenManager.Game.Services, "Content");

            _gameFont = _content.Load<SpriteFont>("Fonts/gamefont");

            string level = string.Format("Maps\\level{0:00}_EASY", _gameInformation.Level);
            _gameMap = _content.Load<Map>(level);

            _texture = _content.Load<Texture2D>("Textures/blank");

            Rectangle rectangleTop 
                = new Rectangle(0, 0, ScreenManager.GraphicsDevice.Viewport.Width, 100);
                
            _screenTopShadow = new ScreenShader(ScreenManager, rectangleTop);
            _screenTopShadow.ShadeDirection = ScreenShader.Direction.TopToBottom;

            Rectangle rectangleBottom
                = new Rectangle(
                    0, ScreenManager.GraphicsDevice.Viewport.Height - 100,
                    ScreenManager.GraphicsDevice.Viewport.Width, 100);

            _screenBottomShadow = new ScreenShader(ScreenManager, rectangleBottom);
            _screenBottomShadow.ShadeDirection = ScreenShader.Direction.BottomToTop;

 
            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();
        }

        public void Restart()
        {
            // Initialise camera using the map to define the boundaries
            _gameCamera = new Camera(ScreenManager.GraphicsDevice.Viewport, _gameMap);

            // initalize game element ball
            _gameBall
                = new Ball(
                    new Vector2(
                        _gameCamera.Width / 2, _gameCamera.WorldRectangle.Height - _gameCamera.Height - 200), _gameInformation);

            //_gameBall
            //    = new Ball(
            //        new Vector2(
            //            _gameCamera.Width / 2, 250), _gameInformation);

            _gameBall.LoadContent(_content);

            //_gameCamera.Target = _gameBall.Position;

            // add players
            _gamePlayers = new Player[2];

            BoundingBox boxPlayer1 = new BoundingBox();
            BoundingBox boxPlayer2 = new BoundingBox();

            _gamePlayers[0] = new Player(
                  new Vector2(
                        _gameCamera.Width / 2, _gameCamera.WorldRectangle.Height - _gameCamera.Height - _gameMap.TileHeight * 2)
                        , boxPlayer1, _gameCamera.Position, false);

            _gamePlayers[0].LoadContent(_content);

            _gamePlayers[1] = new Player(
                 new Vector2(
                       _gameCamera.Width / 2, _gameCamera.WorldRectangle.Height - _gameCamera.Height - _gameMap.TileHeight * 3)
                       , boxPlayer2, _gameCamera.Position, true);

            _gamePlayers[1].LoadContent(_content);


            
            //_gameInformation = new Information();
            _gameInformation.LoadContent(_content);


            if (_isDebug)
            {
                _gameDebug = new Debug();
                _gameDebug.LoadContent(_content);
            }

            
            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();


            _gameBall.Start();
        }

        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            _content.Unload();
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
 
            if (!IsActive) return;

            // disable idle detection
            IdleDetetectionMode(false);

            if (_gameCamera.Position.Y == 0)
            {
                _gameBall.GameInformation.Level++;
                LoadLevel();
                return;
            }

            // update player
            foreach (Player p in _gamePlayers)
            {
                p.Update(gameTime, _gameBall, _gameCamera, ScreenManager.Game.Window.CurrentOrientation);
            }

            if (_gameBall.GameInformation.Lives < 0)
            {
                // game over
                // do trophie validation lost live
                ScreenManager.AddScreen(new GameOverMenuScreen(), ControllingPlayer);
            }
            else
            {
                // Update the Ball
                _gameBall.Update(gameTime, _gameMap, _gameCamera);
            }
          
            // Set camera target to ball location (only if the ball.y is in the 1/3rd area of viewport.height)
            //if (_gameBall.Position.Y < _gameCamera.Target.Y + _gameCamera.Height / 4 - 22f)
            if (_gameBall.Position.Y < _gameCamera.Target.Y + _gameCamera.Height / 3)
            {
                //_gameCamera.Target 
                //    = new Vector2 (
                //        _gameCamera.Target.X, 
                //        _gameBall.Position.Y - (float)_gameCamera.Height / 4 + 22f);
                _gameCamera.Target
                    = new Vector2(
                        _gameCamera.Target.X,
                        _gameBall.Position.Y - (float)_gameCamera.Height / 3);
                // Notify the camera that it has to Update() itself.
                _gameCamera.Update();
            }

        }


        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value; 

            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
            GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];

            // if the user pressed the back button, we return to the main menu
            PlayerIndex player;
            if (input.IsNewButtonPress(Buttons.Back, ControllingPlayer, out player))
            {
                // enable idle detection
                //IdleDetetectionMode(true);
                // TODO: save trophy data, information data, etc
                //_gameBall.Stop();
                //LoadingScreen.Load(ScreenManager, false, ControllingPlayer, new BackgroundScreen(), new MainMenuScreen());
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }
            else
            {
                foreach (GestureSample gest in input.Gestures)
                {
                    if (gest.GestureType == GestureType.Tap)
                    {
                        // enable idle detection
                        //IdleDetetectionMode(true);

                        // TODO: save trophy data, information data, etc
                        //_gameBall.Stop();
                        //_gameBall.Target = (_gameCamera.Position + gest.Position);
                        ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
                       
                    }
                }
            }
        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.Black * 0f, 0, 0);

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            ScreenManager.GraphicsDevice.Clear(Color.Black);

            // start drawing
            spriteBatch.Begin();

            // draw visible map
            _gameMap.Draw(spriteBatch, _gameCamera);

            // draw debug information
            if (_isDebug)
            {
                DrawDebugInformation(spriteBatch, gameTime);
            }

            // draw ball 
            if (_gameBall.GameInformation.Lives >= 0)
            {
                _gameBall.Draw(spriteBatch, gameTime, _gameCamera);
            }
            // draw players
            DrawGamePlayers(spriteBatch, gameTime);

            _screenTopShadow.Draw(spriteBatch);
            _screenBottomShadow.Draw(spriteBatch);

            // draw information
            DrawGameInformation(spriteBatch, gameTime);

            spriteBatch.End();

            if (_gameBall.GameInformation.Lives < 0)
            {
            //    base.Draw(gameTime);
            }

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0)
                ScreenManager.FadeBackBufferToBlack(1f - TransitionAlpha);
        }

        private void DrawGamePlayers(SpriteBatch spriteBatch, GameTime gameTime)
        {
            // draw players
            foreach (Player p in _gamePlayers)
            {
                p.Draw(spriteBatch, gameTime, _gameCamera);
            }
        }

        private void DrawGameInformation(SpriteBatch spriteBatch, GameTime gameTime)
        {
            // init rectangle for drawing
            Rectangle sourceRect =
                new Rectangle(0, (int)_gameCamera.Height - _gameMap.TileHeight, _gameCamera.Width, _gameCamera.Height);

            _gameInformation.Draw(spriteBatch, gameTime, sourceRect);
        }

        
        #endregion

        #region draw debug information

        private void DrawDebugInformation(SpriteBatch spriteBatch, GameTime gameTime)
        {
            _gameDebug.Draw(spriteBatch, gameTime, _gameCamera, _gameMap, _gameBall);
        }

        #endregion

        #region helper methods

        void IdleDetetectionMode(bool enabled)
        {
            if (enabled)
            {
                // enable idle detection
                //Microsoft.Phone.Shell.PhoneApplicationService.Current.ApplicationIdleDetectionMode
                //    = Microsoft.Phone.Shell.IdleDetectionMode.Enabled;
            }
            else
            {
                // disable idle detection
                Microsoft.Phone.Shell.PhoneApplicationService.Current.ApplicationIdleDetectionMode
                    = Microsoft.Phone.Shell.IdleDetectionMode.Disabled;

            }
        }

        void LoadLevel()
        {
            LoadingScreen.Load(ScreenManager, true, ControllingPlayer, new GamePlayScreen(_gameBall.GameInformation));
            ScreenManager.RemoveScreen(this);
        }
        #endregion
    }
}