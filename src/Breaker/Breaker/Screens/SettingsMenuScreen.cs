#region Using Statements

using System;
using System.Configuration;

using Microsoft.Xna.Framework;

using Breaker.Wp7.Xna4.Managers;
using Breaker.Wp7.Xna4.Screens.Base;
using Breaker.Wp7.Xna4.Screens.Elements;
using Breaker.Wp7.Xna4.Screens.Input;

//using Frodito.Wp7.Xna4.Controls;

#endregion

namespace Breaker.Wp7.Xna4.Screens
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    class SettingsMenuScreen : SettingsScreen
    {
        #region Fields

        MenuItem _languageMenuItem;
        MenuItem _volumeMenuItem;
        MenuItem _brightnessMenuItem;
        MenuItem _backMenuItem;

        string[] _languages;
        int _selectedLanguageIndex = 0;
        
        bool _hasChanged = false;


        // implement slider control
        //Color[] primaryColors = { Color.Red, Color.Lime };
        //string[] primaryColorNames = { "Red", "Green"};
        //TextBlock[] txtblkLabels = new TextBlock[1];
        //TextBlock[] txtblkValues = new TextBlock[1];
        //Slider[] sliders = new Slider[1];

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public SettingsMenuScreen()
            : base(ResourceManager.ResourceKeyName.SettingsMenu)
        {

            TransitionOnTime = TimeSpan.FromSeconds(1.0);
            TransitionOffTime = TimeSpan.FromSeconds(1.0);
            
            _selectedLanguageIndex = ResourceManager.LanguageIndex;

            InitializeMenuItems();

            //Initialize();
        }

        /// <summary>
        /// Initilizes the MenuItems of current menu screen.
        /// </summary>
        void InitializeMenuItems()
        {
            // get availabale languages
            _languages 
                = ResourceManager.Text(ResourceManager.ResourceKeyName.SettingsLanguages).Split(',');

            // language
            _languageMenuItem 
                = new MenuItem(ResourceManager.ResourceKeyName.SettingsLanguages);

            // audio
            _volumeMenuItem 
                = new MenuItem(ResourceManager.ResourceKeyName.SettingsMenuItemVolume);

            // screen
            _brightnessMenuItem 
                = new MenuItem(ResourceManager.ResourceKeyName.SettingsMenuItemBrightness);

            // navigation
            _backMenuItem 
                = new MenuItem(ResourceManager.ResourceKeyName.MenuItemBack);

            // add menu item event handlers 
            _languageMenuItem.Selected += MenuItemLanguages_Selected;
            _backMenuItem.Selected += MenuItemBack_Selected;

            // add menuitems to the menu item collection
            MenuItems.Add(_languageMenuItem);
            MenuItems.Add(_brightnessMenuItem);
            MenuItems.Add(_volumeMenuItem);
            MenuItems.Add(_backMenuItem);
        }

    
        public void Initialize()
        {
            //for (int primary = 0; primary < txtblkLabels.Length; primary++)
            //{
            //    txtblkLabels[primary] = new TextBlock(ScreenManager.Game);
            //    txtblkLabels[primary].Text = primaryColorNames[primary];
            //    txtblkLabels[primary].Color = primaryColors[primary];
            //    txtblkLabels[primary].HorizontalAlignment = HorizontalAlignment.Center;
            //    txtblkLabels[primary].VerticalAlignment = VerticalAlignment.Center;
            //    ScreenManager.Game.Components.Add(txtblkLabels[primary]);

            //    //this.Components.Add(txtblkLabels[primary]);

            //    txtblkValues[primary] = new TextBlock(ScreenManager.Game);
            //    txtblkValues[primary].Color = primaryColors[primary];
            //    txtblkValues[primary].HorizontalAlignment = HorizontalAlignment.Center;
            //    txtblkValues[primary].VerticalAlignment = VerticalAlignment.Center;

            //    ScreenManager.Game.Components.Add(txtblkValues[primary]);

            //}

            //sliders[0] = new Slider();
            
            //sliders[0].Value = 100;
            //sliders[1].Value = 75;
            //sliders[2].Value = 192;

            //base.Initialize();
        }
       

        #endregion

        #region Handle Input


        /// <summary>
        /// MenuItem event handler, when the Language menu item is selected.
        /// And changes the the language settings of the resources manager
        /// </summary>
        void MenuItemLanguages_Selected(object sender, PlayerIndexEventArgs e)
        {
            // get selected language based on index;
            _selectedLanguageIndex = (_selectedLanguageIndex + 1) % _languages.Length;
            
            // set languageIndex of the ResourcesManager
            BreakerGame.BreakerSettings.LanguageIndex = _selectedLanguageIndex;
            ResourceManager.LanguageIndex = _selectedLanguageIndex;

            _hasChanged = true;
        }


        void MenuItemBack_Selected(object sender, PlayerIndexEventArgs e)
        {
            // save settings to Isolated Storage
            if (_hasChanged)
            {
                IsolatedStorageManager isManager = new IsolatedStorageManager();
                isManager.SaveSettings(BreakerGame.BreakerSettings);
            }

            // back to main menu
            this.ExitScreen();
        }

        #endregion
    }
}
