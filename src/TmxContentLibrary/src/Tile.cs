using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;

namespace TmxContentLibrary
{
    public class Tile 
	{
		public Texture2D Texture { get; private set; }
		public Rectangle Source { get; private set; }
		public PropertyCollection Properties { get; private set; }
        public Color[] CollisionData { get; private set; }

		internal Tile(Texture2D texture, Rectangle source, PropertyCollection properties, Color[] collisionData)
		{
			Texture = texture;
			Source = source;
			Properties = properties;
            CollisionData = collisionData;
            //CollisionData = new Color[] { new Color(0, 0, 0, 1), new Color(255, 255, 255, 1) };
		}

        
    }
}
