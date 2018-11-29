namespace TopDownShooter.GameObjects
{
    using global::TopDownShooter.Utilities;
    using global::TopDownShooter.Utils;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using System;
    using System.Collections.Generic;

    public abstract class Entity
    {
        // the physic of the game object
        protected Physics _physics;
        // use a name for each animation to access it easily in the dictionary
        protected Dictionary<string, Animation> _animations;
        // current animation being animated
        protected string currentAnimationKey;
        // overall damage it does to other entities
        public double Damage { get; protected set; }
        // full health, once zero, the entity dies 
        public double Health { get; protected set; }
        // to check if died or not
        public bool IsAlive { get; protected set; }
        // game width and height
        public int gameWidth;
        public int gameHeight;

        public Color color;
        private ContentManager content;

        // returns the current animation
        public Animation GetAnimation() { return _animations[currentAnimationKey]; }

        public Vector2 Position
        {
            get =>_physics.position;
            set => _physics.position = value;
        }

        // load the neccesary variables
        public Entity(ContentManager Content, int gameWidth, int gameHeight, double health, double damage, float velocity, Vector2 scale)
        {
            this.gameHeight = gameHeight;
            this.gameWidth = gameWidth;

            Health = health;
            IsAlive = true;
            Damage = damage;

            _physics = new Physics(velocity, scale);

            currentAnimationKey = "";
            _animations = new Dictionary<string, Animation>();

            color = Color.White;
        }

        public Entity(ContentManager content, int gameWidth, int gameHeight)
        {
            this.content = content;
            this.gameWidth = gameWidth;
            this.gameHeight = gameHeight;

            IsAlive = true;

            currentAnimationKey = "";
            _animations = new Dictionary<string, Animation>();

            color = Color.White;
        }

        // draws the entity on the screen
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            _animations[currentAnimationKey].Draw(spriteBatch, _physics.rotation, Vector2.Zero, _physics.position, _physics.scale, color );
        }

#region Needed_Functions
        // in the base update, we have three main functions to use
        // the sub class will inherit the functions and edit them to suit it
        public virtual void Update()
        {
            DoPhysics();
            DoMovement();
            DoAnimation();
        }

        public abstract void DoPhysics();

        public abstract void DoMovement();

        public abstract void DoAnimation();
#endregion

        // any object that takes damage and reaches 0 health dies
        public virtual void TakeDamage(double damage)
        {
            Health -= damage;
            if (Health <= 0)
            {
                IsAlive = false;
            }
        }
        // load the animations to the dictionary
        protected abstract void CreateAnimations(ContentManager Content);

    }
}
