#region usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;

using Microsoft.Devices.Sensors;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using TmxContentLibrary;

#endregion

namespace Breaker.Wp7.Xna4.Elements.Game
{
    /// <summary>
    /// Player class 
    /// </summary>
    class Player
    {
        #region static fields

        public static int PlayerSize = 1;
        public static int Count = 2;
        public static bool IsPlayerStateChanged = false;
        public static bool IsPlayerCountChanged = false;
        
        #endregion

        #region enums

        public enum PlayerState
        {
            Large,
            Normal,
            Small
        }

        #endregion

        #region public fields


        public Vector2 Postion;
        
        public Vector3 Vector3Accelormeter = Vector3.Zero;

        private PlayerState _playerState = PlayerState.Normal;
        public PlayerState State
        {
            get
            {
                return _playerState;
            }
            set
            {
                // depending on playerstate, set the player texture

                switch(value)
                {
                    case PlayerState.Small:

                        _playerTexture = _playerSmallTexture;
                       
                        break;
                    
                    case PlayerState.Large:
                    
                        _playerTexture = _playerLargeTexture;
                        break;
                    
                    default:
                    
                        _playerTexture = _playerNormalTexture;
                        break;
                }

                // set new origin
                _origin = new Vector2(_playerTexture.Width / 2, _playerTexture.Height / 2);

                // set new player state
                _playerState = value;

            }

        }

        #endregion

        #region private fields

        Accelerometer _accelormeter;

        Texture2D _playerSmallTexture;
        Texture2D _playerNormalTexture;
        Texture2D _playerLargeTexture;
        Texture2D _playerTexture;

        BoundingBox _boundingBoxPlayer;

        Vector2 _cameraPostion;
        Vector2 _origin;

        bool _isVertical;
        bool _isVisible = true;
        bool _isAnimation = false;
        bool _isDebug = false;

        float _xOffset = 0f;
        float _yOffset = 0f;

        TimeSpan _animPlayerState =  new TimeSpan();
        TimeSpan _animAdditionalPlayer = new TimeSpan();

        DisplayOrientation _displayOrientation;

        const int MAXSECONDS = 15;
        
        #endregion

        #region initialization

        /// <summary>
        /// Initilizes a new player for the game. 
        /// The player can be moved on the game screen based on the accelormeter reading vector.
        /// A player can only by default move in horizontal [x] direction. 
        /// </summary>
        /// <param name="playerPosition">used to postion the player on the worlmap.</param>
        /// <param name="cameraPostion">used to postion the player in the draw event correctly on the screen</param>
        /// <param name="isVertical">used to enable the player to move also in vertical direction</param>
        public Player(Vector2 playerPosition, BoundingBox boxPlayer, Vector2 cameraPostion, bool isVertical)
        {
            // set initilization values
            Postion = playerPosition;
            _cameraPostion = cameraPostion;
            _isVertical = isVertical;
            _boundingBoxPlayer = boxPlayer;       

            ResetDefaultValues();

            // add accelormeter and 
            // an event handler to be able to read the accelometer vector
            _accelormeter = new Accelerometer();
            _accelormeter.ReadingChanged += 
                new EventHandler<AccelerometerReadingEventArgs>(Accelormeter_ReadingChanged);
        }

        private void ResetDefaultValues()
        {
            PlayerSize = 1;
            Count = 1;
            IsPlayerStateChanged = false;
            IsPlayerCountChanged = false;
        }

        #endregion

        #region content

        /// <summary>
        /// Loads all drawable content of the player.
        /// </summary>
        /// <param name="content" see="ContentManger"></param>
        public void LoadContent(ContentManager content)
        {
            _playerSmallTexture = content.Load<Texture2D>("Textures/Player/small");
            _playerNormalTexture = content.Load<Texture2D>("Textures/Player/normal");
            _playerLargeTexture = content.Load<Texture2D>("Textures/Player/large");

            _playerTexture = _playerNormalTexture;
           
            _accelormeter.Start();
        }
        #endregion

        #region update and draw


