#region using

using System;

using System.Collections.Generic;
using System.Threading;

using Microsoft.Devices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Breaker.Wp7.Xna4.Data;
using Breaker.Wp7.Xna4.Elements.Animations;
using Breaker.Wp7.Xna4.Elements.Collision;

using TmxContentLibrary;

#endregion

namespace Breaker.Wp7.Xna4.Elements.Game
{
    public class Ball
    {

        #region public porperties

        public Vector2 Position;
        public Vector2 Direction;
        public Vector2 Origin;

        public Information GameInformation
        {
            get;
            private set;
        }

        public float Speed
        {
            get;
            private set;
        }

        public Rectangle RectangleBall
        {
            get 
            {
                return new Rectangle(
                    (int)Position.X, (int)Position.Y, 2 * (int)Origin.X, 2 * (int) Origin.Y);
            }
        }


        public bool IsBallMovementDown
        {
            get;
            private set;
        }

        #endregion

        #region private properties

        Texture2D _textureBall;

        float ballScale = .24f; //.28ff;
        
        const float SPEEDSTEP = 1.04f;
        const float DELTA = 00f;
        const float GRAVITY = .0981f;
        int _time = 0;

        // animations
        BallVectorAnimation _animationBallMovement;
        MapTileCollisionAnimation _animationMapTileCollision;
        TrophieAnimation _animationTrophies;

        Dictionary<Vector2, CollisionElement> _previousCollisions;

        List<Vector2> _previousPositions;
      
        bool _isStarted = false;
        bool _isRespaw = false;
        bool _isDebug = false;
        bool _isHitWall = false;
        bool _isArrowTrophie = false;
        bool _isTunnel = false;
        bool _isLostLive = false;
       
        Vector2 _vectorTunnel = Vector2.Zero;
        Vector2 _vectorModeratorAnimationBall = new Vector2(1, 10);

        TimeSpan animCount = TimeSpan.FromMilliseconds(0);
        TimeSpan _animRespawn = TimeSpan.FromSeconds(5);
        TimeSpan _animArrow = TimeSpan.FromSeconds(2);

        Trophies _trophies;
        SettingsData _settings;
        VibrateController _vibrate;

        
        #endregion

        #region constructor


        public Ball(Vector2 mapPosition, Information information)
        {
            Position = mapPosition;
            GameInformation = information;
            GameInformation.Extraball = new bool[4];
            _previousCollisions = new Dictionary<Vector2, CollisionElement>();
            _isStarted = false;
            _settings = new SettingsData();
            _vibrate = VibrateController.Default;

            _trophies = new Trophies();
            
            // add trophie achievement eventhandler
            _trophies.TrophieAchieved 
                += new Trophies.TrophieAchievedEventHandler(Trophie_Achieved);

       }

        #endregion

        #region content

        public void LoadContent(ContentManager content)
        {

            _textureBall = content.Load<Texture2D>("Textures/Player/ball");
            
            Origin = new Vector2(
                _textureBall.Width * ballScale / 2, 
                _textureBall.Height * ballScale / 2);
            
            // load content for animations
            _animationBallMovement = new BallVectorAnimation(content);
            _animationMapTileCollision = new MapTileCollisionAnimation(content);
            _animationTrophies = new TrophieAnimation(content); 
        }


        public void UnloadContent()
        {
            //spriteSheet = null;
        }

        #endregion

        #region public methods

        public void IncreaseSpeed()
        {
            Speed += Speed >= Cheats.MaxBallSpeed ? 0 : SPEEDSTEP;
        }

        public void Restart()
        {
            _isStarted = true;
        }

        public void Start()
        {
            Random rnd = new Random();

            double x = 0;
            while (x < 0.48)
            {
                x = rnd.NextDouble();
            }

            double y = 0;
            while (y < 0.48)
            {
                y = rnd.NextDouble();
            }
            
            if (x > y)
            {
                double t = y;
                y = x;
                x = t;
            }

            int z = rnd.Next(0, 6);
            Speed = 4f;

            if (z % 2 == 0)
            {
                x *= -1;
            }

            if (GameInformation.Level == 1 && Position.Y == 8920)
            {
                y *= -1;
            }

            Direction = new Vector2((float)x, (float)y);

            Direction.Normalize();
            // clear animations
            _animationBallMovement.Clear();
            _animationMapTileCollision.Clear();

            _previousCollisions.Clear();

            _isRespaw = true;

            // set trigger to start moving
            _isStarted = true;

            System.Diagnostics.Debug.WriteLine("Random.X.Y.Z = {0}, {1}, {2}", x, y, z);
        }

