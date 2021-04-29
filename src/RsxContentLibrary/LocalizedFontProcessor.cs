using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;


namespace RsxContentLibrary
{
    [ContentProcessor]
    class LocalizedFontProcessor 
        : ContentProcessor<LocalizedFontDescription, SpriteFontContent>
    {
        public override SpriteFontContent Process(
            LocalizedFontDescription input, ContentProcessorContext context)
        {
            foreach (string resourceFile in input.ResourceFiles)
            {
                string absolutePath = Path.GetFullPath(resourceFile);

                // Make sure the .resx file really does exist.
                if (!File.Exists(absolutePath))
                {
                    throw new InvalidContentException("Can't find " + absolutePath);
                }

                // Load the .resx data.
                XmlDocument xmlDocument = new XmlDocument();

                xmlDocument.Load(absolutePath);
                // Scan each string from the .resx file.
                foreach (XmlNode xmlNode in xmlDocument.SelectNodes("root/data/value"))
                {
                    string resourceString = xmlNode.InnerText;

                    // Scan each character of the string.
                    foreach (char usedCharacter in resourceString)
                    {
                        input.Characters.Add(usedCharacter);
                    }
                }
                context.AddDependency(absolutePath);
            }
            return context.Convert<FontDescription,
                          SpriteFontContent>(input, "FontDescriptionProcessor");
        }
    }
}

