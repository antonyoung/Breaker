using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Breaker.Wp7.Xna4.Data;
using Breaker.Wp7.Xna4.Managers;

namespace Breaker.Wp7.Xna4.Elements.Game
{
    class TrophieAchievedArgs : System.EventArgs
    {

        #region fields

        private TrophieData _data;

        #endregion

        #region public properties

        public TrophieData Trophie
        {
            get
            {
                return _data;
            }
            private set
            {
                _data = value;
            }
        }

        #endregion

        #region public events arguments

        public TrophieAchievedArgs(TrophieData td)
        {
            this._data = td;
        }

        #endregion

    }

    class Trophies
    {

        #region public event handlers

        public delegate void TrophieAchievedEventHandler(TrophieAchievedArgs e);
        public event TrophieAchievedEventHandler TrophieAchieved; 
        
        #endregion
        
        #region public properties

      
        public List<TrophieData> TrophieElements
        {
            get;
            private set;
        }

        public int TrophiePoints
        {
            get
            {
                var list = from t in TrophieElements
                           where
                            (t.DateTimeAchievement != null)
                           select t;
                List<TrophieData> trophies = list.ToList<TrophieData>();

                int points = 0;
                foreach (TrophieData t in trophies)
                {
                    points += (int)t.Value;
                }

                return points;
            }
        }
        #endregion

        #region constructor

        public Trophies()
        {
            LoadTrophies();
        }

        #endregion

        #region private methods

        void LoadTrophies()
        {

            IsolatedStorageManager isManager = new IsolatedStorageManager();
            TrophieElements = isManager.LoadTrophies();


            if (TrophieElements.Count == 0)
            {
                InitTrophiesData();
                isManager.SaveTrophies(TrophieElements);
            }

            //DebugTrophies();

        }

