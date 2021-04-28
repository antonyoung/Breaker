#region usings

using System;
//using System.Collections.Generic;
//using System.Linq;
using System.Text;
// xna
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
// breaker
using Breaker.Wp7.Xna4.Managers;

#endregion

namespace Breaker.Wp7.Xna4.Elements.Game
{
    public class Information
    {
        #region public static fields
 
        public int Lives = 10;
        public bool[] Extraball = new bool[4];

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA2211:Microsoft.Usage")]  
        public int Points = 0;
        public int Level = 1;

        #endregion

        #region private fields

        Texture2D _texture;
        SpriteFont _spriteFont;
        Rectangle _sourceRectangle;
        
        float _xOffsetStep = 20f;
        float _yOffset = 0;
        float _xOffset = 0;
        float _scale = 0.7f;

        TimeSpan _elapsedExtraBallAnimation;
        //TimeSpan _elapsedScoreAnimation;
        //TimeSpan _elapsedLevelAnimation;

        bool _isExtraBallAnimation = false;

        #endregion

        #region public content load

        public void LoadContent(ContentManager content)
        {
            // load texture (1px transparant) and font
            _texture = content.Load<Texture2D>("Textures/blank");
            _spriteFont = content.Load<SpriteFont>("Fonts/gamefont");
            //Reset();
        }


        //private static void Reset()
        //{
        //    this.Lives = 50;
        //    this.Extraball = new bool[4];
        //    this.Points = 0;
        //    this.Level = 1;
        //}

        #endregion

        #region public draw

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime, Rectangle sourceRectangle)
        {
            _sourceRectangle = sourceRectangle;

            // set offset positons to default;
            _xOffset = _xOffsetStep + 16;
            _yOffset = _sourceRectangle.Top + 48 / 2 - (_spriteFont.MeasureString("0").Y / 2 * _scale);

            // draw background rectangle with opacity
            DrawBackgroundInfromation(spriteBatch);

            // draw balls as lives
            DrawLivesInformation(spriteBatch);

            // draw xball
            bool isExtraBall = DrawIsExtraBallInformation(spriteBatch, gameTime);

            // draw level
            DrawLevelInformation(spriteBatch);

            // draw score
            DrawPointsInformation(spriteBatch);

            // extraball collected
            if (isExtraBall)
            {
                // reset extra ball
                Extraball = new bool[4];

                // add extra live
                Lives++;

                // start extraball animation
                _isExtraBallAnimation = true;
            }
        }

        #endregion

        #region private draw

        private void DrawBackgroundInfromation(SpriteBatch spriteBatch)
        {
            // set opacity
            float a = .0f;

            // draw information background
            spriteBatch.Draw(
                _texture, _sourceRectangle, new Color(0, 0, 0, a));
        }

        private void DrawLivesInformation(SpriteBatch spriteBatch)
        {
            int lives =  Lives < 0 ? 0 : Lives;

            string textLives = string.Format(
                ResourceManager.Text(ResourceManager.ResourceKeyName.InformationLives), lives);
            
            Vector2 vSize = _spriteFont.MeasureString(textLives);

            Vector2 vLivesPosition = new Vector2(
                _xOffset, _yOffset);

          
            // set new offset
            _xOffset += vSize.X * _scale +  _xOffsetStep;

            // draw lives
            spriteBatch.DrawString(
                _spriteFont, textLives,
                   vLivesPosition , Color.Red, 0, Vector2.Zero, _scale, SpriteEffects.None, 0);

        }

        private void DrawPointsInformation(SpriteBatch spriteBatch)
        {            
            string scoreText = string.Format(
                ResourceManager.Text(ResourceManager.ResourceKeyName.InformationPoints), Points);

            Vector2 vTextSize = _spriteFont.MeasureString(scoreText);
            
            Vector2 vScorePostion
                = new Vector2(
                    _sourceRectangle.Right - vTextSize.X * _scale - _xOffsetStep - 16f, 
                    _yOffset);

            // draw score
            spriteBatch.DrawString(
                _spriteFont, scoreText, vScorePostion, Color.Yellow, 0, Vector2.Zero, _scale, SpriteEffects.None, 0);

            // reset offset to right to left
            //_xOffset = vScorePostion.X - _xOffsetStep;
        }


