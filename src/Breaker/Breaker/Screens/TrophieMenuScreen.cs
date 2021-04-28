#region Using Statements

using System;
using System.Configuration;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using Breaker.Wp7.Xna4.Data;
using Breaker.Wp7.Xna4.Managers;

using Breaker.Wp7.Xna4.Elements.Game;
using Breaker.Wp7.Xna4.Screens.Base;
using Breaker.Wp7.Xna4.Screens.Elements;
using Breaker.Wp7.Xna4.Screens.Input;

#endregion

namespace Breaker.Wp7.Xna4.Screens
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    class TrophieMenuScreen : TrophieScreen
    {
        #region Fields

        MenuItem _trophieEntryBack;
        ContentManager _content;

        Dictionary<int, Texture2D> _texturesTrophies;
        Trophies _trophies;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public TrophieMenuScreen()
            : base(ResourceManager.ResourceKeyName.TrophiesMenu)
        {

             TransitionOnTime = TimeSpan.FromSeconds(1.5);
             TransitionOffTime = TimeSpan.FromSeconds(1.5);
            
            _trophieEntryBack 
                = new MenuItem(ResourceManager.ResourceKeyName.MenuItemBack);
            
             //add event handlers
            _trophieEntryBack.Selected
                += new System.EventHandler<PlayerIndexEventArgs>(TrophieMenuEntryBack_Selected);

            MenuItems.Add(_trophieEntryBack);
        }

        public override void LoadContent()
        {
            if (_content == null)
            {
                _content = new ContentManager(ScreenManager.Game.Services, "Content");
            }

            _texturesTrophies = new Dictionary<int, Texture2D>();

            _texturesTrophies.Add(180, _content.Load<Texture2D>("Textures/Trophies/platinum"));
            _texturesTrophies.Add(90, _content.Load<Texture2D>("Textures/Trophies/gold"));
            _texturesTrophies.Add(30, _content.Load<Texture2D>("Textures/Trophies/silver"));
            _texturesTrophies.Add(15, _content.Load<Texture2D>("Textures/Trophies/bronze"));
            _texturesTrophies.Add(0, _content.Load<Texture2D>("Textures/Trophies/locked"));

            _trophies = new Trophies();

            // trophie graphical elements
            float deltaY = 81;
            Vector2 trophiePosition = new Vector2(200, 150);

            foreach (TrophieData td in _trophies.TrophieElements)
            {
                Texture2D texture;
                if(td.IsAchieved)
                //if (td.DateTimeAchievement.HasValue)
                {
                    int points = (int)td.Value;
                    texture = _texturesTrophies[points];
                }
                else
                {
                    texture = _texturesTrophies[0];                  
                }

                Trophie t = new Trophie(ScreenManager.Font, texture, td);
                t.Position = trophiePosition;
                TrophiesItems.Add(t);

                trophiePosition.Y += deltaY;

            }

        }

        //void LoadTrophies()
        //{
           
        //    //Managers.IsolatedStorageManager isManager = new Managers.IsolatedStorageManager();
        //    //List<Data.TrophieData> trophies = isManager.LoadTrophies();
            
        //    List<Data.TrophieData> trophies = new List<TrophieData>();

        //    //if (trophies.Count == 0)
        //    //{
        //        trophies = InitTrophies();
        //    //    isManager.SaveTrophies(trophies);
        //    //}
 
        //    float deltaY = 81;
        //    Vector2 trophiePosition = new Vector2(200, 150);

        //    foreach (TrophieData td in trophies)
        //    {
        //        Texture2D texture;

        //        if (td.DateTimeAchievement == null)
        //        {
        //            texture = _texturesTrophies[0];
        //        }
        //        else
        //        {
        //            int points = (int)td.Value;
        //            texture = _texturesTrophies[points];
        //        }

        //        Trophie t = new Trophie(ScreenManager.Font, texture, td);
        //        t.Position = trophiePosition;
        //        TrophiesItems.Add(t);

        //        trophiePosition.Y += deltaY;
                
        //    }
        //}

        #endregion

        #region Handle Input

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TrophieMenuEntryBack_Selected(object sender, PlayerIndexEventArgs e)
        {
            // remove this screeen
            //ScreenManager.AddScreen(new OptionsMenuScreen(), e.PlayerIndex);
            
            // animate off
            this.ExitScreen();

        }
        #endregion

        List<TrophieData> InitTrophies()
        {
            List<TrophieData> trophies = new List<TrophieData>();
        
            // platinum
            TrophieData t1 = new TrophieData();
            t1.TitleResourceKeyName = ResourceManager.ResourceKeyName.TrophieTitlePlatinum;
            t1.DescriptionResourceKeyName = ResourceManager.ResourceKeyName.TrophieDescriptionPlatinum;
            t1.Value = TrophieData.TrophieValue.Platinum;
            t1.Type = TrophieData.TrophieType.Platinum;
            t1.IsHidden = true;
            trophies.Add(t1);

            // gold
            TrophieData t2 = new TrophieData();
            t2.TitleResourceKeyName = ResourceManager.ResourceKeyName.TrophieTitleAllCompleted;
            t2.DescriptionResourceKeyName = ResourceManager.ResourceKeyName.TrophieDescriptionAllCompleted;
            t2.Value = TrophieData.TrophieValue.Gold;
            t2.Type = TrophieData.TrophieType.AllLevels;
            trophies.Add(t2);

            TrophieData t3 = new TrophieData();
            t3.TitleResourceKeyName = ResourceManager.ResourceKeyName.TrophieTitleHighPoints;
            t3.DescriptionResourceKeyName = ResourceManager.ResourceKeyName.TrophieDescriptionHighPoints;
            t3.Value = TrophieData.TrophieValue.Gold;
            t3.Type = TrophieData.TrophieType.Points40;
            trophies.Add(t3);


            TrophieData t4 = new TrophieData();
            t4.TitleResourceKeyName = ResourceManager.ResourceKeyName.TrophieTitleNowBreaking;
            t4.DescriptionResourceKeyName = ResourceManager.ResourceKeyName.TrophieDescriptionNowBreaking;
            t4.Value = TrophieData.TrophieValue.Gold;
            t4.Type = TrophieData.TrophieType.BreakablesCount;
            trophies.Add(t4);

            TrophieData t5 = new TrophieData();
            t5.TitleResourceKeyName = ResourceManager.ResourceKeyName.TrophieTitleWonMillion;
            t5.DescriptionResourceKeyName = ResourceManager.ResourceKeyName.TrophieDescriptionWonMillion;
            t5.Value = TrophieData.TrophieValue.Gold;
            t5.Type = TrophieData.TrophieType.PointsMillion;
            trophies.Add(t5);

            // silver
            TrophieData t6 = new TrophieData();
            t6.TitleResourceKeyName = ResourceManager.ResourceKeyName.TrophieTitleHighScore;
            t6.DescriptionResourceKeyName = ResourceManager.ResourceKeyName.TrophieDescriptionHighScore;
            t6.Value = TrophieData.TrophieValue.Silver;
            t6.Type = TrophieData.TrophieType.HighScore;
            trophies.Add(t6);

            TrophieData t7 = new TrophieData();
            t7.TitleResourceKeyName = ResourceManager.ResourceKeyName.TrophieTitleDark;
            t7.DescriptionResourceKeyName = ResourceManager.ResourceKeyName.TrophieDescriptionDark;
            t7.Value = TrophieData.TrophieValue.Silver;
            t7.Type = TrophieData.TrophieType.TunneledCount;
            t7.IsHidden = true;
            trophies.Add(t7);


            TrophieData t8 = new TrophieData();
            t8.TitleResourceKeyName = ResourceManager.ResourceKeyName.TrophieTitle30Points;
            t8.DescriptionResourceKeyName = ResourceManager.ResourceKeyName.TrophieDescription30Points;
            t8.Value = TrophieData.TrophieValue.Silver;
            t8.Type = TrophieData.TrophieType.Points30;
            trophies.Add(t8);
              
            TrophieData t9 = new TrophieData();
            t9.TitleResourceKeyName = ResourceManager.ResourceKeyName.TrophieTitleBonusCollector;
            t9.DescriptionResourceKeyName = ResourceManager.ResourceKeyName.TrophieDescriptionBonusCollector;
            t9.Value = TrophieData.TrophieValue.Silver;
            t9.Type = TrophieData.TrophieType.Bonus;
            trophies.Add(t9);

            TrophieData t10 = new TrophieData();
            t10.TitleResourceKeyName = ResourceManager.ResourceKeyName.TrophieTitleLoose;
            t10.DescriptionResourceKeyName = ResourceManager.ResourceKeyName.TrophieDescriptionLoose;
            t10.Value = TrophieData.TrophieValue.Silver;
            t10.Type = TrophieData.TrophieType.ExtraBall;
            t10.IsHidden = true;
            trophies.Add(t10);

            // bronze trophies
            TrophieData t11 = new TrophieData();
            t11.TitleResourceKeyName = ResourceManager.ResourceKeyName.TrophieTitleFirstLevel;
            t11.DescriptionResourceKeyName = ResourceManager.ResourceKeyName.TrophieDescriptionFirstLevel;
            t11.Value = TrophieData.TrophieValue.Bronze;
            t11.Type = TrophieData.TrophieType.FirstLevel;
            trophies.Add(t11);

            TrophieData t12 = new TrophieData();
            t12.TitleResourceKeyName = ResourceManager.ResourceKeyName.TrophieTitleLowScore;
            t12.DescriptionResourceKeyName = ResourceManager.ResourceKeyName.TrophieDescriptionLowScore;
            t12.Value = TrophieData.TrophieValue.Bronze;
            t12.Type = TrophieData.TrophieType.LowScore;
            trophies.Add(t12);


            TrophieData t13 = new TrophieData();
            t13.TitleResourceKeyName = ResourceManager.ResourceKeyName.TrophieTitleBrokeIt;
            t13.DescriptionResourceKeyName = ResourceManager.ResourceKeyName.TrophieDescriptionBrokeIt;
            t13.Value = TrophieData.TrophieValue.Bronze;
            t13.Type = TrophieData.TrophieType.BrokeIt;
            trophies.Add(t13);

            TrophieData t14 = new TrophieData();
            t14.TitleResourceKeyName = ResourceManager.ResourceKeyName.TrophieTitleCareful;
            t14.DescriptionResourceKeyName = ResourceManager.ResourceKeyName.TrophieDescriptionCareful;
            t14.Value = TrophieData.TrophieValue.Bronze;
            t14.Type = TrophieData.TrophieType.Careful;
            t14.IsHidden = true;
            trophies.Add(t14);

            TrophieData t15 = new TrophieData();
            t15.TitleResourceKeyName = ResourceManager.ResourceKeyName.TrophieTitleGettingDark;
            t15.DescriptionResourceKeyName = ResourceManager.ResourceKeyName.TrophieDescriptionGettingDark;
            t15.Value = TrophieData.TrophieValue.Bronze;
            t15.Type = TrophieData.TrophieType.Tunneled;
            trophies.Add(t15);

            TrophieData t16 = new TrophieData();
            t16.TitleResourceKeyName = ResourceManager.ResourceKeyName.TrophieTitle20Points;
            t16.DescriptionResourceKeyName = ResourceManager.ResourceKeyName.TrophieDescription20Points;
            t16.Value = TrophieData.TrophieValue.Bronze;
            t16.Type = TrophieData.TrophieType.Points20;
            trophies.Add(t16);

            TrophieData t17 = new TrophieData();
            t17.TitleResourceKeyName = ResourceManager.ResourceKeyName.TrophieTitleHopeless;
            t17.DescriptionResourceKeyName = ResourceManager.ResourceKeyName.TrophieDescriptionHopeless;
            t17.Value = TrophieData.TrophieValue.Bronze;
            t17.Type = TrophieData.TrophieType.Hopeless;
            trophies.Add(t17);

            TrophieData t18 = new TrophieData();
            t18.TitleResourceKeyName = ResourceManager.ResourceKeyName.TrophieTitleEasy;
            t18.DescriptionResourceKeyName = ResourceManager.ResourceKeyName.TrophieDescriptionEasy;
            t18.Value = TrophieData.TrophieValue.Bronze;
            t18.Type = TrophieData.TrophieType.DoublePlayer;
            trophies.Add(t18);

            TrophieData t19 = new TrophieData();
            t19.TitleResourceKeyName = ResourceManager.ResourceKeyName.TrophieTitleScared;
            t19.DescriptionResourceKeyName = ResourceManager.ResourceKeyName.TrophieDescriptionScared;
            t19.Value = TrophieData.TrophieValue.Bronze;
            t19.Type = TrophieData.TrophieType.SinglePlayer;
            trophies.Add(t19);

            TrophieData t20 = new TrophieData();
            t20.TitleResourceKeyName = ResourceManager.ResourceKeyName.TrophieTitleArrows;
            t20.DescriptionResourceKeyName = ResourceManager.ResourceKeyName.TrophieDescriptionArrows;
            t20.Value = TrophieData.TrophieValue.Bronze;
            t20.Type = TrophieData.TrophieType.Vector;
            t20.IsHidden = true;
            trophies.Add(t20);
            return trophies;
        }
    }
}
