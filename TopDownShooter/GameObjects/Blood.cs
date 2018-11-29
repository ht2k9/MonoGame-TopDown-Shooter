using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TopDownShooter.Utilities;

namespace TopDownShooter.GameObjects
{
    public class Blood
    {
        private const int BLOOD_SHOWN_TIMER = 100; // show for 1 second
        private Animation animation; // one animation of the blood
        public bool Active { get;private set; } // active or not
        private double timer; // default timer
        private Vector2 position; // x,y position on scrren

        public Blood(ContentManager Content, Vector2 position)
        {
            this.position = position;

            // load the blood sprite has 16 frames
            var animationTexture = Content.Load<Texture2D>("bloodAnimation");
            animation = new Animation(animationTexture, 16, 4, 4, 128, 128);
            // set active
            Active = true;
            timer = 0;
        }

        public void Update(GameTime gameTime)
        {
            // check if 1 second passed or not
            timer += gameTime.ElapsedGameTime.Milliseconds;
            if (timer >= BLOOD_SHOWN_TIMER)
            {
                Active = false;
            }

            animation.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw the animtation
            animation.Draw(spriteBatch, 0, Vector2.Zero, position, new Vector2(0.8f, 0.8f), Color.White);
        }

    }
}
