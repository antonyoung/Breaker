using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Breaker.Wp7.Xna4.Screens.Elements
{
    class Shade:DrawableGameComponent
    {
        #region fields

        // enum shade direction
        Direction _shadeDirection = Direction.TopToBottom;

        SpriteBatch spriteBatch;
        //Texture2D tinyTexture;

        // debug information
        bool _isDebug = false;

        #endregion

        #region enum
        /// <summary>
        /// enumaration of the possible shading settings
        /// </summary>
        public enum Direction
        {
            LeftToRight,
            RightToLeft,
            TopToBottom,
            BottomToTop
        }

        #endregion

        #region properties

        /// <summary>
        /// 
        /// </summary>
        public float StartOffsetShadePoint = 0;
        
        /// <summary>
        /// used as the shading rectangle on the screen 
        /// </summary>
        public Rectangle DestionationRectangle;

        /// <summary>
        /// Direction of shading (opacity: none - full) 
        /// </summary>
        public Direction ShadeDirection
        {
            get
            {
                return _shadeDirection;
            }
            set
            {
                _shadeDirection = value;
            }
        }

        #endregion

        #region constructors

        public Shade(Game game) : base (game)
        {
        }

       

        #endregion

        #region public methods

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(this.GraphicsDevice);
            base.LoadContent();
        }

        /// <summary>
        /// Helper draw method, to draw shading 
        /// (top slowly the trophies disappear because of shading)
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
           
        }

        public override void Update(GameTime gameTime)
        {
           
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {

            spriteBatch.Begin();
            switch (ShadeDirection)
            {
                case Direction.LeftToRight:
                    DrawXBasedShade(spriteBatch);
                    break;

                case Direction.RightToLeft:
                    DrawXBasedShade(spriteBatch);
                    break;

                case Direction.BottomToTop:
                    DrawYBasedShade(spriteBatch);
                    break;

                default: // TopToBottom
                    DrawYBasedShade(spriteBatch);
                    break;

            }
            spriteBatch.End();
            base.Draw(gameTime);
        }

        protected virtual void DrawXBasedShade(SpriteBatch spriteBatch)
        {

        }

        protected virtual void DrawYBasedShade(SpriteBatch spriteBatch)
        {
            // set base rectangle to be drawn as shading
            Rectangle rectangle
                = new Rectangle(DestionationRectangle.Left, DestionationRectangle.Top, DestionationRectangle.Width, 1);

            float delta = DestionationRectangle.Bottom - rectangle.Top;
            float alfaFactor = 1 / (delta - StartOffsetShadePoint);

            if (_isDebug) // debug info
                System.Diagnostics.Debug.WriteLine("Shadow delta, alfaFactor startoffset: [X:{0}, Y:{1}, A:{2}]", delta, alfaFactor, StartOffsetShadePoint);

            // top screen shading
            // loop to 200 pixels
            for (int i = 0; i < delta; i++)
            {
                // set current pixel
                Vector2 v = new Vector2(DestionationRectangle.Left, DestionationRectangle.Top + i);

                //rectangle
                //    = new Rectangle(0, 0 + i, RectangleShade.Width, 1 + i);

                float a = 1;

                if (ShadeDirection == Direction.TopToBottom)
                {
                    a = i < StartOffsetShadePoint ? 1 : 1 - ((i - StartOffsetShadePoint) * alfaFactor);
                }
                else
                {
                    a = i < StartOffsetShadePoint ? 0 :  (i - StartOffsetShadePoint) * alfaFactor;
                }
                
                if (_isDebug) // debug info
                    System.Diagnostics.Debug.WriteLine("Shadow top screen: [X:{0}, Y:{1}, A:{2}]", v.X, v.Y, a);

                // color based on current opacity
                Color color = new Color(0, 0, 0, a);

                // draw rectangle with opacity or not.
                //spriteBatch.Draw(
                //    _screenManager.TextureBlank, v, rectangle, color, 0f, Vector2.Zero, 1, SpriteEffects.None, 0);

                //spriteBatch.Draw(
                //  _screenManager.TextureBlank, RectangleShade, rectangle, color);
            }
        }

        #endregion
    }
}
