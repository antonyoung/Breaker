using System;
using Microsoft.Xna.Framework;

namespace TmxContentLibrary
{
	/// <summary>
	/// A map layer containing tiles.
	/// </summary>
	public class TileLayer : Layer
	{
		/// <summary>
		/// Gets the layout of tiles on the layer.
		/// </summary>
		public Tile[,] Tiles { get; private set; }
        public Tile[] TilesArray { get; private set; }

        public Tile MapTile(Vector2 vectorMapTileIndex)
        {
            Tile t = null;

            try
            {
                t = Tiles[(int)vectorMapTileIndex.X, (int)vectorMapTileIndex.Y];
            }
            catch (IndexOutOfRangeException)
            {
                // do nothing no tile
            }

            return t;
        }

        public void RemoveTile(Vector2 vectorMapTileIndex)
        {
            try
            {
                Tiles[(int)vectorMapTileIndex.X, (int)vectorMapTileIndex.Y] = null;
            }
            catch (IndexOutOfRangeException)
            {
                // do nothing no tile
            }
        }

		internal TileLayer(string name, int width, int height, bool visible, float opacity, PropertyCollection properties, Map map, int[] data)
			: base(name, width, height, visible, opacity, properties)
		{
			Tiles = new Tile[width, height];
            TilesArray = new Tile[width * height];
            // data is left-to-right, top-to-bottom
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					int index = data[y * width + x];
					Tiles[x, y] = map.Tiles[index];
                    TilesArray[index] = map.Tiles[index]; 
				}
			}
		}
	}
}