        private void DrawLevelInformation(SpriteBatch spriteBatch)
        {
            // retrieve text to display
            string levelText = string.Format(
                ResourceManager.Text(ResourceManager.ResourceKeyName.InformationLevel), Level);
            
            // measure size of string
            Vector2 vSizeText = _spriteFont.MeasureString(levelText);


            // set position in sourcetectangle
            //Vector2 vLevelPostion
            //    = new Vector2(_xOffset, _yOffset);

            float x = 400f - vSizeText.X * _scale / 2f;

            Vector2 vLevelPostion = new Vector2(x, _yOffset); 
            // draw level information
            spriteBatch.DrawString(
                _spriteFont, levelText, vLevelPostion, Color.Green, 0, Vector2.Zero, _scale, SpriteEffects.None, 0);
            
            // set new offset
            //_offset += vLevel.X * _scale + _xOffset;f

        }


        /// <summary>
        /// Draws the Extraballs information based on their index
        /// in different colors and text. If all ExtraBalls are set
        /// to true, then the method becomes true.
        /// </summary>
        /// <param name="spriteBatch">used as the group of sprites to be drawn.</param>
        /// <param name="gameTime">used for the animation ExtraBalls</param>
        /// <returns>bool, used as complete set of extraballs.</returns>
        private bool DrawIsExtraBallInformation(SpriteBatch spriteBatch, GameTime gameTime)
        {
            // reset animation
            if (_isExtraBallAnimation)
            {
                // add time
                _elapsedExtraBallAnimation += gameTime.ElapsedGameTime;

                // animation time > time x
                if (_elapsedExtraBallAnimation.Seconds > 2.0)
                {
                    // reset animation values
                    _elapsedExtraBallAnimation = new TimeSpan();
                    _isExtraBallAnimation = false;
                }
            }


            // draw extraball text
            //string extraBallText
            //    = ResourceManager.Text(ResourceManager.ResourceKeyName.InformationExtraBall);

            // size of the extraball text       
            //Vector2 vTextSize = _spriteFont.MeasureString(extraBallText);

            // set extraball text positon
            //Vector2 vExtraBall = new Vector2(_xOffset, _yOffset);

            // draw extraball text
            //spriteBatch.DrawString(
            //    _spriteFont, extraBallText, vExtraBall, Color.Orange, 0, Vector2.Zero, _scale, SpriteEffects.None, 0);
            

            // draw extraball colors as colored text 
            string ballColor = string.Empty;

            // extraball validation
            bool isAllBalls = true;

            // draw colored ball true, false
            bool b = true; 

            // loop all available colored balls
            for (int i = 0; i < Extraball.Length; i++)
            {
                // validate extra ball value
                b = Extraball[i];

                if (!b)
                {
                    // is not complete
                    isAllBalls = false;
                }

                // is animation running
                if (_isExtraBallAnimation)
                {
                    // reset value to draw the extraball
                    b = true;

                    // draw ball based on time index
                    if (_elapsedExtraBallAnimation.Milliseconds % 3 != 0)
                    {
                        // reset value to draw the extra ball
                        b = false;
                    }
                }

                // set color and text based on index;
                Color color = new Color();
                switch (i)
                {
                    case 0:
                        color = Color.Red;
                        ballColor = b ? "R" : string.Empty;
                        break;
                    case 1:
                        color = Color.Green;
                        ballColor = b ? "G" : string.Empty;
                        break;
                    case 2:
                        ballColor = b ? "B" : string.Empty;
                        color = Color.Blue;
                        break;
                    case 3:
                        ballColor = b ? "Y" : string.Empty;
                        color = Color.Yellow;
                        break;
                }

                // set offset for the next extraball in the collection
                //_xOffset += i == 0 ? vTextSize.X * _scale + _xOffsetStep: 35;
                _xOffset += i == 0 ? _scale + _xOffsetStep : 35;

                // draw extraball with color
                spriteBatch.DrawString(
                    _spriteFont, ballColor, new Vector2(_xOffset, _yOffset), color, 0, Vector2.Zero, _scale, SpriteEffects.None, 0);
            }


            // set total offset for next information to be drawn 
            _xOffset += 35;

            // return all balls collected = extraball
            return isAllBalls;
        }

        #endregion
    }
}