        public void Stop()
        {
            _trophies.SaveTrophiesData();

            _isStarted = false;
        }

        #endregion

        #region Update

        public void Update(GameTime gameTime, Map gameMap, Camera gameCamera)
        {
            _isHitWall = false;

            
            // is ball respawning
            if (IsRespawn(gameTime)) return;

            // is ball started
            if (!_isStarted) return;
            
            // validate lost live
            if (IsLostLive(gameCamera, gameMap)) return;

            _time 
                = gameTime.ElapsedGameTime.Seconds;

            if (gameCamera.Position.Y == 0)
            {
                // level complete !!

                _trophies.SaveTrophiesData();

                if (GameInformation.Level == 1)
                {
                    _trophies.AddTrophie(TrophieData.TrophieType.FirstLevel);
                }
                if (!_isLostLive)
                {
                    _trophies.AddTrophie(TrophieData.TrophieType.Careful);
                }
                GameInformation.Level++;
                return;
            }

            Position.Y += GRAVITY * _time / 1000;
            // ball is in tunnel
            if (_isTunnel)
            {
                Position += new Vector2(0, Direction.Y) * Speed;
                if (Position.Y <= _vectorTunnel.Y)
                {
                    _isTunnel = false;

                    // todo: notify tunnel trophie data
                    _trophies.TunnelTrophie();
                }
                return;
            }
            else
            {
                _previousPositions = CollisionPoints();
               
                Position += Direction * Speed;
            }


            IsBallMovementDown = _previousPositions[0].Y < Position.Y ? true : false;
          
            // update animations
            _animationMapTileCollision.Update(gameTime, gameCamera.Position);
            _animationBallMovement.Update(
                gameTime, gameCamera.Position, Vector2.Add(Position, Vector2.Divide(Origin, 2)));
            _animationTrophies.Update(gameTime);

            // collision detection
            HandleCollision(gameMap);

            // tunnel detection
            HandleTunnels(gameMap);

         }

        #endregion

        #region helpers

        void ResetBallPosition(Camera gameCamera, Map gameMap)
        {
            Vector2 position 
                = new Vector2(
                    gameCamera.Position.X + gameCamera.Width / 4 + 16, 
                    gameCamera.Position.Y + gameCamera.Height / 3);

            List<Vector2> positions = new List<Vector2>();
            int count = 0;

            do
            {
                if (position.Y > gameCamera.Position.Y + gameCamera.Height - gameMap.TileHeight * 2)
                {
                    position.Y = gameCamera.Position.Y;
                }

                if (position.X > gameCamera.Width - gameMap.TileWidth * 2)
                {
                    position.Y += gameMap.TileHeight;
                    position.X = gameMap.TileWidth;
                }
                else
                {
                    position.X += gameMap.TileWidth;
                }

                if(IsOkBallPosition(gameMap, position))
                {
                    positions.Add(position);
                    count++;
                }

            }
            while (count != 10 );

            Random rnd = new Random();
            int index = rnd.Next(0, count - 1);
            Position = positions[index];
        }

