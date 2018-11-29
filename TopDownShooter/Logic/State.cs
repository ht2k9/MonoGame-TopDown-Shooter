using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
namespace TopDownShooter
{
    public abstract class State : Component
    {
        protected ContentManager content;

        protected GraphicsDevice graphicDevice;

        protected TopDownShooter game;

        public State(TopDownShooter game, GraphicsDevice graphicDevice, ContentManager content)
        {
            this.game = game;

            this.content = content;

            this.graphicDevice = graphicDevice;
        }
    }
}
