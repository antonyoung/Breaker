#region usings

using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TmxContentLibrary;

#endregion

namespace TmxContentLibrary
{
    /// <summary>
    /// 
    /// </summary>
    public class Camera
    {

        #region properties

        /// <summary>Width, Game.Viewport.Width</summary>
        public int Width { get; private set; }
        
        /// <summary>Height, Game.Viewport.Height</summary>
        public int Height { get; private set; }

        /// <summary>Camera Scroll target positon on the WorldRectangle from current Camera postion.</summary>
        public Vector2 Target { get; set; }

        /// <summary>Scroll Position, Game.Viewport.Postion WorldRectangle</summary>
        public Rectangle WorldRectangle { get; set; }

        #endregion

        float Speed = 2f;
        const float MAXTARGETHEIGHT = 48f;

        int _offsetX;
        int _offsetY;
        
        Vector2 _position  = new Vector2(0, 0);

        public Vector2 Position
        {
            get
            {
                return new Vector2(
                    _position.X + _offsetX,
                    _position.Y + _offsetY);
            }

            //private set
            //{
            //    _position.Y = -_offsetX;
            //    _position.Y = -_offsetY;
            //}
        }

        /// <summary>
        /// Initialise the camera, using the game map to define the boundaries
        /// </summary>
        /// <param name="vp">Graphics viewport</param>
        /// <param name="map">Game Map</param>
        public Camera(Viewport vp, Map map)
            : this
                (vp, new Rectangle(0, 0, (map.Width * map.TileWidth), (map.Height * map.TileHeight)))
        { }

        /// <summary>
        /// Initialise the camera, using a custom rectangle to define the boundaries
        /// </summary>
        /// <param name="vp">Graphics viewport</param>
        /// <param name="gameWorld">A rectangle defining the map boundaries, in pixels</param>
        public Camera(Viewport vp, Rectangle gameWorld)
        {
            Width = vp.Width;
            Height = vp.Height;

            WorldRectangle = gameWorld;

             // set camera position on world map.
            //_offsetX = (WorldRectangle.Width - Width) / 2;
            _offsetX = -16;
            _offsetY = 0;

            _position.X = WorldRectangle.X;
            _position.Y = WorldRectangle.Height;
            
            // set camera postion target (used for scroll) 
            Target = new Vector2(_position.X, _position.Y);
        }

        /// <summary>
        /// Update the camera
        /// </summary>
        public void Update()
        {
            // Clamp target to map/camera bounds (only y scroll used)
            //Target.X = (int)MathHelper.Clamp(Target.X, ClampRect.X, ClampRect.Width - Width);
            this.Target = new Vector2(Target.X, (int)MathHelper.Clamp(Target.Y, WorldRectangle.Y, WorldRectangle.Height ));
            
            //Target.Y = Target.Y < MAXTARGETHEIGHT ? MAXTARGETHEIGHT : Target.Y;
            
            // Move camera toward target
            _position = Vector2.SmoothStep(_position, Target, Speed);

        }
    }
}