        bool IsOkBallPosition(Map gameMap, Vector2 position)
        {
            bool result = true;

            foreach (Layer l in gameMap.Layers)
            {
                if (l.Properties.Contains("IsPasable"))
                {
                    bool isParseable;
                    bool.TryParse(l.Properties["IsPasable"], out isParseable);
                    if (isParseable)
                    {
                        continue;
                    }
                }

                TileLayer tileLayer = l as TileLayer;

                List<Vector2> detectionPostions = new List<Vector2>();

                Vector2 v0 = new Vector2((int)position.X,(int)position.Y);
                
                Vector2 v1 = new Vector2(v0.X + 2 * Origin.X, v0.Y);
                Vector2 v2 = new Vector2(v0.X, v0.Y +2 * Origin.Y);
                Vector2 v3 = new Vector2(v1.X, v2.Y);

                detectionPostions.Add(v0);
                detectionPostions.Add(v1);
                detectionPostions.Add(v2);
                detectionPostions.Add(v3);

                Rectangle rectangleCollision = new Rectangle(
                    (int)v0.X, (int)v0.Y, 2 * (int)Origin.X, 2 * (int)Origin.Y);

                foreach (Vector2 v in detectionPostions)
                {
                    Vector2 tilePosition
                         = new Vector2((int)(v.X / gameMap.TileWidth), (int)(v.Y / gameMap.TileHeight));

                    Rectangle tileRectangle =
                         new Rectangle(
                             (int)tilePosition.X * gameMap.TileWidth, (int)tilePosition.Y * gameMap.TileHeight,
                             gameMap.TileWidth, gameMap.TileHeight);

                    Tile collisionTile = null;
                    try
                    {
                        collisionTile = tileLayer.Tiles[(int)tilePosition.X, (int)tilePosition.Y];

                        if (collisionTile != null
                            && tileRectangle.Intersects(rectangleCollision))
                        {
                            result = false;
                            break;
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                        result = false;
                        continue;
                    }
                }
            }

            return result;
        }

        bool IsRespawn(GameTime gameTime)
        {
            if (_isRespaw)
            {
                // set new respawn time.
                _animRespawn = _animRespawn.Subtract(gameTime.ElapsedGameTime);

                // reset respawn
                if (_animRespawn.Milliseconds <= 0)
                {
                    _isRespaw = false;

                    // reset respawn time.
                    _animRespawn = TimeSpan.FromSeconds(3);
                }
            }
            return _isRespaw;
        }

        bool IsLostLive(Camera gameCamera, Map gameMap)
        {
            bool isLost = false;

            if (gameCamera.Position.Y + gameCamera.Height <= Position.Y + 2 * Origin.Y)
            {
                _trophies.SaveTrophiesData();

                GameInformation.Lives--;

                // respawn player
                _isRespaw = true;
                _isLostLive = true;

                // set new ball postion
                ResetBallPosition(gameCamera, gameMap);

                // restart game for random velocity;
                this.Start();

                // no updates needed
                isLost = true;

                if (_isArrowTrophie)
                {
                    _trophies.AddTrophie(TrophieData.TrophieType.Vector);
                }

                if (GameInformation.Lives < 0)
                {
                    _trophies.AddTrophie(TrophieData.TrophieType.Hopeless);
                }
                
            }

            return isLost;
        }

        List<Vector2> CollisionPoints()
        {
            List<Vector2> points = new List<Vector2>();
            points.Add(new Vector2(RectangleBall.Left, RectangleBall.Top));
            points.Add(new Vector2(RectangleBall.Left, RectangleBall.Bottom));
            points.Add(new Vector2(RectangleBall.Right, RectangleBall.Top));
            points.Add(new Vector2(RectangleBall.Right, RectangleBall.Bottom));

            return points;
        }

        #endregion

        #region Animation


        //private void SetAnimationFrameIndex(GameTime gameTime)
        //{
        //    //if (Position != Target)
        //    //{
        //        // Move toward the target vector
        //        //Vector2 moveVect = Target-Position;
        //        Vector2 moveVect = Velocity;
                
        //        moveVect.Normalize();
        //        //Vector2 moveVect = Position + velocity;

        //        // Work out which animation row to use (which direction we're facing)
        //        currentRow = GetTileRow(moveVect);

        //        // Get the color from the wall layer tile 5 pixels along the movement vector
        //        //Color? collColor = gameMap.GetColorAt("Walls", Position + (moveVect * 5));

                
        //        // If there's no color data, we're not colliding
        //        //if (collColor != null)
        //        //{
        //            // There is color data, we're interested in anything that's not fully transparent
        //        //    if (collColor.Value.A > 0) collided = true;
        //        //}

        //        //bool collided = gameMap.isCollision("Walls", Position); 

        //        // Only move if we've not collided
        //        //if(!collided)
        //        //{
        //            // Move along the movement vector at our given speed
        //            //Position += moveVect * Speed;

        //            // Do some animation
        //            //animCount += gameTime.ElapsedGameTime;
        //            //if (animCount > animSpeed)
        //            //{
        //            //    animCount = TimeSpan.FromMilliseconds(0);

        //            //    currentFrame += 1;
        //            //    if (currentFrame == numFrames) currentFrame = 0;
        //            //}

        //            // Floats will never be exactly equal, so set position to target when we're close enough.
        //            //if (Vector2.Distance(Position, Target) > Speed)
        //            //{
        //                //Position = Target;
        //            //    currentFrame = 0;
        //            //}
        //        //}

        //    //    // Clamp to camera bounds
        //    //    Position.X = MathHelper.Clamp(Position.X, gameCamera.ClampRect.Left, gameCamera.ClampRect.Right);
        //    //    Position.Y = MathHelper.Clamp(Position.Y, gameCamera.ClampRect.Top, gameCamera.ClampRect.Bottom);
        //    //}

        //    // Pulsate the light!
        //    //lightPulseCycle = (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds * lightPulseSpeed) / 2.0f + .5f;
        //}

        #endregion

        #region draw

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime, Camera gameCamera)
        {
            // ball is in tunnel
            if (_isTunnel) return;

            // draw animations
            _animationBallMovement.Draw(spriteBatch);
            _animationMapTileCollision.Draw(spriteBatch, gameTime);
            _animationTrophies.Draw(spriteBatch);
            
            // draw gameball
            Vector2 rectangle
                = new Vector2(RectangleBall.X, RectangleBall.Y);
            
            spriteBatch.Draw(
                _textureBall, Position - gameCamera.Position, null, 
                Color.Yellow, 0f, Vector2.Zero, ballScale, SpriteEffects.None, 0);

            // virbration
            HandleVibrate();
        }


