using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TopDownShooter
{
    public class TopDownShooter : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private const int GAME_WIDTH = 1280;
        private const int GAME_HEIGHT = 720;

        private State currentState;
        private State nextState;

        // Set the next state
        public void ChangeState(State state)
        {
            nextState = state;
        }

        public TopDownShooter()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        // default initializion of the variables
        // set the screen width and hieght - 720x1280
        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = GAME_WIDTH;
            graphics.PreferredBackBufferHeight = GAME_HEIGHT;
            graphics.ApplyChanges();

            base.Initialize();
        }

        // Load the default state (the menu state)
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            currentState = new MenuState(this, graphics.GraphicsDevice, Content);       
        }

        protected override void UnloadContent() { }

        // see if there is a next state, we set it during run time after calling ChangeState
        protected override void Update(GameTime gameTime)
        {
            if (nextState != null)
            {
                currentState = nextState;
                nextState = null;
            }

            currentState.Update(gameTime);

            base.Update(gameTime);
        }

        // Draw the current state and the background 
        protected override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            spriteBatch.Draw(Content.Load<Texture2D>("main_background"),Vector2.Zero, Color.White);
            currentState.Draw(gameTime, spriteBatch);

            spriteBatch.End();
            base.Draw(gameTime);
        }

        
    }
}
