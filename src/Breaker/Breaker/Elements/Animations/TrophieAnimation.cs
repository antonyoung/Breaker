using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using Breaker.Wp7.Xna4.Data;
using Breaker.Wp7.Xna4.Managers;

namespace Breaker.Wp7.Xna4.Elements.Animations
{
    class TrophieAnimation
    {
        public List<TrophieData> AchievedTrophies
        {
            get;
            set;
        }

        public TrophieAnimation(ContentManager content)
        {
            AchievedTrophies = new List<TrophieData>();
            LoadContent(content);
        }

        #region private fields

        SpriteFont _spriteFont;
        Texture2D _textureBlank;
        TimeSpan _animTrophie = TimeSpan.FromSeconds(5);
        Dictionary<TrophieData.TrophieValue, Texture2D> _texturesTrophies;

        #endregion
        #region content

        void LoadContent(ContentManager content)
        {


            _texturesTrophies = new Dictionary<TrophieData.TrophieValue, Texture2D>();

            // load texture (1px transparant) and font
            _textureBlank = content.Load<Texture2D>("Textures/blank");

            _texturesTrophies.Add(TrophieData.TrophieValue.Platinum, content.Load<Texture2D>("Textures/Trophies/platinum"));
            _texturesTrophies.Add(TrophieData.TrophieValue.Gold, content.Load<Texture2D>("Textures/Trophies/gold"));
            _texturesTrophies.Add(TrophieData.TrophieValue.Silver, content.Load<Texture2D>("Textures/Trophies/silver"));
            _texturesTrophies.Add(TrophieData.TrophieValue.Bronze, content.Load<Texture2D>("Textures/Trophies/bronze"));
            //_texturesTrophies.Add(TrophieData.TrophieValue.None, content.Load<Texture2D>("Textures/Trophies/locked"));

            _spriteFont = content.Load<SpriteFont>("Fonts/gamefont");
            //LoadTrophies();
        }

        #endregion

        #region draw and update

        public void Draw(SpriteBatch spriteBatch)
        {

            if (AchievedTrophies.Count == 0) return;

            string textTrophie = string.Format(
                ResourceManager.Text(AchievedTrophies[0].TitleResourceKeyName));

            Vector2 vSize = _spriteFont.MeasureString(textTrophie);

            var textures =
                from t in _texturesTrophies
                where t.Key == AchievedTrophies[0].Value
                select t;

            Texture2D textureTrophie = null;

            foreach (KeyValuePair<TrophieData.TrophieValue, Texture2D> t in _texturesTrophies)
            {
                if (t.Key == AchievedTrophies[0].Value)
                {
                    textureTrophie = t.Value;
                    break;
                }
            }

            float scale = .25f;

            Vector2 position = new Vector2(800 - vSize.X * scale * 2 - 32f, 10f);
            Vector2 texturePosition = new Vector2(position.X - 48, 10f);

            Rectangle rectangle
                = new Rectangle((int)texturePosition.X - 15, 0, 825 - (int)texturePosition.X, 50);

            // draw background
            spriteBatch.Draw(_textureBlank, rectangle, new Color(0, 0, 0, .50f));

            // draw trophie title
            spriteBatch.DrawString(
                _spriteFont, textTrophie, position, Color.Silver, 0f, Vector2.Zero, scale * 2, SpriteEffects.None, 0);

            // draw trophie value
            spriteBatch.Draw(
                textureTrophie, texturePosition, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0);
        }


        public void Update(GameTime gameTime)
        {
            if (AchievedTrophies.Count == 0) return;

            _animTrophie -= gameTime.ElapsedGameTime;
            if (_animTrophie.Milliseconds < 0)
            {
                _animTrophie = TimeSpan.FromSeconds(5);
                AchievedTrophies.Remove(AchievedTrophies[0]);
            }
        }

        #endregion

    }
}