        #endregion

        #region tile properties

        void ReadTileProperties(Tile tile)
        {
            // set arrow trophie
            _isArrowTrophie = false;

            _trophies.BreakbaleTrophie();

            // points 
            if (tile.Properties.Contains("Points"))
            {
                int points;
                int.TryParse(tile.Properties["Points"], out points);
                switch (points)
                {
                    case 20:
                        _trophies.PointsTrophie(TrophieData.TrophieType.Points20);
                        break;
                    case 30:
                        _trophies.PointsTrophie(TrophieData.TrophieType.Points30);
                        break;
                    case 40:
                        _trophies.PointsTrophie(TrophieData.TrophieType.Points40);
                        break;      
                }

                // add additional points
                GameInformation.Points += points;

                if (GameInformation.Points > 9999)
                {
                    // Earn 10.000 points or more
                    _trophies.AddTrophie(TrophieData.TrophieType.LowScore);
                }

                if (GameInformation.Points > 99999)
                {
                    // Earn 100.000 points or more
                    _trophies.AddTrophie(TrophieData.TrophieType.HighScore);                    
                }
            }

            // extra ball
            if (tile.Properties.Contains("BallIndex"))
            {
                int i;
                int.TryParse(tile.Properties["BallIndex"], out i);

                // set extra ball color on / off based on index
                GameInformation.Extraball[i] = GameInformation.Extraball[i] ? false : true;

                bool isExtraBall = true;
                foreach (bool b in GameInformation.Extraball)
                {
                    if (!b)
                    {
                        isExtraBall = false;
                        break;
                    }
                }

                if (isExtraBall)
                {
                    _trophies.AddTrophie(TrophieData.TrophieType.ExtraBall);
                }
            }

            // vectors
            if (tile.Properties.Contains("VectorX"))
            {
           
                float xV;
                float.TryParse(tile.Properties["VectorX"], out xV);
                //float.TryParse(tile.Properties["VectorY"], out yV);

                // substract of current velocity
                int count = 10;
                do
                {
                    IncreaseSpeed();
                }
                while (count != 10);

                _isArrowTrophie = true;
            }

            // players count
            if (tile.Properties.Contains("PlayerCount"))
            {
           
                int i = 1;
                int.TryParse(tile.Properties["PlayerCount"], out i);

                _trophies.AddTrophie(TrophieData.TrophieType.Bonus);

                if (Player.Count == 2 && i == 1)
                {
                    // lost player trophie
                    _trophies.AddTrophie(TrophieData.TrophieType.SinglePlayer);
                }

                if (Player.Count == 1 && i == 2)
                {
                    // take it easy trophie
                    _trophies.AddTrophie(TrophieData.TrophieType.DoublePlayer);
                }

                // set player count
                Player.Count = i;
                Player.IsPlayerCountChanged = true;


            }

            // players size
            if (tile.Properties.Contains("PlayerSize"))
            {
           
                int i = 1;
                int.TryParse(tile.Properties["PlayerSize"], out i);

                Player.PlayerSize = i;
                Player.IsPlayerStateChanged = true;

                _trophies.AddTrophie(TrophieData.TrophieType.Bonus);

            }

            IncreaseSpeed();
        }

