using System.Collections.Generic;
using System.Xml;

namespace TmxContentPipeline
{
	public class MapObjectLayerContent : LayerContent
	{
		public List<MapObjectContent> Objects = new List<MapObjectContent>();

		public MapObjectLayerContent(XmlNode node)
			: base(node)
		{
			foreach (XmlNode objectNode in node.SelectNodes("object"))
			{
				Objects.Add(new MapObjectContent(objectNode));
			}
		}
	}
}
