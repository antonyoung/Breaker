using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Breaker.Wp7.Xna4.Elements.Animations
{
    class BallVectorAnimation
    {
        #region private fields

        List<Vector2> _animationList;

        TimeSpan _tsAnimation;
        Texture2D _texture;
        Vector2 _cameraPosition;

        //bool _isDebug = false;

        #endregion

        #region constructor

        public BallVectorAnimation(ContentManager content)
        {
            // init new vector list
            _animationList = new List<Vector2>();
            
            // reset amimation time and camera postion
            _tsAnimation = new TimeSpan();
            _cameraPosition = Vector2.Zero;

            // load content into the contentmanager
            _texture = content.Load<Texture2D>("Textures/ball");
        }

        #endregion

        #region draw and update

        public void Update(GameTime gameTime, Vector2 cameraPosition, Vector2 ballPostion)
        {
            if (_animationList.Count > 80)
            {
                _animationList.RemoveAt(0);            
            }

            _animationList.Add(ballPostion);

            _cameraPosition = cameraPosition;
            _tsAnimation += gameTime.ElapsedGameTime;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // draw vector animation
            int count = 0;
            float s = .0f;

        
            if (_tsAnimation.Milliseconds > 5)
            {
                _tsAnimation = new TimeSpan();

                foreach (Vector2 v in _animationList)
                {

                    if (count % 10 == 0)
                    {
                        spriteBatch.Draw(_texture, v - _cameraPosition, null, Color.Lerp(Color.Purple, Color.Orange, s * 30), 0f, Vector2.Zero, s, SpriteEffects.None, 0);
                        //s += .01f;
                        s += .004f;
                    }
                    count++;

                }
            }
        }

        #endregion

        #region animation list methods

        /// <summary>
        /// Clears the list of vectors used for animation.
        /// </summary>
        public void Clear()
        {
            // clear List<Vector2>
            _animationList.Clear();
        }

        /// <summary>
        /// Adds a vector to the list of vectors used for animation
        /// </summary>
        /// <param name="vector2">used as the vector to be animated</param>
        public void Add(Vector2 vector2)
        {
            // add vector to the List<Vector2>
            if(!_animationList.Contains(vector2))
            {
                _animationList.Add(vector2);
            }
        }

        #endregion
    }
}