        /// <summary>
        /// Updates all the player information, like player position, state of the player, multiplayers,
        /// collision with other game elements, etc.
        /// </summary>
        /// <param name="gameTime">used for animation times</param>
        /// <param name="ball">used for the game element to detect collision</param>
        /// <param name="camera">
        /// used for player positon correction,
        /// in the case that the camera is moving to another positon on the world map.
        /// </param>
        public void Update(GameTime gameTime, Ball ball, Camera camera, DisplayOrientation displayOrientation)
        {
            _displayOrientation = displayOrientation;

            // animate player state (small, normal, large) 
             AnimatePlayerState(gameTime);

            // correct any x animation collisons
            // keep players x position on line 
             Postion.X -= _xOffset;
             Postion.Y -= _yOffset;
            _xOffset = 0f;
            _yOffset = 0f;

            // 'default player' 
            if (!_isVertical)
            {
                // default: not able to move in y direction.
                // correct: y position based on camera y position.  
                Postion.Y = camera.Position.Y + camera.Height - 96;
            }
            else 
            {
                // animate double player
                AnimateDoulePlayer(gameTime);

                // check if camera has scrolled to another y position.
                if (camera.Position.Y != _cameraPostion.Y)
                {
                    // set y postion with the delta of the camera scroll 
                    Postion.Y -= _cameraPostion.Y - camera.Position.Y;

                    // set new camera postion
                    _cameraPostion = camera.Position;

                    // clamp y position to map
                    Postion.Y = MathHelper.Clamp(Postion.Y, camera.Position.Y + 96, camera.Position.Y + camera.Width- 148);

                }
            }

            // handle any collison with the game element ball.
            HandleCollision(ball);

          }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime, Camera gameCamera)
        {
            // draw player
            if (!_isVisible | _isAnimation) return;

            Rectangle rec = new Rectangle(0, 0, _playerTexture.Width, 24);

            spriteBatch.Draw(
                _playerTexture, Postion - gameCamera.Position, rec, Color.LightBlue, 0, _origin, 1f, SpriteEffects.None, 0);
        }
        #endregion

        #region accelormeter eventhandler

        private void Accelormeter_ReadingChanged(object sender, AccelerometerReadingEventArgs e)
        {
            if (_isVertical)
            {
                Vector3Accelormeter 
                    = new Vector3((float)e.X, (float)e.Y, (float)e.Z);

                double x = e.X < 0 ? e.X * -1 : e.X;

                // calulate new y position player 
                if (x > BreakerGame.BreakerSettings.YAngle)
                {
                    Postion.Y -= (float)e.X * BreakerGame.BreakerSettings.YMovement;
                }
                else
                {
                    Postion.Y += (float)e.X * BreakerGame.BreakerSettings.YMovement;
                }

                Postion.Y = MathHelper.Clamp(Postion.Y, _cameraPostion.Y + 4 * 48, _cameraPostion.Y + 480 - 3 * 48);
            }

            float delta = (float)e.Y * BreakerGame.BreakerSettings.XMovement;

            if (_displayOrientation == DisplayOrientation.LandscapeLeft)
            {
                // calculate new x position player
                Postion.X -= delta;
            }
            if (_displayOrientation == DisplayOrientation.LandscapeRight)
            {
                Postion.X += delta;
            }

            Postion.X = MathHelper.Clamp(Postion.X, 48 + _origin.X, 800 - 48 - 32 - _origin.X); 
        }
        #endregion

        #region collision detection

        private void HandleCollision(Ball ball)
        {
            // nothing todo
            if (!_isVisible) return;

            if (!ball.IsBallMovementDown) return;

            // set collision rectangle player
            Rectangle rectanglePlayer = new Rectangle(
                (int)Postion.X - (int)_origin.X, (int)Postion.Y - (int)_origin.Y, 
                _playerTexture.Width, _playerTexture.Height);

            // validate collision (rectangle based)
            if(ball.RectangleBall.Intersects(rectanglePlayer))
            {
                // detect x, y collision
                bool isYCollision = false;
                bool isXCollision = false;

                // used for the delta's x, y collision
                int x = 0;
                int y = 0;

                // validate x collision 
                if(rectanglePlayer.Right >= ball.RectangleBall.Left && ball.RectangleBall.Right > rectanglePlayer.Right) 
                {
                    x = rectanglePlayer.Right - ball.RectangleBall.Left;
                    isXCollision = true;
               }

                // validate 2nd x collision 
                if (ball.RectangleBall.Right >= rectanglePlayer.Left && ball.RectangleBall.Left < rectanglePlayer.Left )
                {
                    isXCollision = true;
                    x = ball.RectangleBall.Right - rectanglePlayer.Left;
                }
 
                    // validate y collision
                if (ball.RectangleBall.Bottom >= rectanglePlayer.Top || x == 0)
                {
                    isYCollision = true;
                    y = ball.RectangleBall.Bottom - rectanglePlayer.Top;
                }
                        
 
                // validate x collision (also on the delta's of x and y)
                if (isXCollision && x < y)
                {
                    ball.Direction = Vector2.Reflect(ball.Direction, new Vector2(1, 0));
                    
                    x = x < 5 ? 5 : x;
                    // pixelperfect
                    if (ball.RectangleBall.Right > rectanglePlayer.Left)
                    {
                        // add x collison animation
                        _xOffset = x;
                        Postion.X += _xOffset;
                    }
                    else
                    {
                        _xOffset = x * -1;
                        Postion.X += _xOffset;
                    }
                }
                else
                {
                    y = y < 5 ? 5 : y;
                    Postion.Y += y;
                    ball.Direction = Vector2.Reflect(ball.Direction, new Vector2(0, 1));
                }
               
                if (_isDebug) // debug info
                {
                    System.Diagnostics.Debug.WriteLine("### COLLISION: Ball - Player [{0}, {1}] ###", isXCollision, isYCollision);
                    System.Diagnostics.Debug.WriteLine("### COLLISION: Ball - Player [{0}, {1}] ###", x, y);
                }
                ball.Direction.X += (this.Vector3Accelormeter.X) * 4f;
               
              

            }
        }
        #endregion