        #endregion

        #region collision detection

        private void HandleCollision(Map gameMap)
        {
            TileLayer l = gameMap.GetLayer("Breakables") as TileLayer;

            List<Vector2> postions = CollisionPoints();
              
            Dictionary<Vector2, CollisionElement> collisions = new Dictionary<Vector2, CollisionElement>();

            int index = -1;

            foreach (Vector2 v in postions)
            {
                index++;

                Vector2 tilePosition
                    = new Vector2(
                        (int) (v.X / gameMap.TileWidth), (int)(v.Y / gameMap.TileHeight));

                Tile collisionTile = null;

                try
                {
                    collisionTile = l.Tiles[(int)tilePosition.X, (int)tilePosition.Y];
                }
                catch (IndexOutOfRangeException)
                {
                    //;
                }

                if (collisionTile == null) continue;

                if (!collisions.ContainsKey(tilePosition))
                {
                    if (_isDebug)
                    {
                        System.Diagnostics.Debug.WriteLine(
                            "Adding collision tile position ({0}, {1})", tilePosition.X, tilePosition.Y);
                    }
                    CollisionElement element 
                        = new CollisionElement(collisionTile, _previousPositions[index], v, gameMap.TileWidth, gameMap.TileHeight);

                    collisions.Add(tilePosition, element);
                }
            }


            if (collisions.Count == 0)
            {
                //_previousCollisions.Clear();
                return;
            }

            List<Vector2> wallTiles = new List<Vector2>();

            foreach (KeyValuePair<Vector2, CollisionElement> keyValue in collisions)
            {

                // breakable
                if (keyValue.Value.GameTile.Properties.Contains("IsBreakable"))
                {
                    bool isBreakable = false;
                    bool.TryParse(keyValue.Value.GameTile.Properties["IsBreakable"], out isBreakable);

                    if (isBreakable)
                    {
                        // read breakable game properties
                        ReadTileProperties(keyValue.Value.GameTile);

                        // remove gametile from game map
                        l.RemoveTile(keyValue.Value.TileCurrent);

                        // add animation of breaking a breakable                       
                        _animationMapTileCollision.Add(new Vector2(keyValue.Key.X * gameMap.TileWidth + gameMap.TileWidth / 2,
                            keyValue.Key.Y * gameMap.TileHeight + gameMap.TileHeight / 2));
                    }
                    else
                    {
                        if (!_previousCollisions.ContainsKey(keyValue.Key))
                        {
                            _previousCollisions.Add(keyValue.Key, keyValue.Value);
                            _isHitWall = true;
                        }
                        else
                        {
                            wallTiles.Add(keyValue.Key);
                        }
                    }
                }
            }

            foreach (Vector2 v in wallTiles)
            {
                collisions.Remove(v);
            }

            wallTiles.Clear();

            // validate _previousCollisions
            foreach (KeyValuePair<Vector2, CollisionElement> keyValue in _previousCollisions)
            {
                if(!collisions.ContainsKey(keyValue.Key))
                {
                    wallTiles.Add(keyValue.Key);
                }
            }

            foreach(Vector2 v in wallTiles)
            {
                _previousCollisions.Remove(v);
            }

            if (collisions.Count == 0)
            {
                //_previousCollisions.Clear();
                return;
            }

            if (_isDebug)
            {
                System.Diagnostics.Debug.WriteLine(
                    "CollisionsCount: {0}, PreviousCollisionsCount: {1}", collisions.Count, _previousCollisions.Count);
            }
            Vector2 collisionVector = Vector2.Zero;

            if (collisions.Count > 2)
            {
                collisionVector = CollisionTriple();
            }

            if (collisions.Count == 2)
            {
                collisionVector = CollisionDouble(collisions);
            }

            if (collisions.Count == 1)
            {

                collisionVector = CollisionSingle(collisions);
            }

            if (collisionVector == Vector2.Zero)
            {
                Direction = Vector2.Reflect(Direction, new Vector2(1,0));
                Direction.Normalize();
                Direction = Vector2.Reflect(Direction, new Vector2(0,1));
                Direction.Normalize();
            }
            else
            {
                Direction = Vector2.Reflect(Direction, collisionVector);
                Direction.Normalize();
            }
            if (_isDebug)
            {
                System.Diagnostics.Debug.WriteLine("Direction {0}, {1}", Direction.X, Direction.Y);
            }
           
        }


