using System;
using System.Resources;
using System.Globalization;
using System.Xml.Serialization;
using System.Threading;

using System.Runtime.Serialization;

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Breaker.Wp7.Xna4.Managers
{
    public class ResourceManager
    {
        #region fields

        private static Dictionary<string, string> _culturesAvailable;
        private static CultureInfo _cultureInfo;
        private static string _cultureName;
        private static string[] _languages;

        #endregion

        #region enum used resource keys

        /// <summary>
        /// enum of available resource keys
        /// </summary>
        [Flags]
        public enum ResourceKeyName: int
        {
            // about screen
            AboutMenu,
            AboutMenuText,
            AboutMenuItemReview,

            // game over
            GameOverMenu,
            GameOverMenuItemRetry,
            
            // loading screen
            LoadLoading,

            // general menu entries
            MenuItemExit,
            MenuItemBack,
            
            // main menu
            MainMenu,
            MainMenuItemGame,
            MainMenuItemOptions,
            MainMenuItemAbout,
            
            // options menu
            OptionsMenu,
            OptionsMenuItemCheats,
            OptionsMenuItemSettings,
            OptionsMenuItemTrophies,
            
            // pause menu
            PauseMenu,
            PauseMenuItemContinue,
            PauseMenuItemSettings,

            
            // cheats menu
            CheatsMenu,
            CheatsMenuItemLocked,
            CheatsMenuItemDoublePlayer,
            
            // information 
            InformationLives,
            InformationPoints,
            InformationExtraBall,
            InformationLevel,
            
            // settings menu
            SettingsLanguages,

            SettingsMenu,
            SettingsMenuItemLanguages,
            SettingsMenuItemBrightness,
            SettingsMenuItemVolume,

            // trophies menu
            TrophiesMenu,

            // trophies descriptions
            TrophieDescription20Points,
            TrophieDescription30Points,
            TrophieDescriptionAllCompleted,
            TrophieDescriptionArrows,
            TrophieDescriptionBonusCollector,
            TrophieDescriptionBrokeIt,
            TrophieDescriptionCareful,
            TrophieDescriptionDark,
            TrophieDescriptionEasy,
            TrophieDescriptionFirstLevel,
            TrophieDescriptionGettingDark,
            TrophieDescriptionHighPoints,
            TrophieDescriptionHighScore,
            TrophieDescriptionHopeless,
            TrophieDescriptionLoose,
            TrophieDescriptionLowScore,
            TrophieDescriptionNowBreaking,
            TrophieDescriptionScared,
            TrophieDescriptionWonMillion,
            TrophieDescriptionPlatinum,
            TrophieDescriptionHidden,

            // trophie titles
            TrophieTitle20Points,
            TrophieTitle30Points,
            TrophieTitleAllCompleted,
            TrophieTitleArrows,
            TrophieTitleBonusCollector,
            TrophieTitleBrokeIt,
            TrophieTitleCareful,
            TrophieTitleDark,
            TrophieTitleEasy,
            TrophieTitleFirstLevel,
            TrophieTitleGettingDark,
            TrophieTitleHighPoints,
            TrophieTitleHighScore,
            TrophieTitleHopeless,
            TrophieTitleLoose,
            TrophieTitleLowScore,
            TrophieTitleNowBreaking,
            TrophieTitlePlatinum,
            TrophieTitleScared,
            TrophieTitleWonMillion,
            TrophieTitleHidden
        }

 
        #endregion

        #region properties

        private static int _languageIndex;
        public static int LanguageIndex
        {
            get
            {
                return _languageIndex; 
            }

            set
            {
                if (value >= 0 && value < _languages.Length)
                {
                    _languageIndex = value;
                    _cultureName = _languages[_languageIndex];
                }
            }

        }
      
        private static string CultureName
        {
            get { return _cultureName; }

            set
            {
                if (value == null)
                {
                    _cultureName = CultureInfo.CurrentUICulture.Name;
                }
                else
                {
                    _cultureName = value;
                }

                string cultureKey = _cultureName.Substring(0, 2);

                if (!_culturesAvailable.ContainsKey(cultureKey))
                {

                    _cultureName = _culturesAvailable["en-EN"];
                }
                else
                {
                    if (!_culturesAvailable.ContainsValue(_cultureName))
                    {
                        _cultureName = _culturesAvailable[cultureKey];
                    }
                }
 
                _cultureInfo = new CultureInfo(_cultureName.Substring(0, 2));
                System.Threading.Thread.CurrentThread.CurrentCulture = _cultureInfo;
 
            }
        }
        #endregion

        #region public static methods


        public static string Text(ResourceKeyName key)
        {
            string text = string.Empty;

            //System.Resources.ResourceManager RM =
            //    new System.Resources.ResourceManager("Breaker.Wp7.Xna4.Resources", System.Reflection.Assembly.GetExecutingAssembly());

            switch (_cultureName)
            {
                case "nl-NL":
                    text = Resources.nl_NL.ResourceManager.GetString(key.ToString());
                    break;
                case "es-ES":
                    text = Resources.es_ES.ResourceManager.GetString(key.ToString());
                    break;
                default:
                    text = Resources.en_EN.ResourceManager.GetString(key.ToString());
                    break;
            }
            //RM.GetString(key.ToString());
            return text;
        }

        #endregion

        #region initilazation

        static ResourceManager()
        {
            _culturesAvailable = new Dictionary<string, string>();
            
            // load available game cultures
            LoadAvailableCultures();

            // set resourcemanger to the set culture
            CultureName = CultureInfo.CurrentUICulture.Name;
        }

        private static void LoadAvailableCultures()
        {
            foreach (string
                   s in System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceNames())
            {
                System.Diagnostics.Debug.WriteLine("ManigfestResourceName: {0}", s);

                int index = s.IndexOf('-') - 2;

                if (index < 0) continue;

                string cultureKey = s.Substring(index, 2);
                string culturename = s.Substring(index, 5);

                System.Diagnostics.Debug.WriteLine("Adding Screen Culture [{0}, {1}]", cultureKey, culturename);

                _culturesAvailable.Add(cultureKey, culturename);
             }

            int i = 0;
            
            _languages = new string[_culturesAvailable.Count];

            foreach (string key in _culturesAvailable.Keys)
            {
                _languages[i] = _culturesAvailable[key];
                if (_languages[i] == CultureName)
                {
                    LanguageIndex = i;
                }
                i++;
            }
        }
  
    #endregion

    }
}
    