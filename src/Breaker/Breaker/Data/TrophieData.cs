#region usings

using System;
using System.Runtime.Serialization;
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
        //DateTime? _dateTimeAchievement = DateTime.Now;

        int _count;

        bool _isHidden;
        bool _isDebug = false;
        
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
            set
            {
                _count = value;
            }
        }
        
        public ResourceManager.ResourceKeyName TitleResourceKeyName;
        public ResourceManager.ResourceKeyName DescriptionResourceKeyName;

        public bool IsAchieved
        {
            get;
            set;
        }
        //public DateTime? DateTimeAchievement
        //{
        //    get
        //    {
        //        return _dateTimeAchievement;
        //    }
        //    set
        //    {
        //        _dateTimeAchievement = value;
        //    }
        //}

        #endregion

 
    }
}