        Vector2 CollisionSingle(Dictionary<Vector2, CollisionElement> collision)
        {
            if (_isDebug)
            {
                System.Diagnostics.Debug.WriteLine("** CollisionSingle **");
            }
            Vector2 vCurrent = new Vector2();
            Vector2 vPrevious = new Vector2();
         
            foreach (KeyValuePair<Vector2, CollisionElement> key in collision)
            {
                vCurrent = key.Value.TileCurrent;
                vPrevious = key.Value.TilePrevious;
            }

            Vector2 collisionVector = Vector2.Zero;

            if (vCurrent.X == 0)
            {
                collisionVector = new Vector2(1, 0);
                //Direction = new Vector2(Direction.X + DELTA, Direction.Y);
                Direction = new Vector2(Direction.X, Direction.Y);
                return collisionVector;

            }

            if (vCurrent.X == 15)
            {
                collisionVector = new Vector2(1, 0);
                //Direction = new Vector2(Direction.X + DELTA, Direction.Y);
                Direction = new Vector2(Direction.X, Direction.Y);
                return collisionVector;
            }
            if (_isDebug)
            {
                System.Diagnostics.Debug.WriteLine("Collision Tile = {0}, {1}", vCurrent.X, vCurrent.Y);
                System.Diagnostics.Debug.WriteLine("Ball Tile = {0}, {1}", vPrevious.X, vPrevious.Y);
            }
            if (vPrevious.X == vCurrent.X)
            {
                if (_isDebug)
                {
                    System.Diagnostics.Debug.WriteLine("Collisions.X");
                }
                Direction = new Vector2(Direction.X + DELTA, Direction.Y);
                collisionVector = new Vector2(0, 1);
                //return collisionVector;
            }
            if (vPrevious.Y == vCurrent.Y)
            {
                if (_isDebug)
                {
                    System.Diagnostics.Debug.WriteLine("Collisions.Y");
                }
                collisionVector = new Vector2(1, 0);
                Direction = new Vector2(Direction.X, Direction.Y + DELTA);
                //return collisionVector;            
            }
            if (_isDebug)
            {
                System.Diagnostics.Debug.WriteLine("** END: CollisionSingle **");
            }
            return collisionVector;
        }


