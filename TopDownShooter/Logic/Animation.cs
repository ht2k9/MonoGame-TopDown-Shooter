namespace TopDownShooter.Utilities
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class Animation
    {
        private int DEFAULT_DURATION = 20; // Default duration of the animation 
        private Texture2D texture; // the sprite sheet
        private int framesCount; // over all frames count
        private int currentFrameRow; // x frame we currently have (Row)
        private int currentFrameCol; // y frame we currently have (Column)
        private int currentFrame; // current x,y frame
        private int rows; // over all number of rows
        private int cols; // over all number of columns
        private double frameDuration; // 
        private double timeSinceLastChange;

        // width and height per frame
        private int width;
        private int height;

        // finished looping or not
        public bool Looping = true;

        public int Width { get => width; }
        public int Height { get => height; }

        // create rectangle from the frame
        public Rectangle FrameBounds { get; private set; }

        // to change the duration if we need to
        public Animation(Texture2D spriteSheet, int frames, int rows, int cols, int width, int height, int duration)
        {
            texture = spriteSheet;
            framesCount = frames;

            // all frame variables start from first frame
            currentFrameRow = 0;
            currentFrameCol = 0;
            currentFrame = 0;
            frameDuration = DEFAULT_DURATION = duration;
            timeSinceLastChange = 0;

            this.rows = rows;
            this.cols = cols;
            this.width = width;
            this.height = height;
        }

        // default duration
        public Animation(Texture2D spriteSheet, int frames, int rows, int cols, int width, int height)
        {
            texture = spriteSheet;
            framesCount = frames;

            currentFrameRow = 0;
            currentFrameCol = 0;
            currentFrame = 0;
            frameDuration = DEFAULT_DURATION;
            timeSinceLastChange = 0;

            this.rows = rows;
            this.cols = cols;
            this.width = width;
            this.height = height;
        }

        public void Update(GameTime gameTime)
        {
            // add milisecond to change the frame
            timeSinceLastChange += gameTime.ElapsedGameTime.Milliseconds;
            // check if frame reached the duration
            if (timeSinceLastChange >= frameDuration)
            {
                timeSinceLastChange = 0; // if yes return to zero

                // next column
                currentFrameCol = (currentFrameCol + 1) % cols;
                if (currentFrameCol == 0)
                {
                    // count how mnay we changed columns
                    // set next row
                    currentFrameRow = (currentFrameRow + 1) % rows;
                }
                currentFrame = (currentFrame + 1) % framesCount;
                
                if (currentFrame == 0)
                {
                    // when looping and then we returned to 0, restore the current row and column
                    currentFrameRow = 0;
                    currentFrameCol = 0;
                }

                // if we reached the end, set looping done
                if (currentFrame == framesCount)
                {
                    Looping = false;
                }

                // set the rectangle bounds for each frame
                FrameBounds = new Rectangle(width * currentFrameCol,
                    height * currentFrameRow, width, height);
            }
        }

        public void Draw(SpriteBatch spriteBatch, double rotation, Vector2 origin, Vector2 position, Vector2 scale, Color color)
        {
            // if we got zero origin, set it to the center of the frame
            if(origin == Vector2.Zero)
                origin = new Vector2(width / 2, height / 2);

            // draw the current frame
            spriteBatch.Draw(texture, position, sourceRectangle: FrameBounds, origin: origin, scale: scale, rotation: (float)rotation, color: color);
        }
    }
}
