using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using TopDownShooter.Utils;
using Database;

namespace TopDownShooter
{
    class LevelMenu : State
    {
        private List<Component> components;

        private List<Fighter> fighters;
        private Fighter fighter;

        public LevelMenu(TopDownShooter game, GraphicsDevice graphicDevice, ContentManager content) : base(game, graphicDevice, content)
        {
            fighters = Data.ReadObject("fighters.json");
            fighter = fighters[0];

            string[] maps = new string[] {"wood", "desert", "grass"}; // resources names

            // create new list then add components of type button
            components = new List<Component>(); 

            var buttonFont = content.Load<SpriteFont>("Font");

            for(int i=0; i<maps.Length; i++)
            {
                var text = maps[i];
                if (i != 0)
                    text += "_locked"; // lock the closed levels

                var bt = content.Load<Texture2D>(text);
                
                var button = new Button(bt, buttonFont, 250, 400)
                {
                    Position = new Vector2( 20 + (i) * 420, 250)     // set the button to take on third of the screen width         
                };

                if(i==0)
                    button.Click += (sender, e) => Button_Click(sender, e); // costumize the click event to the button text to load different level

                components.Add(button);
            }

            var buttonTexture = content.Load<Texture2D>("button");

            for (int i = 0; i < fighters.Count; i++)
            {
                if (fighters[i].canUse)
                { 
                    var txt = fighters[i].weapontype;
                    var btn = new Button(buttonTexture, content.Load<SpriteFont>("Font"), 50, 100)
                    {
                        Position = new Vector2(50 + (i) * 220, 600),
                        Text = txt
                    };

                    btn.Click += (sender, e) => Select_Weapon(sender, e, txt);

                    components.Add(btn);
                }
            }

            Button backBtn = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(100, 100),
                Text = "Back"
            };

            backBtn.Click += (sender, e) => Return(sender, e);
            components.Add(backBtn);
        }

        private void Return(object sender, EventArgs e)
        {
            game.ChangeState(new MenuState(game, graphicDevice, content)); // enter the level menu
        }

        private void Select_Weapon(object sender, EventArgs e, string item)
        {
            foreach (Fighter f in fighters)
            {
                if (f.weapontype == item)
                {
                    fighter = f;
                }
            }
        }

        private void Button_Click(object sender, EventArgs e)
        {
            SoundManager.StopSound();
            game.ChangeState(new GameState(game, graphicDevice, content, fighter)); // load the game menu screen
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var comp in components)
                comp.Draw(gameTime, spriteBatch); // draw components on the screen
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var comp in components)
                comp.Update(gameTime); // update the components
        }
    }
}
