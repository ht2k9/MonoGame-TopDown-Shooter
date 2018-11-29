using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TopDownShooter;
using Database;

namespace TopDownShooter.Logic
{
    class Room : Component
    {
        public Vector2 Enterance;
        public GameObject Exit;

        private TopDownShooter game;
        private GraphicsDevice graphic;
        private Data data;

        private ContentManager Content;
        private Random rng;

        private List<Component> components;
        public List<GameObject> collideables;

        public List<Vector2> spawnPlaces;

        public int currentLevel = 0;

        public bool DoorOpened { get; set; }

        public Room(ContentManager Content, TopDownShooter game, GraphicsDevice graphic)
        {
            data = new Data();
            rng = new Random();

            this.graphic = graphic;
            this.game = game;
            this.Content = Content;

            components = new List<Component>();
            collideables = new List<GameObject>();  
       
            LoadMap();
        }

        private void DrawMap1(ContentManager Content)
        {
            string path = "Wood";
            Texture2D t = Content.Load<Texture2D>("MapObject/" + path + "/floor1");

            t = Content.Load<Texture2D>("MapObject/" + path + "/door1");
            Exit = new GameObject(new Vector2(5 * t.Width / 2, 720 - t.Height / 2), t, ObjectType.DOOR, t.Width / 2, t.Height / 2);

            spawnPlaces = new List<Vector2>()
            {
                new Vector2(50, 150),
                new Vector2(50, 600),
                new Vector2(1150, 400)
            };

            t = Content.Load<Texture2D>("MapObject/plant1");

            for (int i = 0; i < spawnPlaces.Count; i++)
            {
                var c = new GameObject(spawnPlaces[i], t, 0f, t.Width / 4, t.Height / 4);
                c.Type = ObjectType.SPAWN_POINT;
                c.Health = 20;
                collideables.Add(c);
            }
        }

        private void DrawMap2(ContentManager Content)
        {
            Texture2D t;
           
            spawnPlaces = new List<Vector2>()
            {
                new Vector2(50, 150),
                new Vector2(50, 600),
                new Vector2(1150, 600)
            };

            t = Content.Load<Texture2D>("MapObject/plant1");

            for (int i = 0; i < spawnPlaces.Count; i++)
            {
                if(i==2)
                    t = Content.Load<Texture2D>("MapObject/plant2");

                var c = new GameObject(spawnPlaces[i], t, 0f, t.Width / 4, t.Height / 4);
                c.Type = ObjectType.SPAWN_POINT;
                c.Health = 20;
                collideables.Add(c);
            }
        }
 
        private void DrawMap3(ContentManager Content)
        {
            string path = "Metal";
            Texture2D t = Content.Load<Texture2D>("MapObject/" + path + "/floor1");

            t = Content.Load<Texture2D>("MapObject/plant4");
            var c = new GameObject(new Vector2(1150, 600), t, 0f, t.Width / 4, t.Height / 4);
            c.Type = ObjectType.SPAWN_POINT;
            c.Health = 20;
            collideables.Add(c);

            spawnPlaces = new List<Vector2>()
            {
                new Vector2(1150, 600)
            };
        }

        public void LoadMap()
        {
            currentLevel++;
            DoorOpened = false;

            collideables.Clear();
            components.Clear();

            switch (currentLevel)
            {
                case 1:
                    DrawMap1(Content);
                    break;
                case 2:
                    DrawMap2(Content);
                    break;
                case 3:
                    DrawMap3(Content);
                    break;
                default:
                    game.ChangeState(new LevelMenu(game, graphic, Content));
                    break;
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Exit.Draw(gameTime, spriteBatch);

            foreach (var component in components)
            {
                component.Draw(gameTime, spriteBatch);
            }

            foreach (var component in collideables)
            {
                component.Draw(gameTime, spriteBatch);
            }
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var component in components)
            {
                component.Update(gameTime);
            }

            foreach (var component in collideables)
            {
                component.Update(gameTime);
            }
        }

        public Vector2 GetSpawnPosition()
        {
            if (spawnPlaces.Count != 0)
                return spawnPlaces[rng.Next(0, spawnPlaces.Count)];
            else
                return Vector2.Zero;
        }
    }
}


class GameObject : Component
{
    private float Rotation;
    public int Health;

    public GameObject(Vector2 position, Texture2D texture, ObjectType type)
    {

        Position = position;
        Texture = texture;
        Type = type;

        Rectangle = new Rectangle(position.ToPoint(),
            new Point(Texture.Width, Texture.Height));

    }

    public GameObject(Vector2 position, Texture2D texture, float rotation, int Width, int Height)
    {
        Health = 3;

        Position = position;
        Texture = texture;
        Rotation = rotation;
        Type = ObjectType.Breakable;

        Rectangle = new Rectangle((int)Position.X, (int)Position.Y,
            Width, Height);

    }

    public GameObject(Vector2 position, Texture2D texture, ObjectType type, int Width, int Height)
    {
        Health = 3;

        Position = position;
        Texture = texture;
        Type = type;

        Rectangle = new Rectangle((int)Position.X, (int)Position.Y,
            Width, Height);
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Texture, destinationRectangle: Rectangle, rotation: Rotation);
    }

    public override void Update(GameTime gameTime) { }

    public bool TakeDamage()
    {
        Health--;
        if (Health < 0)
            return true;

        return false;
    }
}