        Vector2 CollisionDouble(Dictionary<Vector2, CollisionElement> collisions)
        {
            if (_isDebug)
            {
                System.Diagnostics.Debug.WriteLine("** CollisionDouble **");
            }
            int i = 0;
            float x = 0f; 
            float y = 0f;
            
            foreach (KeyValuePair<Vector2, CollisionElement> keyValue in collisions)
            {
                if (i == 0)
                {
                    x = keyValue.Value.TileCurrent.X;
                    y = keyValue.Value.TileCurrent.Y;
                }

                if (i == 1)
                {
                    if (x == keyValue.Value.TileCurrent.X)
                    {
                        if (_isDebug)
                        {
                            System.Diagnostics.Debug.WriteLine("Collisions.X");
                        }
                        x = 1f;
                        y = 0f;
                        Direction = new Vector2(Direction.X + DELTA, Direction.Y);
                        break;
                    }
                    if (y == keyValue.Value.TileCurrent.Y)
                    {
                        if (_isDebug)
                        {
                            System.Diagnostics.Debug.WriteLine("Collisions.Y");
                        }
                        x = 0f;
                        y = 1f;
                        Direction = new Vector2(Direction.X, Direction.Y + DELTA);
                        break;
                    }
                    else
                    {
                        if (_isDebug)
                        {
                            System.Diagnostics.Debug.WriteLine("Collisions.XY");
                        }
                        x = 1f;
                        y = 1f;
                        Direction = new Vector2(Direction.X + DELTA, Direction.Y + DELTA);
                        break;
                    }
                }

                 i++;

            }

            if (_isDebug)
            {
                System.Diagnostics.Debug.WriteLine("** END: CollisionDouble **");
            }
            Vector2 collisionVector = new Vector2(x, y);
            return collisionVector;
        }


        Vector2 CollisionTriple()
        {
            if (_isDebug)
            {
                System.Diagnostics.Debug.WriteLine("** CollisionTriple **");
            }
            Direction = new Vector2(Direction.X + DELTA, Direction.Y + DELTA);
                       
            Vector2 tripleCollision = Vector2.Zero;
           
            if (_isDebug)
            {
                System.Diagnostics.Debug.WriteLine("** END: CollisionTriple **");
            }
            return tripleCollision;
        }

        #endregion

        void HandleTunnels(Map gameMap)
        {
            TileLayer l = gameMap.GetLayer("Tunnels") as TileLayer;

            List<Vector2> postions = CollisionPoints();

            Vector2 tilePosition
                = new Vector2(
                    (int)(RectangleBall.Right / gameMap.TileWidth), (int)(RectangleBall.Bottom / gameMap.TileHeight));

            Tile tunnelTile = null;

            try
            {
                tunnelTile = l.Tiles[(int)tilePosition.X, (int)tilePosition.Y];
            }
            catch (IndexOutOfRangeException)
            {
                return;
            }

            if (tunnelTile == null) return;

            if (tunnelTile.Properties.Contains("IsStartPoint"))
            {
                bool isTunnel;
                bool.TryParse(tunnelTile.Properties["IsStartPoint"], out isTunnel);

                if (isTunnel)
                {
                    Vector2 vEnd = TunnelEndPosition(tilePosition, l);
                    _isTunnel = true;
                    _vectorTunnel = Vector2.Multiply(vEnd, gameMap.TileHeight);
                }
            }              
        }

        Vector2 TunnelEndPosition(Vector2 startPostion, TileLayer tunnelLayer)
        {
            bool isEndPosition = false;
            int index = (int)startPostion.Y - 1;

            do
            {
                Tile tunnelTile = null;
                
                try
                {
                    tunnelTile = tunnelLayer.Tiles[(int)startPostion.X, index];
                }
                catch (IndexOutOfRangeException)
                {
                    //;
                }

                if (tunnelTile == null)
                {
                    index--;
                }
                else
                {
                    if (tunnelTile.Properties.Contains("IsStartPoint"))
                    {
                        bool isTunnel;
                        bool.TryParse(tunnelTile.Properties["IsStartPoint"], out isTunnel);

                        if (!isTunnel)
                        {
                            isEndPosition = true;
                        }
                    }                 
                }
            }
            while (!isEndPosition);

            return new Vector2(startPostion.X, index);
        }

        void HandleVibrate()
        {
            if (!_settings.IsVibrate) return;

            if (!_isHitWall) return;

            _vibrate.Start(TimeSpan.FromMilliseconds(65));
        }

        #region Trophie Eventhandler

        void Trophie_Achieved(TrophieAchievedArgs e)
        {
            //Trophies.AchievedTrophies.Add(e.Trophie);
            _animationTrophies.AchievedTrophies.Add(e.Trophie);
        }
        #endregion
    }
}
