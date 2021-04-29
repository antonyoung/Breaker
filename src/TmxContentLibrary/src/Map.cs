using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TmxContentLibrary
{
    
    // Class marked as compliant.
    [CLSCompliantAttribute(true)]

	/// <summary>
	/// A full map from Tiled.
	/// </summary>
	public class Map
	{
        //public enum UseForCollisionDetection { Triangles, Rectangles, Circles, PerPixel }

        //public static UseForCollisionDetection CDPerformedWith { get; set; }

		private readonly Dictionary<string, Layer> namedLayers = new Dictionary<string, Layer>();
		
		/// <summary>
		/// Gets the version of Tiled used to create the Map.
		/// </summary>
		public Version Version { get; private set; }

		/// <summary>
		/// Gets the orientation of the map.
		/// </summary>
		public Orientation Orientation { get; private set; }

		/// <summary>
		/// Gets the width (in tiles) of the map.
		/// </summary>
		public int Width { get; private set; }

		/// <summary>
		/// Gets the height (in tiles) of the map.
		/// </summary>
		public int Height { get; private set; }

		/// <summary>
		/// Gets the width of a tile in the map.
		/// </summary>
		public int TileWidth { get; private set; }

		/// <summary>
		/// Gets the height of a tile in the map.
		/// </summary>
		public int TileHeight { get; private set; }

		/// <summary>
		/// Gets a list of the map's properties.
		/// </summary>
		public PropertyCollection Properties { get; private set; }

		/// <summary>
		/// Gets a collection of all of the tiles in the map.
		/// </summary>
		public ReadOnlyCollection<Tile> Tiles { get; private set; }

		/// <summary>
		/// Gets a collection of all of the layers in the map.
		/// </summary>
		public ReadOnlyCollection<Layer> Layers { get; private set; }  

        bool _isDebug = false; 
        
        #region initialization

        internal Map(ContentReader reader) 
		{
			// read in the basic map information
			Version = new Version(reader.ReadString());
			Orientation = (Orientation)reader.ReadByte();
			Width = reader.ReadInt32();
			Height = reader.ReadInt32();
			TileWidth = reader.ReadInt32();
			TileHeight = reader.ReadInt32();
			Properties = new PropertyCollection();
			Properties.Read(reader);

			// create a list for our tiles
			List<Tile> tiles = new List<Tile>();
			Tiles = new ReadOnlyCollection<Tile>(tiles);

            //List<PropertyCollection> tileProperties = new List<PropertyCollection>();

            // read in each tile set
            int numTileSets = reader.ReadInt32();
            for (int i = 0; i < numTileSets; i++)
            {
                // get the id and texture
                int firstId = reader.ReadInt32();
                string tilesetName = reader.ReadString();

                Texture2D texture = reader.ReadExternalReference<Texture2D>();

                // Read in color data for collision purposes
                // You'll probably want to limit this to just the tilesets that are used for collision
                // I'm checking for the name of my tileset that contains wall tiles
                // Color data takes up a fair bit of RAM
                Color[] collisionData = null;
                //if (tilesetName == "Tileset1")
                //{
                //    collisionData = new Color[texture.Width * texture.Height];
                //    texture.GetData<Color>(collisionData);
                //}

                /*  <tileset firstgid="1" name="Breakables" tilewidth="48" tileheight="48">
                    <image source="TileSheets/Breakables.png" width="192" height="384"/>
                    <tile id="12">
                        <properties>
                            <property name="Points" value="10"/>
                        </properties>
                    </tile>
                 */

                // read in each individual tile
                int numTiles = reader.ReadInt32();
                for (int j = 0; j < numTiles; j++)
                {
                    int id = firstId + j;
                    if (id > 23 && id < 28)
                    {
                        //System.Diagnostics.Debug.WriteLine(reader.ReadString());
                    }
                    Rectangle source = reader.ReadObject<Rectangle>();
                    PropertyCollection props = new PropertyCollection();

                    props.Read(reader);

                    //props = tileProperties[id];
                    Tile t = new Tile(texture, source, props, collisionData);
                    while (id >= tiles.Count)
                    {
                        tiles.Add(null);
                    }
                    tiles.Insert(id, t);
                }
            }

			// read in all the layers
			List<Layer> layers = new List<Layer>();
			Layers = new ReadOnlyCollection<Layer>(layers);
			int numLayers = reader.ReadInt32();
			for (int i = 0; i < numLayers; i++)
			{
				Layer layer = null;

				// read generic layer data
				string type = reader.ReadString();
				string name = reader.ReadString();
				int width = reader.ReadInt32();
				int height = reader.ReadInt32();
				bool visible = reader.ReadBoolean();
				float opacity = reader.ReadSingle();
				PropertyCollection props = new PropertyCollection();
				props.Read(reader);

				// using the type, figure out which object to create
				if (type == "layer")
				{
					int[] data = reader.ReadObject<int[]>();
					layer = new TileLayer(name, width, height, visible, opacity, props, this, data);
				}
				else if (type == "objectgroup")
				{
					List<MapObject> objects = new List<MapObject>();

					// read in all of our objects
					int numObjects = reader.ReadInt32();
					for (int j = 0; j < numObjects; j++)
					{
						string objName = reader.ReadString();
						string objType = reader.ReadString();
						Rectangle objLoc = reader.ReadObject<Rectangle>();
						PropertyCollection objProps = new PropertyCollection();
						objProps.Read(reader);

						objects.Add(new MapObject(objName, objType, objLoc, objProps));
					}

					layer = new MapObjectLayer(name, width, height, visible, opacity, props, objects);
				}
				else
				{
					throw new Exception("Invalid type: " + type);
				}

				layers.Add(layer);
				namedLayers.Add(name, layer);
			}
		}
        #endregion

        #region public methods

        /// <summary>
		/// Gets a layer by name.
		/// </summary>
		/// <param name="name">The name of the layer to retrieve.</param>
		/// <returns>The layer with the given name.</returns>
		public Layer GetLayer(string name)
		{
			return namedLayers[name];
		}

        //public int TileIndex(Vector2 position, Vector2 velocity)
        //{

        //    return 1;
        //}

        //private Rectangle tileSourceRectangle(int tileIndex)
        //{
        //    return new Rectangle(
        //        (tileIndex % Width) * TileWidth,
        //        (tileIndex / Width) * TileHeight,
        //        TileWidth,
        //        TileHeight);
        //}

        /// <summary>
        /// The Rectangle of a Tile on the Map, based on the position on the Map.
        /// Can be used for Rectangle collision detection. 
        /// </summary>
        /// <param name="vectorMapPosition">used as the x, y coordinates on the map.</param>
        /// <returns>the Rectangle of a Tile on the Map</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1704:Microsoft.Naming")]  
        public Rectangle RectanlgeMapTile(Vector2 vectorMapPosition)
        {
            vectorMapPosition.X = (int)vectorMapPosition.X;
            vectorMapPosition.Y = (int)vectorMapPosition.Y;

            Vector2 tilePosition
                 = new Vector2((int)(vectorMapPosition.X / TileWidth), (int)(vectorMapPosition.Y / TileHeight));


            Rectangle tileRectangle =
                 new Rectangle(
                     (int)tilePosition.X * TileWidth, (int)tilePosition.Y * TileHeight,
                     TileWidth, TileHeight);

            return tileRectangle;
        }

        public Vector2 MapTileIndex(Vector2 vectorMapPosition)
        {
            vectorMapPosition.X = (int)vectorMapPosition.X;
            vectorMapPosition.Y = (int)vectorMapPosition.Y;

            return new Vector2(
                (int)(vectorMapPosition.X / TileWidth),
                (int)(vectorMapPosition.Y / TileHeight));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapLayer"></param>
        /// <param name="rectangleGameElement"></param>
        /// <returns></returns>
        public Dictionary<Rectangle, Vector2> CollisionMapLayerTiles(TileLayer tileLayer, Rectangle rectangleGameElement)
        {
            Dictionary<Rectangle, Vector2> mapLayerTilesCollisionDetection = new Dictionary<Rectangle, Vector2>();

            // add all corner positions of the game element rectangle
            List<Vector2> vectors = new List<Vector2>();

            vectors.Add(new Vector2(rectangleGameElement.Left, rectangleGameElement.Top));
            vectors.Add(new Vector2(rectangleGameElement.Right, rectangleGameElement.Top));
            vectors.Add(new Vector2(rectangleGameElement.Left, rectangleGameElement.Bottom));
            vectors.Add(new Vector2(rectangleGameElement.Right, rectangleGameElement.Bottom));

            foreach (Vector2 v in vectors) // loop all positions
            {
                // retrieve game map tile as rectangle
                Rectangle rectangleMapTile = RectanlgeMapTile(v);

                // validate: current map tile rectangle 
                if (!mapLayerTilesCollisionDetection.ContainsKey(rectangleMapTile))
                {
                    // retrieve the map tile as tile object
                    Vector2 tileIndex = MapTileIndex(v);

                    if (tileLayer == null)
                    {
                        throw new ArgumentNullException("tileLayer");
                    }

                    Tile mapTile = tileLayer.MapTile(tileIndex);

                    // validate map tile object
                    if (mapTile == null) continue;

                    // add map tile rectangle and the layer tile object to the collection;
                    mapLayerTilesCollisionDetection.Add(rectangleMapTile, v);
                }

            }

            if (_isDebug)
            {
                System.Diagnostics.Debug.WriteLine("MapCollisionTilesCount = {0}", mapLayerTilesCollisionDetection.Count);
            }

            return mapLayerTilesCollisionDetection;
        }

        #endregion

        #region drawing

        /// <summary>
        /// Draws all layers of the map
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch to use to render the map.</param>
        /// <param name="gameCamera">The camera to use for positioning.</param>
        public void Draw(SpriteBatch spriteBatch, Camera gameCamera)
        {
            foreach (var l in Layers)
            {
                if (!l.Visible)
                    continue;

                DrawLayer(spriteBatch, l, gameCamera, Vector2.Zero, 1.0f, Color.White * l.Opacity);

            }

            
        }

        /// <summary>
        /// Draws a single layer by layer name
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch to use to render the layer.</param>
        /// <param name="layerName">The name of the layer to draw.</param>
        /// <param name="gameCamera">The camera to use for positioning.</param>
        public void DrawLayer(SpriteBatch spriteBatch, string layerName, Camera gameCamera)
        {
            var l = GetLayer(layerName);

            if (!l.Visible)
                return;

            TileLayer tileLayer = l as TileLayer;
			if (tileLayer != null)
			{
                DrawLayer(spriteBatch, l, gameCamera, new Vector2(0,0), tileLayer.Opacity, Color.White);
            }
        }

        /// <summary>
        /// Draws a single layer by layer name, with a specified color
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch to use to render the layer.</param>
        /// <param name="layerName">The name of the layer to draw.</param>
        /// <param name="gameCamera">The camera to use for positioning.</param>
        public void DrawLayer(SpriteBatch spriteBatch, string layerName, Camera gameCamera, Color color)
        {
            var l = GetLayer(layerName);

            if (!l.Visible)
                return;

            TileLayer tileLayer = l as TileLayer;
            if (tileLayer != null)
            {
                DrawLayer(spriteBatch, l, gameCamera, new Vector2(0, 0), tileLayer.Opacity, color);
            }
        }

        /// <summary>
        /// Draws a single layer as shadows, by layer name
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch to use to render the layer.</param>
        /// <param name="layerName">The name of the layer to draw.</param>
        /// <param name="gameCamera">The camera to use for positioning.</param>
        /// <param name="shadowOffset">Pixel amount to offset the shadowing by.</param>
        /// <param name="alpha">Shadow opacity</param>
        public void DrawLayer(SpriteBatch spriteBatch, string layerName, Camera gameCamera, Vector2 shadowOffset, float alpha)
        {
            var l = GetLayer(layerName);

            if (!l.Visible)
                return;

            TileLayer tileLayer = l as TileLayer;
			if (tileLayer != null)
			{
                DrawLayer(spriteBatch, l, gameCamera, shadowOffset, alpha, Color.Black);
            }
        }


        /// <summary>
        /// Draws a single layer of the map
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch to use to render the layer.</param>
        /// <param name="tileLayer">The layer to draw.</param>
        /// <param name="gameCamera">The camera to use for positioning.</param>
        /// <param name="offset">A pixel amount to offset the tile positioning by</param>
        /// <param name="alpha">Layer opacity.</param>
        /// <param name="color">The color to use when drawing.</param>
        public void DrawLayer(SpriteBatch spriteBatch, Layer layer, Camera gameCamera, Vector2 offset, float alpha, Color color)
        {
            if (layer == null)
            {
                throw new ArgumentNullException("layer");
            }

            if (!layer.Visible)
                return;

            if (spriteBatch == null)
            {
                throw new ArgumentNullException("spriteBatch");
            }

            TileLayer tileLayer = layer as TileLayer;
            if (tileLayer != null)
            {
                // Calculate scroll offset
                Vector2 scrollOffset = Vector2.One;

                if (tileLayer.Properties.Contains("ScrollOffsetX"))
                    scrollOffset.X = float.Parse(tileLayer.Properties["ScrollOffsetX"], System.Globalization.CultureInfo.InvariantCulture);
                if (tileLayer.Properties.Contains("ScrollOffsetY"))
                    scrollOffset.Y = float.Parse(tileLayer.Properties["ScrollOffsetY"], System.Globalization.CultureInfo.InvariantCulture);

                Vector2 drawPos = new Vector2(-TileWidth, -TileHeight);

                if (gameCamera == null)
                {
                    throw new ArgumentNullException("gameCamera");
                }

                // Calculate remainders
                drawPos.X -= (int)(gameCamera.Position.X * scrollOffset.X) % TileWidth;
                drawPos.Y -= (int)(gameCamera.Position.Y * scrollOffset.Y) % TileHeight;

                // Add shadow offset
                drawPos += offset;

                // Convert the draw position to ints to avoid odd artifacting
                drawPos.X = (int)drawPos.X;
                drawPos.Y = (int)drawPos.Y;

                float originalX = drawPos.X;

                for (int y = (int)((gameCamera.Position.Y * scrollOffset.Y) / TileHeight)-1; y < (((gameCamera.Position.Y * scrollOffset.Y) + gameCamera.Height) / TileHeight)+1; y++)
                {
                    for (int x = (int)((gameCamera.Position.X * scrollOffset.X) / TileWidth)-1; x < (((gameCamera.Position.X * scrollOffset.X) + gameCamera.Width) / TileWidth)+1; x++)
                    {
                        if (x >= 0 && x < tileLayer.Width && y > 0 && y < tileLayer.Height)
                        {
                            Tile tile = tileLayer.Tiles[x, y];

                            if (tile != null)
                            {

                                spriteBatch.Draw(tile.Texture, drawPos, tile.Source, color * alpha);

                            }
                        }

                        drawPos.X += TileWidth;
                    }
                    drawPos.X = originalX;
                    drawPos.Y += TileHeight;
                }
            }

        }

        #endregion

        #region collision detection

        public Color? GetColorAt(string layerName, Vector2 position)
        {
            var l = GetLayer(layerName);

            TileLayer tileLayer = l as TileLayer;

            position.X = (int)position.X;
            position.Y = (int)position.Y;

            Vector2 tilePosition = new Vector2((int)(position.X / TileWidth), (int)(position.Y/TileHeight));

            Tile collisionTile = tileLayer.Tiles[(int)tilePosition.X, (int)tilePosition.Y];

            if (collisionTile == null)
                return null;

            if (collisionTile.CollisionData != null)
            {
                int positionOnTileX = ((int)position.X - (((int)position.X / TileWidth) * TileWidth));
                int positionOnTileY = ((int)position.Y - (((int)position.Y / TileHeight) * TileHeight));
                positionOnTileX = (int)MathHelper.Clamp(positionOnTileX, 0, TileWidth);
                positionOnTileY = (int)MathHelper.Clamp(positionOnTileY, 0, TileHeight);

                int pixelCheckX = (collisionTile.Source.X) + positionOnTileX;
                int pixelCheckY = (collisionTile.Source.Y) + positionOnTileY;

                //Texture2D tex = collisionTile.Texture;
                //Color[] texColors = new Color[pixelCheckY * pixelCheckX];
                //tex.GetData<Color>(texColors);

                //Color?[] clrs = new Color?[(pixelCheckY * 48) + pixelCheckX];

                //return clrs;

                //collisionTile.Texture.GetData<

                return collisionTile.CollisionData[(pixelCheckY * collisionTile.Texture.Width) + pixelCheckX];
            }
            else
            {
                return null;
            }
        }

        
        //public bool IsCollision(string layerName, Vector2 position, Rectangle collideRectangle, Vector2 velocity)
        //{
        //    bool rec = false;

        //    var l = GetLayer(layerName);

        //    if (l.Properties.Contains("IsPasable"))
        //    {
        //        bool isParseable;
        //        bool.TryParse(l.Properties["IsPasable"], out isParseable);
        //        if (isParseable)
        //        {
        //            return false;
        //        }
        //    }

        //    TileLayer tileLayer = l as TileLayer;

        //    position.X = (int)position.X;
        //    position.Y = (int)position.Y;

        //   Vector2 tilePosition 
        //        = new Vector2( (int)(position.X / TileWidth), (int)(position.Y / TileHeight) );

        //   Rectangle tileRectangle = 
        //        new Rectangle(
        //            (int)tilePosition.X * TileWidth, (int)tilePosition.Y * TileHeight,
        //            TileWidth, TileHeight);

        //    Tile collisionTile = null;

        //    try
        //    {
        //        collisionTile = tileLayer.Tiles[(int)tilePosition.X, (int)tilePosition.Y];

        //        if (collisionTile != null 
        //            && tileRectangle.Intersects( collideRectangle ))
        //        {

        //            CollisionTileRectanlge = tileRectangle;

        //            //System.Diagnostics.Debug.WriteLine(
        //            //    "### NEW HIT [{0},{1}] ###", tilePosition.X, tilePosition.Y);
        //            //System.Diagnostics.Debug.WriteLine("MapTilePosition.X = {0}, MapTilePostion.Y = {1}", tilePosition.X * TileWidth, tilePosition.Y * TileHeight);
        //            //System.Diagnostics.Debug.WriteLine("Position.X = {0}, Postion.Y = {1}", position.X, position.Y);
        //            //System.Diagnostics.Debug.WriteLine("Velociy.X = {0}, velocity.Y = {1}", velocity.X, velocity.Y);

        //            rec = true;
                   
        //            if (rec && l.Properties.Contains("IsBreakable"))
        //            {
        //                bool isBreakable;
        //                bool.TryParse(l.Properties["IsBreakable"], out isBreakable);

        //                if (isBreakable)
        //                {
        //                    tileLayer.Tiles[(int)tilePosition.X, (int)tilePosition.Y] = null;
        //                }
        //            }
        //        }

        //    }
        //    catch (IndexOutOfRangeException)
        //    {
        //        System.Diagnostics.Debug.WriteLine("* IndexOutOfRangeException");
        //        // respawn ball

        //        return rec;
        //    }
        //    return rec;
        //}

        #endregion
    }
}