        #region animations

        private void AnimatePlayerState(GameTime gameTime)
        {
            // set player state based on set index
            switch (PlayerSize)
            {
                case 0:
                    // small player 
                    State = PlayerState.Small;
                    break;
                case 2:
                    // large player
                    State = PlayerState.Large;
                    break;
                default:
                    // normal player
                    State = PlayerState.Normal;
                    break;
            }

            // validate if state has changed 
            if (IsPlayerStateChanged)
            {
                // if state not equal to Normal
                if (PlayerSize != 1)
                {
                    if (_isDebug)
                    {
                        System.Diagnostics.Debug.WriteLine("### RESTART PLAYER SATE ANIMATION ###");
                    }
                    // reset animation
                    _animPlayerState = new TimeSpan();
                }
                // reset state changed
                IsPlayerStateChanged = false;
            }
            
            // reset player state
            if (_playerState != PlayerState.Normal)
            {
                // set animation state time.
                _animPlayerState += gameTime.ElapsedGameTime;

                // reset animation state time 
                if (_animPlayerState.Seconds > 15)
                {
                    // set to normal index
                    PlayerSize = 1;
                    // set state to normal
                    State = PlayerState.Normal;

                    // reset animation state time.
                    _animPlayerState = new TimeSpan();
                    if (_isDebug)
                    {
                        System.Diagnostics.Debug.WriteLine("### STOP PLAYER STATE ANIMATION ###");
                    }
                }

            }
        }

        private void AnimateDoulePlayer(GameTime gameTime)
        {
            // is verical (double) player visible
            _isVisible = Player.Count > 1 ? true : false;

            if (IsPlayerCountChanged)
            {
                if (Count == 2 && _isVertical)
                {
                    if (_isDebug)
                    {
                        System.Diagnostics.Debug.WriteLine("### RESTART DOUBLE PLAYER ANIMATION ###");
                    }
                    _animAdditionalPlayer = new TimeSpan();
                    _isAnimation = false;
                    _isVisible = true;
                    IsPlayerCountChanged = false;
                }
            }

            if(_isVisible && _isVertical)
            {
                // set new timespan.
                _animAdditionalPlayer += gameTime.ElapsedGameTime;

                if (_animAdditionalPlayer.Seconds + 3 >= MAXSECONDS)
                {
                    _isAnimation = _animAdditionalPlayer.Milliseconds % 3 == 0 ? false : true;

                    if (_isDebug)
                    {
                        System.Diagnostics.Debug.WriteLine(
                            "IsAnimation: Player [{0}] % 2 = {1}", _isAnimation, _animAdditionalPlayer.Milliseconds % 2);
                    }
                }

                // check new elapsed doubleplayer time
                if (_animAdditionalPlayer.Seconds > MAXSECONDS)
                {
                    // reset doubleplayer
                    _animAdditionalPlayer = new TimeSpan();
                    _isVisible = false;
                    Player.Count = 1;
                    if (_isDebug)
                    {
                        System.Diagnostics.Debug.WriteLine("### STOP DOUBLE PLAYER ANIMATION ###");
                    }
                }
            }
        }
        #endregion
    }
}
