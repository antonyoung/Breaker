using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Breaker.Wp7.Xna4.Elements.Animations
{
    class MapTileCollisionAnimation
    {
        #region private fields

        Dictionary<Vector2, TimeSpan> AnimationList;

        Texture2D _texture;
        Vector2 _cameraPosition;
        int _milliseconds = 500;

        bool _isDebug = false;
 
        #endregion

        #region constructor

        public MapTileCollisionAnimation(ContentManager content)
        {
            AnimationList = new Dictionary<Vector2,TimeSpan>();

            _cameraPosition = Vector2.Zero;
            _texture = content.Load<Texture2D>("Textures/ball");
        }

        #endregion

        #region update and draw

        public void Update(GameTime gameTime, Vector2 cameraPosition)
        {
            if (_isDebug)
            {
                System.Diagnostics.Debug.WriteLine("START: anim MTCA: AnimationList.Count = {0}", AnimationList.Count);
            }
            // set new camera position, needed for drawing
            _cameraPosition = cameraPosition;

            // temp dictionary
            Dictionary<Vector2, TimeSpan> d = new Dictionary<Vector2, TimeSpan>();

            // loop threw animation list
            foreach (KeyValuePair<Vector2, TimeSpan> key in AnimationList)
            {
                // set current timespan of current key
                TimeSpan ts = key.Value + gameTime.ElapsedGameTime;
                
                // validate timespan
                if (ts.Milliseconds < _milliseconds)
                {

                    if (_isDebug) // only debugging info
                    {
                        System.Diagnostics.Debug.WriteLine("Anim: add temp vector({0}, {1})", key.Key.X, key.Key.Y);
                    }
                    // add to temp dictionary
                    d.Add(key.Key, ts);
                }
            }

            // set temp dictionary to animation list
            AnimationList = d;

            if (_isDebug)
            {
                System.Diagnostics.Debug.WriteLine("END: anim MTCA: AnimationList.Count = {0}", AnimationList.Count);
            }
        }


        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
         
            // loop all key value pairs
            foreach (KeyValuePair<Vector2, TimeSpan> key in AnimationList)
            {
                
                // get from current key the animation time value.
                TimeSpan ts = key.Value;
                
                // animate size and color based on animation time value
                float scaleFactor = 0.032f - ((float)ts.Milliseconds / gameTime.ElapsedGameTime.Milliseconds) * 0.0004f;
                float lerpFactor = ((float)ts.Milliseconds / gameTime.ElapsedGameTime.Milliseconds) * 0.06f;


                // draw vector position 'center map tile'
                Vector2 v = key.Key - _cameraPosition;
               
                // draw origin 'center'
                Vector2 origin = new Vector2(
                    _texture.Width * scaleFactor / 2,
                    _texture.Height * scaleFactor / 2);

                if (_isDebug)
                {
                    System.Diagnostics.Debug.WriteLine("TimeSpan.Milliseconds = {0}", ts.Milliseconds);
                    System.Diagnostics.Debug.WriteLine("MapTilePositon [{0}, {1}]", key.Key.X, key.Key.Y);
                    System.Diagnostics.Debug.WriteLine("LerpFactor = {0}", lerpFactor);
                    System.Diagnostics.Debug.WriteLine("ScaleFactor = {0}", scaleFactor);
                }

                // draw collison map tile animation.
                spriteBatch.Draw(_texture, Vector2.Subtract(v, origin), null, Color.Lerp(Color.Red, Color.Yellow, lerpFactor), 0f, origin, scaleFactor, SpriteEffects.None, 0);
               
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
            AnimationList.Clear();
        }

        /// <summary>
        /// Adds a vector to the list of vectors used for animation
        /// </summary>
        /// <param name="vector2">used as the vector to be animated</param>
        public void Add(Vector2 vector2)
        {
            // add vector to the List<Vector2>
            if (!AnimationList.ContainsKey(vector2))
            {
                AnimationList.Add(vector2, new TimeSpan());
            }
        }

        #endregion
    }
}
