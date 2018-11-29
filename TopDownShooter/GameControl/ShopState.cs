using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Database;

namespace TopDownShooter
{
    class ShopState : State
    {
        private int points;
        private List<Button> shop;
        private Dictionary<string, int> items;
        private SpriteFont font;

        public ShopState(TopDownShooter game, GraphicsDevice graphicDevice, ContentManager content) : base(game, graphicDevice, content)
        {
            points = Data.GetScore();

            font = content.Load<SpriteFont>("Font");

            items = new Dictionary<string, int>();
            items.Add("Gun", 50);
            items.Add("Rifle", 150);
            items.Add("Shotgun", 200);

            shop = new List<Button>();
            var buttonTexture = content.Load<Texture2D>("button");
            Button btn;

            int i = 0;
            foreach (var item in items.Keys)
            {
                btn = new Button(buttonTexture, font, 100, 200)
                {
                    Position = new Vector2(50 + (i) * 220, 450),
                    Text = items[item].ToString()
                };
                

                btn.Click += (sender, e) => Buy_Item(sender, e, item);
                shop.Add(btn);
                i++;
            }

            btn = new Button(buttonTexture, font, 100, 200)
            {
                Position = new Vector2(100, 100),
                Text = "Back"
            };

            btn.Click += (sender, e) => Return(sender, e);
            shop.Add(btn);
        }

        private void Return(object sender, EventArgs e)
        {
            game.ChangeState(new MenuState(game, graphicDevice, content)); // enter the level menu
        }

        private void Buy_Item(object sender, EventArgs e, string item)
        {
            List<Fighter> fighters = Data.ReadObject("fighters.json");

            // check if already bought item and can afford it
            foreach (var f in fighters)
            {
                Console.WriteLine(f.canUse);
                if (items[item] <= points && !f.canUse)
                {
                    f.CanUse(item);
                    points -= items[item];
                    Data.SetScore(points);
                }

            }

            Data.WriteToObject("fighters.json", fighters);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            int i = 0;
            spriteBatch.DrawString(font, "points: "+points, new Vector2(20, 20), Color.Gold);

            foreach (var item in shop)
                item.Draw(gameTime, spriteBatch);

            // draw items in shop
            foreach (var item in items.Keys)
            {
                var buttonTexture = content.Load<Texture2D>("Icons/" + item);
                spriteBatch.Draw(buttonTexture, new Rectangle(50 + (i) * 220, 250, 200, 100), Color.White);

                i++;
            }
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var item in shop)
                item.Update(gameTime);
        }
    }
}
