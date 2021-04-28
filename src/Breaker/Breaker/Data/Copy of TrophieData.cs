#region usings

using System;
using Breaker.Wp7.Xna4.Managers;

#endregion 

namespace Breaker.Wp7.Xna4.Data
{
    /// <summary>
    /// Class which defines the data of a Trophie.
    /// </summary>
    public class TrophieData
    {

        #region fields

        TrophieValue _trophieValue = TrophieValue.Bronze;
        TrophieType _trophieType = TrophieType.Bonus;

        DateTime? _dateTimeAchievement;
        
        int _count;

        bool _isHidden;
        bool _isDebug = true;
        
        #endregion

        #region enumerations

        /// <summary>
        /// Enum of the kinds of trophies.
        /// </summary>
        [Flags]
        public enum TrophieType
        {
            Platinum,
            AllLevels,
            BreakablesCount,
            Bonus,
            BrokeIt,
            Careful,
            DoublePlayer,
            ExtraBall,
            FirstLevel,
            HighScore,
            Hopeless,
            LostLive,
            LowScore,
            Points40,
            Points30,
            Points20,
            PointsMillion,
            SinglePlayer,
            Tunneled,
            TunneledCount,
            Vector
        }
        
        
        /// <summary>
        /// Enum of Possible Trophie Values
        /// </summary>
        public enum TrophieValue:int
        {
            Platinum = 180,
            Gold = 90,
            Silver = 30,
            Bronze = 15,
            None = 0
        }

        #endregion

        #region properties

        /// <summary>
        /// Determines the value to the trophie, based on enumeration.
        /// </summary>
        public TrophieValue Value
        {
            get
            {
                return _trophieValue;
            }
            set
            {
                _trophieValue = value;
            }
        }

        /// <summary>
        /// Determines the kind of trophie, based on enumeration.
        /// </summary>
        public TrophieType Type
        {
            get
            {
                return _trophieType;
            }
            set
            {
                _trophieType = value;
            }
        }
        
        public bool IsHidden 
        {
            get
            { 
                return _isHidden;
            }
            set
            {
                _isHidden = value;
            }
        }

        public int Count
        {
            get
            {
                return _count;
            }
            private set
            {
                _count = value;
            }
        }
        
        public ResourceManager.ResourceKeyName TitleResourceKeyName;
        public ResourceManager.ResourceKeyName DescriptionResourceKeyName;

        public DateTime? DateTimeAchievement
        {
            get
            {
                return _dateTimeAchievement;
            }
            private set
            {
                _dateTimeAchievement = value;
            }
        }

        #endregion

        #region public methods
        
        /// <summary>
        /// Validates if the Trophie has been achieved or not.
        /// </summary>
        /// <returns>boolean, in the case the trophie has been achieved.</returns>
        public bool Validate()
        {
            bool isAchieved = false;

            // 1 count trophies (x5) 
            if(_trophieType == TrophieType.BrokeIt | _trophieType == TrophieType.FirstLevel | 
                _trophieType ==  TrophieType.Hopeless | _trophieType == TrophieType.Tunneled | 
                _trophieType == TrophieType.Vector | _trophieType == TrophieType.Careful |
                _trophieType == TrophieType.AllLevels | _trophieType == TrophieType.Platinum)
            {
                if (Count == 0)
                {
                    _count++;
                    isAchieved = true;
                    DateTimeAchievement = DateTime.Now;
                }
            }

            // 1.000 count trophies (x4)
            if (_trophieType == TrophieType.SinglePlayer | _trophieType == TrophieType.DoublePlayer |
                _trophieType == TrophieType.ExtraBall | _trophieType == TrophieType.TunneledCount)
            {
                _count++;
                if (DateTimeAchievement == null && _count > 999)
                {
                    isAchieved = true;
                    DateTimeAchievement = DateTime.Now;
                }
            }


            // 10.000 count trophies (x5)
            if (_trophieType == TrophieType.Bonus | _trophieType == TrophieType.LowScore |
                _trophieType == TrophieType.Points20 | _trophieType == TrophieType.Points30 |
                _trophieType == TrophieType.Points40)
            {

                // Collected 10.000 times a Bonus breabable
                _count++;
                if (DateTimeAchievement == null && _count > 9999)
                {
                    isAchieved = true;
                    DateTimeAchievement = DateTime.Now;
                }
            }

            // 100.000 count trophies (x1)
            if (_trophieType == TrophieType.HighScore)
            {
                _count++;
                if (DateTimeAchievement == null && _count > 99999)
                {
                    isAchieved = true;
                    DateTimeAchievement = DateTime.Now;
                }
            }

            // 1.000.000 count trophies (x1)
            if (_trophieType == TrophieType.BreakablesCount)
            {
                _count++;
                if (DateTimeAchievement == null && _count > 999999)
                {
                    isAchieved = true;
                    DateTimeAchievement = DateTime.Now;
                }
            }


            if (_isDebug && _trophieType != TrophieType.BreakablesCount)
            {
                // debug information.
                System.Diagnostics.Debug.WriteLine(string.Empty);
                System.Diagnostics.Debug.WriteLine("TrophieType = {0}", _trophieType);
                System.Diagnostics.Debug.WriteLine("TrophieValue = {0}", _trophieValue);
                //System.Diagnostics.Debug.WriteLine("DateTimeAchievement = {0}", _dateTimeAchievement);
                System.Diagnostics.Debug.WriteLine("Count = {0}", _count);
                System.Diagnostics.Debug.WriteLine("IsHidden = {0}", _isHidden);
                System.Diagnostics.Debug.WriteLine("************************************************");
                    
            }

            return isAchieved;
        }

        public bool Validate(int points)
        {
            bool isAchieved = false;

            if (_trophieType == TrophieType.PointsMillion)
            {
                _count += points;
                if (DateTimeAchievement == null && _count > 999999)
                {
                    isAchieved = true;
                    DateTimeAchievement = DateTime.Now;
                    _isHidden = false;
                }
            }

            return isAchieved;
        }
        #endregion
    }
}
