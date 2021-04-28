using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TiledLib;

namespace Breaker.Wp7.Xna4
{
    public class Ball
    {
        #region public porperties

        public Vector2 Position;
        public Vector2 Target;

        public int Score = 0;
        public bool[] CurrentBalls = new bool[4];
        public float Speed;

        public Vector2 velocity; // = new Vector2(-2.5f, 6f);

        private Rectangle CollisonSourceRectangle
        {
            get
            {

                int left = (int)Position.X;
                int top = (int)Position.Y;

                if (velocity.X < 0)
                {
                    left = (int)Position.X + (int)ballOrigin.X * 2;
                }
                if (velocity.Y < 0)
                {
                    top = (int)Position.Y + (int)ballOrigin.Y * 2;
                }

                return new Rectangle(
                    left, top, (int)ballOrigin.X * 2, (int)ballOrigin.Y * 2);
            }
        }

        #endregion

        Texture2D spriteSheet;
        Texture2D shadowTexture;
        Texture2D infoTexture;
        //float _speed = .2f;
        Texture2D ballTexture;
        Texture2D testTexture;

        Vector2 ballOrigin;

        float ballScale = .07f;
        
        const float MAXSPEED = 9f;
        const float VELOCITYSTEPY = .04f;
        const float VELOCITYSTEPX = .02f;
        
        int frameWidth = 128;
        int frameHeight = 192;
        int numFrames = 6;
        int currentFrame = 0;
        int currentRow = 0;
        
        TimeSpan animSpeed = TimeSpan.FromMilliseconds(100);
        TimeSpan animCount = TimeSpan.FromMilliseconds(0);


        public Ball(Vector2 mapPosition)
        {
            Position = mapPosition;
            Target = Position;
            Speed = 3f;
            Random rnd = new Random();
            int x = rnd.Next(1, 6);
            int y = rnd.Next(5, 8);
            velocity = new Vector2((float)x, (float)y);
        }

        #region content

        public void LoadContent(ContentManager content)
        {
            //spriteSheet = content.Load<Texture2D>("characters/wizard");
            //shadowTexture = content.Load<Texture2D>("characters/shadow");
            //lightTexture = content.Load<Texture2D>("characters/light");
            
            ballTexture = content.Load<Texture2D>("Textures/ball");
            infoTexture = content.Load<Texture2D>("Textures/foreground");
            ballOrigin = new Vector2(ballTexture.Width * ballScale / 2, ballTexture.Height * ballScale / 2);
            testTexture = content.Load<Texture2D>("Textures/tile");

            //lightMaskBS = new BlendState()
            //{
            //    ColorSourceBlend = Blend.Zero,
            //    ColorDestinationBlend = Blend.SourceAlpha,

            //    AlphaSourceBlend = Blend.Zero,
            //    AlphaDestinationBlend = Blend.SourceAlpha,
            //}; 
        }


        public void UnloadContent()
        {
            spriteSheet = null;
        }

        #endregion



