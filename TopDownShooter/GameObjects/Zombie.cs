namespace TopDownShooter.GameObjects
{
    using global::TopDownShooter.Utilities;
    using global::TopDownShooter.Utils;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    public delegate void AttackSignal();

    public class Zombie : Entity
    {
        // name of the animations
        private const string MOVE_ANIMATION_KEY = "move"; 
        private const string ATTACK_ANIMATION_KEY = "attack";

        private const int ATTACK_DISTANCE = 50; // distance to the fighter to invoke attack signal

        private const double ATTACK_ANIMATION_TIMER = 200; // fire rate of the zombie melee attack

        private Vector2 targetPos; // fighter position
        private GameTime gameTime; // passed game time
        private double attackTimer; // time passed when invoking the attack 

        public event AttackSignal zombieAttack; // function to invoke when attacking

        public Zombie(ContentManager Content, int gameWidth, int gameHeight, string type)
            :base(Content, gameWidth, gameHeight)
        {
            if (type == "default")
            {
                _physics = new Physics(3f, new Vector2(0.45f, 0.45f));
                Health = 10; Damage = 4; color = Color.White;
            }

            if (type == "minion")
            {
                _physics = new Physics(6f, new Vector2(0.35f, 0.35f));
                Health = 7; Damage = 2; color = Color.Red;
            }

            if (type == "leader")
            {
                _physics = new Physics(1.5f, new Vector2(0.6f, 0.6f));
                Health = 15; Damage = 12; color = Color.Blue;
            }

            currentAnimationKey = MOVE_ANIMATION_KEY;

            CreateAnimations(Content);
        }

        public void Update(GameTime gameTime, Vector2 targetPos)
        {
            this.gameTime = gameTime;
            this.targetPos = targetPos;

            base.Update();
        }

        public override void TakeDamage(double damage)
        {
            // TODO: Knockback

            base.TakeDamage(damage);           
        }

        // load different animation sprite sheet and create a new animation to add to the dictionary
        protected override void CreateAnimations(ContentManager Content)
        {
            var moveAnimation = Content.Load<Texture2D>("Sprites/zombie1/" + MOVE_ANIMATION_KEY);
            var attackAnimation = Content.Load<Texture2D>("Sprites/zombie1/" + ATTACK_ANIMATION_KEY);

            _animations.Add(MOVE_ANIMATION_KEY, new Animation(moveAnimation, 17,6,3,288,314));
            _animations.Add(ATTACK_ANIMATION_KEY, new Animation(attackAnimation, 9, 3, 3, 294, 318));
        }

        public override void DoPhysics()
        {
            // get direction to the current target
            Vector2 direction = OwnMath.GetDirectionToPoint(Position.ToPoint(), targetPos.ToPoint());
            
            // get the angle between the zombie and the player
            float angle = OwnMath.CalculateAngleBetweenPoints(Position.ToPoint(), targetPos.ToPoint());
            float distance = direction.Length(); // distance equals the length of the vector
            direction.Normalize(); // normalize the direction to units

            // move the zombie and set the animation if the zombie didnt reach the player
            // basically follow the player
            if (distance > ATTACK_DISTANCE)
            {
                currentAnimationKey = MOVE_ANIMATION_KEY;
                _physics.position.X += direction.X * _physics.speed;
                _physics.position.Y += direction.Y * _physics.speed;
            }
            else // if reached invoke the attack according to the fire rate
            {
                currentAnimationKey = ATTACK_ANIMATION_KEY; // set the animation to attack
                attackTimer += gameTime.ElapsedGameTime.Milliseconds; // increase time
                if (attackTimer >= ATTACK_ANIMATION_TIMER)
                {
                    // Attack
                    zombieAttack.Invoke();
                    attackTimer = 0; // reset timer after reaching max
                }
            }

            _physics.rotation = angle;
        }

        public override void DoMovement() {}

        public override void DoAnimation()
        {
            // update the animation
            foreach (var pair in _animations)
            {
                _animations[pair.Key].Update(gameTime);
            }
        }
    }
}
