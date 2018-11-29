using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using TopDownShooter.GameObjects;
using Database;
using TopDownShooter.Logic;
using System;

namespace TopDownShooter
{
    class GameState : State
    {
        private const int GAME_WIDTH = 1300;
        private const int GAME_HEIGHT = 700;

        // Game objects we need:
        private Player player;
        private GameManager gameManager;
        private SpriteFont font;

        Button resetBtn;

        public GameState(TopDownShooter game, GraphicsDevice graphicDevice, ContentManager content, Fighter fighter) : base(game, graphicDevice, content)
        {
            game.IsMouseVisible = false;

            player = new Player(content, GAME_WIDTH, GAME_HEIGHT, fighter, 8f, new Vector2(0.45f, 0.45f));

            gameManager = new GameManager(content, player, GAME_WIDTH, GAME_HEIGHT, new Room(content, game, graphicDevice));

            player.shootSignal += gameManager.ShootZombies;
            player.punchSignal += gameManager.PunchZombies;

            font = content.Load<SpriteFont>("Font");

            resetBtn = new Button(content.Load<Texture2D>("button"), font, 100, 200)
            {
                Position = new Vector2(GAME_WIDTH / 2 - 100, GAME_HEIGHT / 2),
                Text = "Reset"
            };
            resetBtn.Click += (sender, e) => Reset_Game(sender, e);
        }

        private void Reset_Game(object sender, EventArgs e)
        {
            game.ChangeState(new LevelMenu(game, graphicDevice, content)); // enter the level menu
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (player.IsAlive)
            {                
                gameManager.Draw(gameTime, font, spriteBatch);

                player.Draw(spriteBatch);
                player.Input.Cursor.Draw(gameTime, spriteBatch);

                spriteBatch.DrawString(font,"Destroy Plant and Kill Zombies", new Vector2(10, GAME_HEIGHT /6), Color.Red);
            }
            else
            {
                graphicDevice.Clear(Color.Black);
                spriteBatch.DrawString(font, "GAME OVER", new Vector2(GAME_WIDTH / 2 , GAME_HEIGHT / 2), Color.IndianRed);
                resetBtn.Draw(gameTime, spriteBatch);
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (player.IsAlive)
            {
                player.Update(gameTime);
                gameManager.Update(gameTime);
            }
            else
            {
                resetBtn.Update(gameTime);
                game.IsMouseVisible = true;
            }
        }
    }
}