        public void Update(GameTime gameTime, Map gameMap, Camera gameCamera)
        {
            // set new position
            Position -= velocity;
            
            // start animation
            SetAnimationFrameIndex(gameTime);

            // check for any collison on current positon.
            foreach(Layer l in gameMap.Layers)
            {
                // detect collision in current layer
                string name = l.Name;

                int x, y;
                x = (int)Math.Round(Position.X);
                y = (int)Math.Round(Position.Y);
             
                bool isCollision = false;

                Vector2 pos = new Vector2(CollisonSourceRectangle.X, CollisonSourceRectangle.Y); 
                
                // detect if any collison has occured in current layer.
                isCollision = gameMap.CollisionRectangle(name, pos, CollisonSourceRectangle, velocity);

                if(!isCollision) continue; // check next layer

                //bool isV = false;
                //bool isH = IsHorizontalCollision(gameMap, l);
                //bool isV = IsVerticalCollison(gameMap, l);

                bool isH = gameMap.CollisionIsHorizontal(pos, CollisonSourceRectangle);
                bool isV = gameMap.CollisonIsVertical(pos, CollisonSourceRectangle);

                if (isH)
                {
                    velocity.X *= -1;
                    //velocity = Vector2.Reflect(velocity, new Vector2(0, 1));
                }
                if (isV)
                {
                    //velocity = Vector2.Reflect(velocity, new Vector2(1, 0));
                    velocity.Y *= -1;
                }
                
                //velocity = gameMap.CollisionUpdateVelocity(name, pos, velocity);
                
                // increase ball speed, because of collision
                if (velocity.X < 0)
                {
                    velocity.X -= velocity.X < MAXSPEED * - 1 ? 0 : VELOCITYSTEPX;
                }
                else
                {
                    velocity.X += velocity.X > MAXSPEED ? 0 : VELOCITYSTEPX;
                }

                if (velocity.Y < 0)
                {
                    velocity.Y -= velocity.Y < MAXSPEED * -1 ? 0 : VELOCITYSTEPY;
                }
                else
                {
                    velocity.Y += velocity.Y > MAXSPEED ? 0 : VELOCITYSTEPY;
                }


                // read layer properties
                if (l.Properties.Contains("Points"))
                {
                    int points;
                    int.TryParse(l.Properties["Points"], out points);
                    Score += points;
                }

                // validate later properties
                if (l.Properties.Contains("BallIndex"))
                {
                    int i;
                    int.TryParse(l.Properties["BallIndex"], out i);
                   
                    CurrentBalls[i] = CurrentBalls[i] ? false : true;
                }

                // validate later properties
                if (l.Properties.Contains("VectorX"))
                {
                    float xV, yV;
                    float.TryParse(l.Properties["VectorX"], out xV);
                    float.TryParse(l.Properties["VectorY"], out yV);
                    
                    velocity.X += xV;
                    velocity.Y += yV;
                }

                break;
            }

            if (Position.Y >= gameCamera.Target.Y + gameCamera.Height - gameMap.TileHeight)
            {
                Position.Y = gameCamera.Target.Y + gameCamera.Height - gameMap.TileHeight - 1;
                
                // player dead
                velocity.Y = velocity.Y * -1;
            }

            if (Position.Y <= gameMap.TileHeight)
            {
                Position.Y = gameMap.TileHeight + 1;
                velocity.Y *=  -1;
            }

            
            //Position.X = MathHelper.Clamp(
            //    Position.X, gameCamera.WorldRectangle.Left + gameMap.TileWidth + 1, gameCamera.WorldRectangle.Right - gameMap.TileWidth -1);
        }

        private void  SetAnimationFrameIndex(GameTime gameTime)
        {
            //if (Position != Target)
            //{
                // Move toward the target vector
                //Vector2 moveVect = Target-Position;
                Vector2 moveVect = velocity;
                
                moveVect.Normalize();
                //Vector2 moveVect = Position + velocity;

                // Work out which animation row to use (which direction we're facing)
                currentRow = GetTileRow(moveVect);

                // Get the color from the wall layer tile 5 pixels along the movement vector
                //Color? collColor = gameMap.GetColorAt("Walls", Position + (moveVect * 5));

                
                // If there's no color data, we're not colliding
                //if (collColor != null)
                //{
                    // There is color data, we're interested in anything that's not fully transparent
                //    if (collColor.Value.A > 0) collided = true;
                //}

                //bool collided = gameMap.isCollision("Walls", Position); 

                // Only move if we've not collided
                //if(!collided)
                //{
                    // Move along the movement vector at our given speed
                    //Position += moveVect * Speed;

                    // Do some animation
                    animCount += gameTime.ElapsedGameTime;
                    if (animCount > animSpeed)
                    {
                        animCount = TimeSpan.FromMilliseconds(0);

                        currentFrame += 1;
                        if (currentFrame == numFrames) currentFrame = 0;
                    }

                    // Floats will never be exactly equal, so set position to target when we're close enough.
                    //if (Vector2.Distance(Position, Target) > Speed)
                    //{
                        //Position = Target;
                    //    currentFrame = 0;
                    //}
                //}

            //    // Clamp to camera bounds
            //    Position.X = MathHelper.Clamp(Position.X, gameCamera.ClampRect.Left, gameCamera.ClampRect.Right);
            //    Position.Y = MathHelper.Clamp(Position.Y, gameCamera.ClampRect.Top, gameCamera.ClampRect.Bottom);
            //}

            // Pulsate the light!
            //lightPulseCycle = (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds * lightPulseSpeed) / 2.0f + .5f;
        }


