using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Breaker.Wp7.Xna4.Data;
using Breaker.Wp7.Xna4.Managers;

namespace Breaker.Wp7.Xna4.Screens.Elements
{
    
    public class Trophie
    {
        #region fields

        
        Vector2 _origin;
        Texture2D _textureTrophie;
        SpriteFont _spriteFont;
        float scale = .45f;
        const int PADDING = 30;

        string _trophieTitle = string.Empty;
        string _trophieDescription = string.Empty;

        #endregion

        #region properties

        public Vector2 Position;
        public int Value
        {
            get;
            private set;
        }
        
      

        #endregion

        #region public methods

        public Trophie(SpriteFont spriteFont, Texture2D texture, TrophieData trophieData)
        {
            //if (trophieData.IsHidden && trophieData.DateTimeAchievement == null)
            if (trophieData.IsHidden && !trophieData.IsAchieved)

            {
                _trophieTitle = ResourceManager.Text(ResourceManager.ResourceKeyName.TrophieTitleHidden);
                _trophieDescription = ResourceManager.Text(ResourceManager.ResourceKeyName.TrophieDescriptionHidden);
            }
            else
            {
                _trophieTitle = ResourceManager.Text(trophieData.TitleResourceKeyName);
                _trophieDescription = ResourceManager.Text(trophieData.DescriptionResourceKeyName);
            }

            _spriteFont = spriteFont;
            _textureTrophie = texture;
            _origin = new Vector2(_textureTrophie.Width * scale / 2, _textureTrophie.Height * scale / 2);
        }

        
        public void Draw(SpriteBatch spriteBatch)
        {
            // draw trophie title
            spriteBatch.DrawString(_spriteFont, _trophieTitle, Position, Color.Yellow, 0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0);

            // draw texture
            Vector2 texturePosition = new Vector2(Position.X - _origin.X * 2 - PADDING, Position.Y + 5);
            spriteBatch.Draw(_textureTrophie, texturePosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0);
             
            // draw trophie description
            Color color = new Color(192, 192, 192);
 
            Vector2 descriptionPosition = new Vector2(Position.X + 15, Position.Y + 30);
            spriteBatch.DrawString(_spriteFont, _trophieDescription, descriptionPosition, color, 0F, Vector2.Zero, .8f, SpriteEffects.None, 0);

        }

        #endregion
    }
}
