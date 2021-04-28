using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Breaker.Wp7.Xna4
{
    public class World
    {
        public TimeSpan dayLength;
        public TimeSpan timeOfDay;
        public float ambientLightLevel;
        public float spotlightLevel;

        /// <summary>
        /// Create a new instance of the World
        /// </summary>
        /// <param name="DayLength">Length of a game day in seconds</param>
        public World(double DayLength)
        {
            dayLength = TimeSpan.FromSeconds(DayLength);
            timeOfDay = TimeSpan.FromSeconds(0);
            ambientLightLevel = 1.0f;
            spotlightLevel = 0f;
        }

        public void Update(GameTime gameTime)
        {
            // Update time of day
            timeOfDay += gameTime.ElapsedGameTime;
            if (timeOfDay > dayLength) timeOfDay = TimeSpan.FromSeconds(0);

            float halfDay = (float)(dayLength.TotalMilliseconds / 2);
            if (timeOfDay.TotalMilliseconds < halfDay)
            {
                ambientLightLevel -= (0.9f / halfDay) * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                spotlightLevel += (1.0f / halfDay) * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            }
            else
            {
                ambientLightLevel += (0.9f / halfDay) * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                spotlightLevel -= (1.0f / halfDay) * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            }
        }
    }
}