        #region draw 

        public void Draw(SpriteBatch spriteBatch, Camera gameCamera)
        {
            //Rectangle sourceRect = new Rectangle(currentFrame * frameWidth, currentRow * frameHeight, frameWidth, frameHeight);
            //spriteBatch.Draw(spriteSheet, Position - gameCamera.Position, sourceRect, Color.White, 0f, drawOrigin, drawScale, SpriteEffects.None, 0);

            //spriteBatch.Draw(ballTexture, new Vector2(Position.X - drawOrigin.X * 2, Position.Y - drawOrigin.Y) - gameCamera.Position, null, Color.Yellow, 0f, drawOrigin, drawScale, SpriteEffects.None, 0);
            // draw position ball as square
            Rectangle x = new Rectangle(

                (int)Position.X, 0,
                2, gameCamera.Height
            );

            Rectangle x1 = new Rectangle(

                (int)Position.X + (int)ballOrigin.X * 2, 0,
                2, gameCamera.Height
            );
            Rectangle y = new Rectangle(

                0, (int)Position.Y,
                gameCamera.Width, 2
            );


            spriteBatch.Draw(infoTexture, x, Color.Green);
            spriteBatch.Draw(infoTexture, x1, Color.Green);
            spriteBatch.Draw(testTexture, y, Color.Green);

            spriteBatch.Draw(ballTexture, new Vector2(Position.X, Position.Y) - gameCamera.Position, null, Color.Yellow, 0f, ballOrigin, ballScale, SpriteEffects.None, 0);

            spriteBatch.Draw(testTexture, CollisonSourceRectangle, Color.Firebrick);

        }

        public void DrawShadow(SpriteBatch spriteBatch, Camera gameCamera)
        {
            spriteBatch.Draw(shadowTexture, Position - gameCamera.Position, null, Color.White * 0.3f, 0f, ballOrigin, ballScale, SpriteEffects.None, 0);
        }

        //public void DrawLight(SpriteBatch spriteBatch, Camera gameCamera, World gameWorld, RenderTarget2D ambientTarget, RenderTarget2D lightsTarget, GraphicsDevice gd)
        //{
        //    gd.SetRenderTarget(lightsTarget);
        //    gd.Clear(new Color(0, 0, 0, 0));

        //    Rectangle sourceRect = new Rectangle(((int)Position.X - (int)gameCamera.Position.X) - (lightTexture.Width / 2),
        //                                         ((int)(Position.Y - 15) - (int)gameCamera.Position.Y) - (lightTexture.Height / 2),
        //                                         lightTexture.Width,
        //                                         lightTexture.Height);

        //    sourceRect.Inflate((int)(lightPulseCycle * 5), (int)(lightPulseCycle * 5));

        //    spriteBatch.Begin();
        //    spriteBatch.Draw(ambientTarget, sourceRect, sourceRect, Color.White);
        //    spriteBatch.End();

        //    spriteBatch.Begin(SpriteSortMode.Immediate, lightMaskBS);
        //    spriteBatch.Draw(lightTexture, sourceRect, null, Color.White);
        //    spriteBatch.End();

        //    gd.SetRenderTarget(null);
        //}

        #endregion

        private int GetTileRow(Vector2 moveVect)
        {
            // Convert a movement vector to face direction
            float angle = ((float)Math.Atan2(-moveVect.Y, -moveVect.X) + MathHelper.TwoPi) % MathHelper.TwoPi;  
            int polarRegion = (int)Math.Round(angle * 8f / MathHelper.TwoPi) % 8;

            // Do a little bit of jigging because our spritesheet isn't in order
            polarRegion += 2;
            if (polarRegion > 7) polarRegion -= 8;

            return polarRegion;
        }

        #region horizontal collison

