using System;
using System.Collections.Generic;
using System.Linq;

using System.Globalization;
using System.Threading;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;

using Breaker.Wp7.Xna4.Managers;
using Breaker.Wp7.Xna4.Data;
using Breaker.Wp7.Xna4.Screens;

namespace Breaker.Wp7.Xna4
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class BreakerGame : Microsoft.Xna.Framework.Game
    {
        #region Fields

        GraphicsDeviceManager _graphics;
        ScreenManager _screenManager;
        ResourceManager _resourceManger;
        
        public static SettingsData BreakerSettings;

        public static List<TrophieData> TrophiesData;

        #endregion

        #region Initialization

        /// <summary>
        /// The main game constructor.
        /// </summary>
        public BreakerGame()
        {
            Content.RootDirectory = "Content";
            Guide.IsScreenSaverEnabled = false;

            this.Window.OrientationChanged +=new EventHandler<EventArgs>(GameWindow_OrientationChanged);
            _graphics = new GraphicsDeviceManager(this);
            _graphics.IsFullScreen = true;
            
            TargetElapsedTime = TimeSpan.FromTicks(333333);

            
         
            
            // you can choose whether you want a landscape or portait
            // game by using one of the two helper functions.
            InitializeLandscapeGraphics();
 
            
            InitializeResources();

            // Create the screen manager component.
            _screenManager = new ScreenManager(this);

            //GameResources = ResourcesGame();

            Components.Add(_screenManager);

 
            // attempt to deserialize the screen manager from disk. if that
            // fails, we add our default screens.
            if (!_screenManager.DeserializeState())
            {
                // Activate the first screens.
                _screenManager.AddScreen(new BackgroundScreen(), null);
                _screenManager.AddScreen(new MainMenuScreen(), null);
            }
        }

 
        protected override void OnExiting(object sender, System.EventArgs args)
        {
            // serialize the screen manager whenever the game exits
            //screenManager.SerializeState();

            base.OnExiting(sender, args);
        }

        /// <summary>
        /// Helper method to the initialize the game to be a portrait game.
        /// </summary>
        private void InitializePortraitGraphics()
        {
            _graphics.PreferredBackBufferWidth = 480;
            _graphics.PreferredBackBufferHeight = 800;
        }

        /// <summary>
        /// Helper method to initialize the game to be a landscape game.
        /// </summary>
        private void InitializeLandscapeGraphics()
        {
            _graphics.SupportedOrientations
                = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;

            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 480;
        
        }


        /// <summary>
        /// Helper method to initilize the language settings of the game.
        /// </summary>
        private void InitializeResources()
        {
            // load resources from isolate storage;
            _resourceManger = new ResourceManager();

            IsolatedStorageManager isManager = new IsolatedStorageManager();
            BreakerSettings = isManager.LoadSettings();
            
            if (BreakerSettings != null)
            {
                BreakerSettings.LanguageIndex = ResourceManager.LanguageIndex;
            }

            if(TrophiesData == null)
            {
                TrophiesData = isManager.LoadTrophies();
            }
        }

        void GameWindow_OrientationChanged(object sender, EventArgs e)
        {
            //
        }
        #endregion

        #region Draw

        protected override void Update(GameTime gameTime)
        {
            _graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
            _graphics.ApplyChanges();

            base.Update(gameTime);
        
        }
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            _graphics.GraphicsDevice.Clear(Color.Black);

            // The real drawing happens inside the screen manager component.
            base.Draw(gameTime);
        }

        #endregion
    }
}