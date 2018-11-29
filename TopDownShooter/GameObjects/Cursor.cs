namespace TopDownShooter.GameObjects
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    public class Cursor : Component
    {
        private Texture2D cursorTexture; // the texture of the pointer
        private Vector2 scale; // 

        public Cursor(ContentManager Content)
        {
            cursorTexture = Content.Load<Texture2D>("crosshair");
            scale = new Vector2(0.05f, 0.05f); 
        }
        public override void Update(GameTime gameTime)
        {
            // Follow mouse movements on screen and set the position accordingly
            MouseState mouse = Mouse.GetState();
            Position.X = mouse.X - cursorTexture.Width / 2 * scale.X ;
            Position.Y = mouse.Y - cursorTexture.Height/ 2 * scale.Y; 
        }

        public override void Draw(GameTime gameTime, SpriteBatch spritebatch)
        {
            spritebatch.Draw(cursorTexture, Position, scale: scale);
        }
    }
}