        void DebugTrophies()
        {
            System.Diagnostics.Debug.WriteLine("*** START ***");
            
            foreach (TrophieData t in TrophieElements)
            {
                System.Diagnostics.Debug.WriteLine(string.Empty);
                System.Diagnostics.Debug.WriteLine("t.Type = {0}", t.Type);
                System.Diagnostics.Debug.WriteLine("t.Value = {0}", t.Value);
                System.Diagnostics.Debug.WriteLine("t.Count = {0}", t.Count);
                System.Diagnostics.Debug.WriteLine("t.DateTimeAchievement = {0}", t.DateTimeAchievement.HasValue);

            }
   
            System.Diagnostics.Debug.WriteLine("*** END ***");
   
        }
        void InitTrophiesData()
        {
            TrophieElements = new List<TrophieData>();

            // platinum
            TrophieData t1 = new TrophieData();
            t1.TitleResourceKeyName = ResourceManager.ResourceKeyName.TrophieTitlePlatinum;
            t1.DescriptionResourceKeyName = ResourceManager.ResourceKeyName.TrophieDescriptionPlatinum;
            t1.Value = TrophieData.TrophieValue.Platinum;
            t1.Type = TrophieData.TrophieType.Platinum;
            t1.IsHidden = true;
            TrophieElements.Add(t1);

            // gold
            TrophieData t2 = new TrophieData();
            t2.TitleResourceKeyName = ResourceManager.ResourceKeyName.TrophieTitleAllCompleted;
            t2.DescriptionResourceKeyName = ResourceManager.ResourceKeyName.TrophieDescriptionAllCompleted;
            t2.Value = TrophieData.TrophieValue.Gold;
            t2.Type = TrophieData.TrophieType.AllLevels;
            TrophieElements.Add(t2);

            TrophieData t3 = new TrophieData();
            t3.TitleResourceKeyName = ResourceManager.ResourceKeyName.TrophieTitleHighPoints;
            t3.DescriptionResourceKeyName = ResourceManager.ResourceKeyName.TrophieDescriptionHighPoints;
            t3.Value = TrophieData.TrophieValue.Gold;
            t3.Type = TrophieData.TrophieType.Points40;
            TrophieElements.Add(t3);


            TrophieData t4 = new TrophieData();
            t4.TitleResourceKeyName = ResourceManager.ResourceKeyName.TrophieTitleNowBreaking;
            t4.DescriptionResourceKeyName = ResourceManager.ResourceKeyName.TrophieDescriptionNowBreaking;
            t4.Value = TrophieData.TrophieValue.Gold;
            t4.Type = TrophieData.TrophieType.BreakablesCount;
            TrophieElements.Add(t4);

            TrophieData t5 = new TrophieData();
            t5.TitleResourceKeyName = ResourceManager.ResourceKeyName.TrophieTitleWonMillion;
            t5.DescriptionResourceKeyName = ResourceManager.ResourceKeyName.TrophieDescriptionWonMillion;
            t5.Value = TrophieData.TrophieValue.Gold;
            t5.Type = TrophieData.TrophieType.PointsMillion;
            TrophieElements.Add(t5);

            // silver
            TrophieData t6 = new TrophieData();
            t6.TitleResourceKeyName = ResourceManager.ResourceKeyName.TrophieTitleHighScore;
            t6.DescriptionResourceKeyName = ResourceManager.ResourceKeyName.TrophieDescriptionHighScore;
            t6.Value = TrophieData.TrophieValue.Silver;
            t6.Type = TrophieData.TrophieType.HighScore;
            TrophieElements.Add(t6);

            TrophieData t7 = new TrophieData();
            t7.TitleResourceKeyName = ResourceManager.ResourceKeyName.TrophieTitleDark;
            t7.DescriptionResourceKeyName = ResourceManager.ResourceKeyName.TrophieDescriptionDark;
            t7.Value = TrophieData.TrophieValue.Silver;
            t7.Type = TrophieData.TrophieType.TunneledCount;
            t7.IsHidden = true;
            TrophieElements.Add(t7);


            TrophieData t8 = new TrophieData();
            t8.TitleResourceKeyName = ResourceManager.ResourceKeyName.TrophieTitle30Points;
            t8.DescriptionResourceKeyName = ResourceManager.ResourceKeyName.TrophieDescription30Points;
            t8.Value = TrophieData.TrophieValue.Silver;
            t8.Type = TrophieData.TrophieType.Points30;
            TrophieElements.Add(t8);

            TrophieData t9 = new TrophieData();
            t9.TitleResourceKeyName = ResourceManager.ResourceKeyName.TrophieTitleBonusCollector;
            t9.DescriptionResourceKeyName = ResourceManager.ResourceKeyName.TrophieDescriptionBonusCollector;
            t9.Value = TrophieData.TrophieValue.Silver;
            t9.Type = TrophieData.TrophieType.Bonus;
            TrophieElements.Add(t9);

            TrophieData t10 = new TrophieData();
            t10.TitleResourceKeyName = ResourceManager.ResourceKeyName.TrophieTitleLoose;
            t10.DescriptionResourceKeyName = ResourceManager.ResourceKeyName.TrophieDescriptionLoose;
            t10.Value = TrophieData.TrophieValue.Silver;
            t10.Type = TrophieData.TrophieType.ExtraBall;
            t10.IsHidden = true;
            TrophieElements.Add(t10);

            // bronze trophies
            TrophieData t11 = new TrophieData();
            t11.TitleResourceKeyName = ResourceManager.ResourceKeyName.TrophieTitleFirstLevel;
            t11.DescriptionResourceKeyName = ResourceManager.ResourceKeyName.TrophieDescriptionFirstLevel;
            t11.Value = TrophieData.TrophieValue.Bronze;
            t11.Type = TrophieData.TrophieType.FirstLevel;
            TrophieElements.Add(t11);

            TrophieData t12 = new TrophieData();
            t12.TitleResourceKeyName = ResourceManager.ResourceKeyName.TrophieTitleLowScore;
            t12.DescriptionResourceKeyName = ResourceManager.ResourceKeyName.TrophieDescriptionLowScore;
            t12.Value = TrophieData.TrophieValue.Bronze;
            t12.Type = TrophieData.TrophieType.LowScore;
            TrophieElements.Add(t12);


            TrophieData t13 = new TrophieData();
            t13.TitleResourceKeyName = ResourceManager.ResourceKeyName.TrophieTitleBrokeIt;
            t13.DescriptionResourceKeyName = ResourceManager.ResourceKeyName.TrophieDescriptionBrokeIt;
            t13.Value = TrophieData.TrophieValue.Bronze;
            t13.Type = TrophieData.TrophieType.BrokeIt;
            TrophieElements.Add(t13);

            TrophieData t14 = new TrophieData();
            t14.TitleResourceKeyName = ResourceManager.ResourceKeyName.TrophieTitleCareful;
            t14.DescriptionResourceKeyName = ResourceManager.ResourceKeyName.TrophieDescriptionCareful;
            t14.Value = TrophieData.TrophieValue.Bronze;
            t14.Type = TrophieData.TrophieType.Careful;
            t14.IsHidden = true;
            TrophieElements.Add(t14);

            TrophieData t15 = new TrophieData();
            t15.TitleResourceKeyName = ResourceManager.ResourceKeyName.TrophieTitleGettingDark;
            t15.DescriptionResourceKeyName = ResourceManager.ResourceKeyName.TrophieDescriptionGettingDark;
            t15.Value = TrophieData.TrophieValue.Bronze;
            t15.Type = TrophieData.TrophieType.Tunneled;
            TrophieElements.Add(t15);

            TrophieData t16 = new TrophieData();
            t16.TitleResourceKeyName = ResourceManager.ResourceKeyName.TrophieTitle20Points;
            t16.DescriptionResourceKeyName = ResourceManager.ResourceKeyName.TrophieDescription20Points;
            t16.Value = TrophieData.TrophieValue.Bronze;
            t16.Type = TrophieData.TrophieType.Points20;
            TrophieElements.Add(t16);

            TrophieData t17 = new TrophieData();
            t17.TitleResourceKeyName = ResourceManager.ResourceKeyName.TrophieTitleHopeless;
            t17.DescriptionResourceKeyName = ResourceManager.ResourceKeyName.TrophieDescriptionHopeless;
            t17.Value = TrophieData.TrophieValue.Bronze;
            t17.Type = TrophieData.TrophieType.Hopeless;
            TrophieElements.Add(t17);

            TrophieData t18 = new TrophieData();
            t18.TitleResourceKeyName = ResourceManager.ResourceKeyName.TrophieTitleEasy;
            t18.DescriptionResourceKeyName = ResourceManager.ResourceKeyName.TrophieDescriptionEasy;
            t18.Value = TrophieData.TrophieValue.Bronze;
            t18.Type = TrophieData.TrophieType.DoublePlayer;
            TrophieElements.Add(t18);

            TrophieData t19 = new TrophieData();
            t19.TitleResourceKeyName = ResourceManager.ResourceKeyName.TrophieTitleScared;
            t19.DescriptionResourceKeyName = ResourceManager.ResourceKeyName.TrophieDescriptionScared;
            t19.Value = TrophieData.TrophieValue.Bronze;
            t19.Type = TrophieData.TrophieType.SinglePlayer;
            TrophieElements.Add(t19);

            TrophieData t20 = new TrophieData();
            t20.TitleResourceKeyName = ResourceManager.ResourceKeyName.TrophieTitleArrows;
            t20.DescriptionResourceKeyName = ResourceManager.ResourceKeyName.TrophieDescriptionArrows;
            t20.Value = TrophieData.TrophieValue.Bronze;
            t20.Type = TrophieData.TrophieType.Vector;
            t20.IsHidden = true;
            TrophieElements.Add(t20);
        }

