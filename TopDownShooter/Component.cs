using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TopDownShooter
{
    // Use this class to laod default functions that most class will use.
    // The classes that inherit from this abstract will be drawen on the sreen.
    public abstract class Component
    {
        public enum ObjectType
        {
            GRAPHIC, Breakable, WALL, FLOOR, SPAWN_POINT, DOOR
        }

        public Vector2 Position;
        public Texture2D Texture;
        
        public Rectangle Rectangle;

        public ObjectType Type;

        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);

        public abstract void Update(GameTime gameTime);
        
    }
}