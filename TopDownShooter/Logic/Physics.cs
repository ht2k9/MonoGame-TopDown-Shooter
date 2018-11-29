using Microsoft.Xna.Framework;

namespace TopDownShooter.Utils
{
    public class Physics
    {     
        // the roation of the entity
        public double rotation;

        // default speed
        public float speed;
        // current speed to add to position
        public float velocity;
        // x,y scale
        public Vector2 scale;
        // position on screen
        public Vector2 position;

        public Physics(float velocity, Vector2 scale)
        {
            speed = this.velocity = velocity;
            this.scale = scale;

            rotation = 0;
        }
    }
}
