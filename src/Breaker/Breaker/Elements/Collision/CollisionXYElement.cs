using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace Breaker.Wp7.Xna4.Elements.Collision
{
    public class CollisionXYElement:IComparer<CollisionXYElement>
    {
        #region private fields

        bool _isXCollision = false;
        bool _isYCollision = false;

        bool _isDebug = true;

        #endregion

        #region properties

        public bool IsXCollision
        {
            private set
            {
                _isXCollision = value;
            }
            get
            {
                return _isXCollision;
            }
            
        }
        
        public bool IsYCollision
        {
            private set
            {
                _isYCollision = value;
            }

            get
            {
                return _isYCollision;
            }

        }

        #endregion

        #region constructors

        public CollisionXYElement()
        {
            _isXCollision = false;
            _isYCollision = false;
        }

        public CollisionXYElement(bool isXCollision, bool isYCollision)
        {
            _isXCollision = isXCollision;
            _isYCollision = isYCollision;
        }

        #endregion

        public void Collision(KeyValuePair<Rectangle, Vector2> keyValue, Rectangle rectangleGameElement, Vector2 velocityGameElement)
        {

            // set collision rectangle player
            Rectangle rectangleMapTile = keyValue.Key;
            //Tile tile = keyValue.Value;
            Vector2 vectorPosition = keyValue.Value;

            // validate collision (rectangle based)
            if (rectangleGameElement.Intersects(rectangleMapTile))
            {
                if (_isDebug)
                {
                    System.Diagnostics.Debug.WriteLine(
                        "RectangleBall [{0}, {1}, {2}, {3}]", 
                        rectangleGameElement.Left, rectangleGameElement.Top, 
                        rectangleGameElement.Right, rectangleGameElement.Bottom);

                    System.Diagnostics.Debug.WriteLine(
                        "RectangleMapTile [{0}, {1}, {2}, {3}]", 
                        rectangleMapTile.Left, rectangleMapTile.Top, 
                        rectangleMapTile.Right, rectangleMapTile.Bottom);
                }

                HandleCollision(rectangleMapTile, vectorPosition, velocityGameElement);
            }           
        }

        private void HandleCollision(Rectangle mapTile, Vector2 position, Vector2 velocity)
        {
            bool isV = false;
            bool isH = false;

            int x = mapTile.X;
            int y = mapTile.Y;

            if (velocity.Y < 0)
            {
                if (position.Y - mapTile.Top >= 0 &&
                    position.Y - mapTile.Top <= 10)
                {
                    y = (int)position.Y - mapTile.Top;

                    if (_isDebug)
                    {
                        System.Diagnostics.Debug.WriteLine("delta Y = {0}", y);
                    }

                    if (y > 0)
                    {
                        isV = true;
                    }
                }
            }

            if (velocity.Y > 0)
            {

                if (mapTile.Bottom - position.Y >= 0 &&
                     mapTile.Bottom - position.Y <= 10)
                {
                    y = mapTile.Bottom - (int)position.Y;

                    if (_isDebug)
                    {
                        System.Diagnostics.Debug.WriteLine("delta Y = {0}", y);
                    }

                    if (y > 0)
                    {
                        isV = true;
                    }
                }
            }


            if (velocity.X > 0)
            {
                if (mapTile.Right - position.X >= 0 &&
                    mapTile.Right - position.X <= 10)
                {

                    x = mapTile.Right - (int)position.X;

                    if (_isDebug)
                    {
                        System.Diagnostics.Debug.WriteLine("delta X = {0}", x);
                    }

                    if (x > 0)
                    {
                        isH = true;
                    }
                }
            }

            if (velocity.X < 0)
            {
                //System.Diagnostics.Debug.WriteLine("X[{0}], VX-", gameMap.CollisionTileRectanlge.Left - pos.X);

                if (position.X - mapTile.Left >= 0 &&
                    position.X - mapTile.Left <= 10)
                {
                    x = (int)position.X - mapTile.Left;

                    if (_isDebug)
                    {
                        System.Diagnostics.Debug.WriteLine("delta X = {0}", x);
                    }

                    if (x > 0)
                    {
                        isH = true;
                    }
                }
            }

            if ((isV && isH))
            {
                if (_isDebug)
                {
                    System.Diagnostics.Debug.WriteLine(
                        "[{0}, {1}] = CollisionXYElement.HandleCollision [{2}, {3}]",isH,  isV, x, y);
                }

                isH = x < y ? true : false;
                isV = y < x ? true : false;
            }

            IsXCollision = isH;
            IsYCollision = isV;

            if (_isDebug)
            {
                System.Diagnostics.Debug.WriteLine(
                    "[{0}, {1}] = CollisionXYElement.HandleCollision [{2}, {3}]", isH, isV, x, y);
            }


        }


        public int Compare(CollisionXYElement x, CollisionXYElement y)
        {
            throw new NotImplementedException();
        }
    }
}
