using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using TmxContentLibrary;

namespace Breaker.Wp7.Xna4.Elements.Game
{
    class Debug
    {
       
        Texture2D _texture;
        SpriteFont _spriteFont;
        
        Map _map;
        Camera _camera;
        Ball _ball;

        float _fontScale = .7f;
        

        #region public content load

        public void LoadContent(ContentManager content)
        {
            // load texture (1px transparant) and font
            _texture = content.Load<Texture2D>("Textures/blank");
            _spriteFont = content.Load<SpriteFont>("Fonts/gamefont");
        }

        #endregion

        #region public draw


        public void Draw(SpriteBatch spriteBatch, GameTime gameTime, Camera camera, Map map, Ball ball)
        {
 
            _map = map;
            _camera = camera;
            _ball = ball;
            
            // draw background area 
            DrawBackgroundDebug(spriteBatch);

            // draw map collision
            DrawCollisionMapTileDebug(spriteBatch);

            // draw ball collision rectangle
            DrawCollisonBallDetectionRectangleDebug(spriteBatch);

            // draw ball collision rectangle
            DrawCollisonBallDetectionLinesDebug(spriteBatch);

            // draw ball velocity
            DrawDirectionDebug(spriteBatch);

        }

        #endregion

        #region draw debug information

        private void DrawBackgroundDebug (SpriteBatch spriteBatch)
        {
            // draw debug background
            Rectangle rectangleBackground
                = new Rectangle(0, 0, _camera.Width, _map.TileHeight);

            spriteBatch.Draw(_texture, rectangleBackground, new Color(0, 0, 0, .6f));
        }


        private void DrawDirectionDebug(SpriteBatch spriteBatch)
        {
            // draw velocity
            string txt
                = string.Format("X: {0}, Y: {1}", _ball.Direction.X, _ball.Direction.Y);

            spriteBatch.DrawString(
                _spriteFont, txt, new Vector2(24f, 6f), Color.Fuchsia, 0f, Vector2.Zero, _fontScale, SpriteEffects.None, 0);

        }

        //private void DrawDebug(SpriteBatch spriteBatch)
        //{
        //    var memuse = (long)DeviceExtendedProperties.GetValue("ApplicationPeakMemoryUsage");
        //    var maxmem = (long)DeviceExtendedProperties.GetValue("DeviceTotalMemory");
        //    memuse /= 1024 * 1024;
        //    maxmem /= 1024 * 1024;
        //    spriteBatch.DrawString(ScreenManager.Font, "Mem Usage: " + memuse + "/" + maxmem, new Vector2(10, 10), Color.Black);
        //    spriteBatch.DrawString(ScreenManager.Font, "Mem Usage: " + memuse + "/" + maxmem, new Vector2(9, 9), Color.White);
        //}

        private void DrawCollisionMapTileDebug(SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(
            //    _texture,
            //    new Vector2(
            //        _map.CollisionTileRectanlge.X, _map.CollisionTileRectanlge.Y) - _camera.Position,
            //        _map.CollisionTileRectanlge, new Color(176f, 32f, 4f, .4f));
        }


        private void DrawCollisonBallDetectionLinesDebug(SpriteBatch spriteBatch)
        {
            // set left line detection
            Rectangle x = new Rectangle(
              (int)_ball.Position.X - (int)_camera.Position.X, 0,
              2, _camera.Height
            );

            // set top line detection
            Rectangle x1 = new Rectangle(
                (int)_ball.Position.X + (int)_ball.Origin.X * 2 - (int)_camera.Position.X, 0,
                2, _camera.Height
            );

            // set right line detection
            Rectangle y = new Rectangle(
                0, (int)_ball.Position.Y,
                _camera.Width, 2
            );

            // set bottom line detection
            Rectangle y1 = new Rectangle(

                0, (int)_ball.Position.Y + (int)_ball.Origin.Y * 2,
                _camera.Width, 2
            );

            // draw left line detection
            spriteBatch.Draw(
                _texture, new Vector2(x.X, x.Y), x, Color.Green);

            // draw right line detection
            spriteBatch.Draw(
                _texture, new Vector2(x1.X, x1.Y), x1, Color.Green);

            // draw top line detection
            spriteBatch.Draw(
                _texture, new Vector2(y.X, y.Y) - _camera.Position, y, Color.Green);

            // draw bottom line detection
            spriteBatch.Draw(
                _texture, new Vector2(y1.X, y1.Y) - _camera.Position, y1, Color.Green);

        }

        private void DrawCollisonBallDetectionRectangleDebug(SpriteBatch spriteBatch)
        {
            // draw fill ball detection region
        //    spriteBatch.Draw(
        //        _texture,
        //        new Vector2(
        //            _ball.CollisonDetectionRectangle.X, _ball.CollisonDetectionRectangle.Y) - _camera.Position,
        //        _ball.CollisonDetectionRectangle, new Color(176f, 32f, 4f, .2f));
        }

        #endregion
    }
}