        #endregion

        #region private trophie methods

        void CalculateTrophies(List<TrophieData> trophies)
        {
            foreach (TrophieData t in trophies)
            {

                if (t.DateTimeAchievement == null)
                {
                    if (t.Validate())
                    {
                        t.IsHidden = false;
                        // notify calling class that the trophie has been achieved.
                        TrophieAchievedArgs args = new TrophieAchievedArgs(t);
                        TrophieAchieved(args);

                        System.Diagnostics.Debug.WriteLine("TrophiePoints = {0}", TrophiePoints);
                    }
                }
            }
        }

        void AllTrophies()
        {
            var list = from t in TrophieElements
                       where
                        (t.DateTimeAchievement == null)
                       select t;
            List<TrophieData> trophies = list.ToList<TrophieData>();

            if (trophies.Count == 1)
            {
                TrophieData t = trophies[0];
                t.IsHidden = false;
                // notify calling class that the trophie has been achieved.
                TrophieAchievedArgs args = new TrophieAchievedArgs(t);
                TrophieAchieved(args);

            }
        }

        void MilionPointsTrophie(int points)
        {
            var list = from t in TrophieElements
                       where
                        (t.Type == TrophieData.TrophieType.PointsMillion)
                       select t;
            List<TrophieData> trophies = list.ToList<TrophieData>();

            TrophieData trophie = trophies[0];
            if (trophie.Validate(points))
            {
                trophie.IsHidden = false;
                // notify calling class that the trophie has been achieved.
                TrophieAchievedArgs args = new TrophieAchievedArgs(trophie);
                TrophieAchieved(args);
            }
        }

        #endregion

        #region public trophie methods

        public void SaveTrophiesData()
        {
            IsolatedStorageManager isManager = new IsolatedStorageManager();
            bool isSaved = isManager.SaveTrophies(TrophieElements);

            TrophieElements = isManager.LoadTrophies();
            //DebugTrophies();
        }

        public void AddTrophie(TrophieData.TrophieType trophieType)
        {
            var list = from t in TrophieElements
                       where
                        t.Type == trophieType
                       select t;

            List<TrophieData> trophies = list.ToList<TrophieData>();
            CalculateTrophies(trophies);

        }

        public void PointsTrophie(TrophieData.TrophieType trophieType)
        {
            var list = from t in TrophieElements
                       where
                        t.Type == trophieType
                       select t;

            List<TrophieData> trophies = list.ToList<TrophieData>();
            CalculateTrophies(trophies);

            int points = 0;

            switch (trophieType)
            {
                case TrophieData.TrophieType.Points40:
                    points = 40;
                    break;

                case TrophieData.TrophieType.Points30:
                    points = 30;
                    break;

                case TrophieData.TrophieType.Points20:
                    points = 40;
                    break;

                default:
                    points = 10;
                    break;
            }

            MilionPointsTrophie(points);

        }

        public void BreakbaleTrophie()
        {
            var list = from t in TrophieElements
                       where
                        (t.Type == TrophieData.TrophieType.BrokeIt 
                        || t.Type == TrophieData.TrophieType.BreakablesCount)
                       select t;

            List<TrophieData> trophies = list.ToList<TrophieData>();
            CalculateTrophies(trophies);
        }
    
        public void TunnelTrophie()
        {
            // add tunnel
            var list = from t in TrophieElements
                       where 
                        (t.Type == TrophieData.TrophieType.Tunneled || 
                        t.Type == TrophieData.TrophieType.TunneledCount)
                       select t;

            List<TrophieData> trophies = list.ToList<TrophieData>();
            CalculateTrophies(trophies);
        }

        #endregion

    }
}