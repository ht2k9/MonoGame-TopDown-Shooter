using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TopDownShooter.Utilities;
using System;

namespace TopDownShooter.GameObjects
{
    class Bullet : Component
    {
        Player player; // the parent of the bullet

        public double damage;

        float speed;
        double rotation;

        private Vector2 direction;

        public Bullet(Texture2D Texture, Player player)
        {
            Random r = new Random();
            this.Texture = Texture;
            this.player = player;

            damage = player.Damage;
            Console.WriteLine(damage);

            // spawn from the origin of the player
            Position = player.Position;
            // calculcate which direction the bullet will go
            direction = OwnMath.GetDirectionToPoint(player.Position.ToPoint(), player.Input.Cursor.Position.ToPoint()); 
            // get the rotation to the mouse pointer (equal be the player._physics.rotation)
            rotation = OwnMath.CalculateAngleBetweenPoints(player.Position.ToPoint(), player.Input.Cursor.Position.ToPoint());
            // default speed
            speed = 5f;
        }


        public override void Update(GameTime gameTime)
        {
            // get the units of the same direction of the vector
            direction.Normalize();
            // keep moving in the same direction
            Position.X += direction.X * speed * 5f;
            Position.Y += direction.Y * speed * 5f;

            Rectangle = new Rectangle(Position.ToPoint(), new Point(Texture.Width, Texture.Height));
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Draw the bullet
            spriteBatch.Draw(texture: Texture, position: Position, rotation: (float)rotation);
        }
    }
}
