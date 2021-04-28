using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TmxContentLibrary;
using Microsoft.Xna.Framework;

namespace Breaker.Wp7.Xna4.Elements.Collision
{
    public class CollisionElement
    {
        #region public properties

        public Vector2 PositionCurrent
        {
            get;
            private set;
        }

        public Vector2 PositionPrevious
        {
            get;
            private set;
        }

        public Vector2 TileCurrent
        {
            get
            {
                return new Vector2(
                    (int)(PositionCurrent.X / _width), 
                    (int)(PositionCurrent.Y / _height));
            }
        }

        public Vector2 TilePrevious
        {
            get
            {
                return new Vector2(
                    (int)(PositionPrevious.X / _width),
                    (int)(PositionPrevious.Y / _height));
            }

        }

        public Tile GameTile
        {
            get;
            private set;
        }
        #endregion

        #region private properties
        
        int _width = 0;
        int _height = 0;

        #endregion

        #region constructor

        public CollisionElement(Tile tile, Vector2 previousPosition, Vector2 currentPosition, int tileWidth, int tileHeight)
        {
            GameTile = tile;
            PositionCurrent = currentPosition;
            PositionPrevious = previousPosition;
            _height = tileHeight;
            _width = tileWidth;
        }

        #endregion
    }
}