        bool IsHorizontalCollision(Map gameMap)
        {
            bool b = false;

            try
            {
                int x1, y1, x2, y2;
                x1 = (int)Math.Round(Position.X, 0);
                y1 = (int)Math.Round(Position.Y, 0);


                Vector2 tilePosition = new Vector2(x1 / gameMap.TileWidth, y1 / gameMap.TileHeight);
                int factor = (int)Math.Round(gameMap.TileWidth / velocity.X, 0);

                Vector2 previousPosition = Position - velocity; // *factor;

                x2 = (int)Math.Round(previousPosition.X, 0);
                y2 = (int)Math.Round(previousPosition.Y, 0);

                Vector2 tilePreviousPosition = new Vector2(x2 / gameMap.TileWidth, y2 / gameMap.TileHeight);

                System.Diagnostics.Debug.WriteLine("Current tile X, Y: {0}, {1}", tilePosition.X, tilePosition.Y);
                System.Diagnostics.Debug.WriteLine("Previous tile X, Y: {0}, {1}", tilePreviousPosition.X, tilePreviousPosition.Y);

                if (tilePosition.X + 1 == tilePreviousPosition.X || tilePosition.X - 1 == tilePreviousPosition.X)
                {
                    b = true;
                }

            }
            catch (IndexOutOfRangeException)
            {
                b = false;
            }
            System.Diagnostics.Debug.WriteLine("isHorizontalCollision: {0}", b);
            return b;
        }
        
        bool IsHorizontalCollision(Map map, Layer layer)
        {
                      
            int x, y;
            
            x = (int)Position.X;
            y = (int)Position.Y;

            Vector2 tilePosition = 
                new Vector2((x / map.TileWidth), (y / map.TileHeight));
            
            Rectangle tileRectangle = new Rectangle(
                (map.TileWidth * (int)tilePosition.X), (map.TileHeight * (int)tilePosition.Y), 
                map.TileWidth, map.TileHeight);

            Rectangle ballRectangle = new Rectangle(x, y, 20, 20);

            bool deltaX = false;

            if (!tileRectangle.Intersects(ballRectangle))  return deltaX;

            int top, left, right, bottom;

            top = Math.Max(ballRectangle.Top, tileRectangle.Top)
                - Math.Min(ballRectangle.Top, tileRectangle.Top);

            left = Math.Max(ballRectangle.Left, tileRectangle.Left)
                - Math.Min(ballRectangle.Left, tileRectangle.Left);

            right = Math.Max(ballRectangle.Right, tileRectangle.Right)
                - Math.Min(ballRectangle.Right, tileRectangle.Right);

            bottom = Math.Max(ballRectangle.Bottom, tileRectangle.Bottom)
                - Math.Min(ballRectangle.Bottom, tileRectangle.Bottom);

            Rectangle intersects = new Rectangle(top, left, right, bottom);

            if (ballRectangle.Top > tileRectangle.Top
                || ballRectangle.Bottom >= tileRectangle.Bottom)
            {
                deltaX = true;
            }
            return deltaX;
        }

        #endregion

        bool IsVerticaltalCollision(Map gameMap)
        {
            bool b = false;

            try
            {
                int x1, y1, x2, y2;
                x1 = (int)Math.Round(Position.X, 0);
                y1 = (int)Math.Round(Position.Y, 0);

                Vector2 tilePosition = new Vector2(x1 / gameMap.TileWidth, y1 / gameMap.TileHeight);

                int factor = (int)Math.Round(gameMap.TileHeight / velocity.X, 0);

                Vector2 previousPosition = Position - velocity; // *factor;

                x2 = (int)Math.Round(previousPosition.X, 0);
                y2 = (int)Math.Round(previousPosition.Y, 0);

                Vector2 tilePreviousPosition = new Vector2(x2 / gameMap.TileWidth, y2 / gameMap.TileHeight);

                System.Diagnostics.Debug.WriteLine("Current tile X, Y: {0}, {1}", tilePosition.X, tilePosition.Y);
                System.Diagnostics.Debug.WriteLine("Previous tile X, Y: {0}, {1}", tilePreviousPosition.X, tilePreviousPosition.Y);

                if (tilePosition.Y + 1 == tilePreviousPosition.Y || tilePosition.Y - 1 == tilePreviousPosition.Y)
                {
                    b = true;
                }

            }
            catch (IndexOutOfRangeException)
            {
                b = false;
            }
            System.Diagnostics.Debug.WriteLine("isVerticalCollision: {0}", b);
            return b;
        }   
    }
}
