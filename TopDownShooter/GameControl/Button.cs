using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopDownShooter
{
    class Button : Component
    {
        private double defTimer; // default timer
        public double timer; // passed time

        private SpriteFont font; // font of the button
        private bool isHovering; // set mouse hovering or not

        private MouseState currMouse; // current mouse state/click
        private MouseState prevMouse; // previous mouse state/click

        private int Width; // width of the button
        private int Height; // height of the button

        public event EventHandler Click;

        public bool Clicked { get; private set; } // button clicked or not

        public bool CanUse { get; set; } // can we even click the button?

        public Color PenColor; // Font color

        public string Text { get; set; } // what to write
        public Rectangle Rectangle { get => new Rectangle( (int)Position.X, (int)Position.Y, Width, Height);}

        // no specific height or width
        public Button(Texture2D Texture, SpriteFont font)       
        {
            this.Texture = Texture;
            this.font = font;

            PenColor = Color.Black;

            Height = Texture.Height;
            Width = Texture.Width;
        }

        // specific height or width
        public Button(Texture2D Texture, SpriteFont font, int height, int width)
        {
            this.Texture = Texture;
            this.font = font;

            PenColor = Color.Black;

            Height = height;
            Width = width;
        }

        // set time untill we can use the button (used for abilites)
        public Button(Texture2D Texture, SpriteFont font, int height, int width, double time)
        {
            this.Texture = Texture;
            this.font = font;

            PenColor = Color.Black;

            defTimer = time;

            Height = height;
            Width = width;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        { 
            // Default color to white
            var color = Color.White;

            // if mouse hovering set color to gray
            if (isHovering)
                color = Color.Gray;

            // draw the button
            spriteBatch.Draw(Texture, Rectangle, color);

            //draw the text if it's not null or empty
            if (!string.IsNullOrEmpty(Text))
            {
                var x = (Rectangle.X + (Rectangle.Width / 2) - font.MeasureString(Text).X / 2);
                var y = (Rectangle.Y + (Rectangle.Height / 2) - font.MeasureString(Text).Y / 2);

                spriteBatch.DrawString(font, Text, new Vector2(x, y), PenColor);
            }
        }

        public override void Update(GameTime gameTime)
        {
            // save current mouse clicks
            prevMouse = currMouse;
            // check if there is a click and save it
            currMouse = Mouse.GetState();

            // create mouse rectangle of 1 pixel on screen
            var mouseRectangle = new Rectangle(currMouse.X, currMouse.Y, 1, 1);
            
            isHovering = false;

            // is the mouse rectangle interscting with the button rectangle
            if (mouseRectangle.Intersects(Rectangle))
            {
                isHovering = true;

                // 
                if (currMouse.LeftButton == ButtonState.Released && prevMouse.LeftButton == ButtonState.Pressed)
                    Click?.Invoke(this, new EventArgs());
            }

            // we can't mouse press the skill buttons, but we can activate them on keyboard 
            if (CanUse)
                return;

            // shadow hover (make color gray) button if some time is passed and reset it when done
            timer += gameTime.ElapsedGameTime.TotalSeconds;

            if (timer > defTimer)
            {
                CanUse = true;
                timer = 0;
                isHovering = false;
            }
            else
            {
                isHovering = true;
            }
        }
    }
}
