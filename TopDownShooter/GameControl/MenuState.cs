using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TopDownShooter.Utils;
using Microsoft.Xna.Framework.Media;

namespace TopDownShooter
{
    class MenuState : State
    {
        private List<Component> components;

        public MenuState(TopDownShooter game, GraphicsDevice graphicDevice, ContentManager content) : base(game, graphicDevice, content)
        {
            // create new buttons and load icons from resource then add to list to display on screen
            game.IsMouseVisible = true;

            var buttonTexture = content.Load<Texture2D>("button");
            var buttonFont = content.Load<SpriteFont>("Font");

            var pos = new Vector2(550, graphicDevice.Viewport.Height / 2);

            var newGameButton = new Button(buttonTexture, buttonFont)
            {
                Position = pos,
                Text = "New Game"
            };

            newGameButton.Click += NewGameButton_Click;

            var loadGameButton = new Button(buttonTexture, buttonFont)
            {
                Position = pos + new Vector2(0, 70),
                Text = "Shop"
            };

            loadGameButton.Click += LoadGameButton_Click;

            var quitGameButton = new Button(buttonTexture, buttonFont)
            {
                Position = pos + new Vector2(0, 140),
                Text = "Quit"
            };

            //create list of component of type buttons
            quitGameButton.Click += QuitGameButton_Click;

            SoundManager.LoopSound(content.Load<Song>("Music/background_music"));

            components = new List<Component>()
               {
                   newGameButton,
                   loadGameButton,
                   quitGameButton

               };

        }

        private void QuitGameButton_Click(object sender, EventArgs e)
        {
            game.Exit();
        }

        private void LoadGameButton_Click(object sender, EventArgs e)
        {
            game.ChangeState(new ShopState(game, graphicDevice, content)); // enter the shop menu
        }

        private void NewGameButton_Click(object sender, EventArgs e)
        {
            game.ChangeState(new LevelMenu(game, graphicDevice, content)); // enter the level menu
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var comp in components)
                comp.Draw(gameTime, spriteBatch); // draw each button
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var comp in components)
                comp.Update(gameTime); // update each button
        }
    }
}
