using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content;

namespace RsxContentLibrary
{
    class LocalizedFontDescription: FontDescription
    {
        public LocalizedFontDescription(string fontName, float size, float spacing) 
            : base(fontName, size, spacing) { }
   
        [ContentSerializer(Optional = true, CollectionItemName = "Resx")]
        public List<string> ResourceFiles
        {
            get { return resourceFiles; }
        }

        List<string> resourceFiles = new List<string>();
    }
}
