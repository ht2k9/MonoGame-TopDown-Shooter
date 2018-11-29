

namespace TopDownShooter.GameObjects
{
    using Utilities;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using System;
    using Microsoft.Xna.Framework.Content;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework.Audio;
    using global::TopDownShooter.Utils;
    using Database;

    // event to fire bullets
    public delegate void ShootSignal();
    // event to melee attack
    public delegate void PunchSignal();

    public class Player : Entity
    { 
        private const int FULL_CHARGE = 200; // Charge meter to use fighter ability
        private const int BORDER_OFFSET = 50; // how many meters to stay away from border

        private double FIRE_RATE; // fire rate of the current weapon
        private double timer; // count seconds of the game

        private string WEAPON; // current weapon
        private int MAGAZINE_SIZE; // how many bullets before reload

        // sound instance to play in different actions and input
        private Dictionary<String, SoundEffectInstance> sounds;

        // names of the animations
        private const string IDLE_ANIMATION_KEY = "idle";
        private const string SHOOT_ANIMATION_KEY = "shoot";
        private const string MOVE_ANIMATION_KEY = "move";
        private const string WARP_ANIMATION_KEY = "warp";
        private const string RELOAD_ANIMATION_KEY = "reload";
        private const string MELEE_ANIMATION_KEY = "melee";

        private const float DEFAULT_ACCELERATION = 6f; // default accelarion

        public int abilityCharge; // ability meter current charge
        public int bulletsCounter; // current bullets in the magazine

        private double shootDelayTimer; // timer before shooting another bullet

        private bool abilityActivated = false; // see ability is activated or not

        public Input Input; // keyboard and mosue input
        public event ShootSignal shootSignal; // shoot function fired when pressing the right input
        public event PunchSignal punchSignal; // melee attack fired when pressing the right input

        private GameTime gameTime; // use game time in differnet class functions
        private List<Button> skills; // skills of the fighter

        public Player(ContentManager Content, int gameWidth, int gameHeight, Fighter fighter, float velocity, Vector2 scale)
            : base(Content, gameWidth, gameHeight,fighter.health, fighter.damage, velocity, scale)
        {
            Console.WriteLine(fighter);
            // start with full magazine
            bulletsCounter = MAGAZINE_SIZE = fighter.magazineSize;
            // set fire rate
            FIRE_RATE = fighter.firerate;
            // set weapon to load from different resources
            WEAPON = fighter.weapontype;
            // set default speed
            _physics.speed = DEFAULT_ACCELERATION;
            // set position to the cetner of the screen
            _physics.position = new Vector2(gameWidth / 2, gameHeight / 2);

            // create sound instances from resources
            sounds = new Dictionary<string, SoundEffectInstance>
            {
                { "Shoot", Content.Load<SoundEffect>("Music/"+ WEAPON +"-shot").CreateInstance() },
                { "Reload", Content.Load<SoundEffect>("Music/"+ WEAPON +"-reload").CreateInstance() },
                { "Melee", Content.Load<SoundEffect>("Music/knife-slash").CreateInstance() },
                { "Hurt", Content.Load<SoundEffect>("Music/hurt").CreateInstance() },
                { "Warp", Content.Load<SoundEffect>("Music/teleport").CreateInstance() },
                { "Walk", Content.Load<SoundEffect>("Music/footstep").CreateInstance() },
                { "Rapid-Fire", Content.Load<SoundEffect>("Music/rifle-shot").CreateInstance() }
            };
            // set the actualy key presses on the keyboard
            Input = new Input() { Cursor = new Cursor(Content), Down = Keys.S, Left = Keys.A, Right = Keys.D, Up = Keys.W, Teleport = Keys.E, Reload = Keys.R, Punch = Keys.F, Throw = Keys.G };

            // icon names to load from recources
            string[] icons = new string[] { "shoot", "reload", "melee", "warp" };
            // create a new list and add the icons to the skills
            skills = new List<Button>();

            // load default font from resources
            var buttonFont = Content.Load<SpriteFont>("Font");

            for (int i = 0; i < icons.Length; i++)
            {
                // for each icon or skill name load from recources and set the position
                var text = icons[i];
                var buttonTexture = Content.Load<Texture2D>("icons/"+ text+"_icon");

                var button = new Button(buttonTexture, buttonFont, 50, 50, (i * 3))
                {
                    Position = new Vector2((gameWidth/2 - 100) + i * 55, gameHeight - 100)
                };

                skills.Add(button);
            }

            CreateAnimations(Content);
        }

        public void Update(GameTime gameTime)
        {
            // update game time
            this.gameTime = gameTime;
            // update skill time
            foreach (var skil in skills)
                skil.Update(gameTime);

            base.Update();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // draw the fighter/player animation on the screen
            GetAnimation().Draw(spriteBatch, _physics.rotation, new Vector2(156, 140), _physics.position, _physics.scale, Color.White);

            // draw skill icons on the screen
            foreach (var skil in skills)
                skil.Draw(gameTime, spriteBatch);
        }

        public override void DoPhysics()
        {
            Input.Cursor.Update(gameTime);

            // Rotation regarding the cursor class
            _physics.rotation = OwnMath.CalculateAngleBetweenPoints(Position.ToPoint(), Input.Cursor.Position.ToPoint());
            
            // Make sure that the player does not go out of bounds
            Position = new Vector2(MathHelper.Clamp(Position.X, 80, gameWidth-80),
                MathHelper.Clamp(Position.Y, 80, gameHeight-80));
        }

        public override void DoAnimation()
        {
            foreach (var pair in _animations)
            {
                pair.Value.Update(gameTime);
            }

            abilityCharge = MathHelper.Clamp(abilityCharge, 0, FULL_CHARGE);
        }

        public override void DoMovement()
        {
            // if using skill of teleport then wait untill finished
            if (currentAnimationKey == WARP_ANIMATION_KEY)
            {
                if (WaitForTime(0.7f))
                {
                    Position = Input.Cursor.Position;
                    skills[3].CanUse = false;
                }
                
                return;
            }
            // if using skill of reload then wait untill finished
            if (currentAnimationKey == RELOAD_ANIMATION_KEY)
            {

                if (WaitForTime(1f))
                {
                    bulletsCounter = MAGAZINE_SIZE;
                    skills[1].CanUse = false;
                }

                return;
            }
            // if using skill of melee then wait untill finished
            if (currentAnimationKey == MELEE_ANIMATION_KEY)
            {

                if (WaitForTime(0.5f))
                {
                    punchSignal.Invoke();
                    skills[2].CanUse = false;
                }

                return;
            }
            // if using skill of ability then wait untill finished
            if (abilityActivated)
            {
                if (shootDelayTimer < 8)
                {
                    abilityCharge = 0;
                    currentAnimationKey = SHOOT_ANIMATION_KEY;
                    shootDelayTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    SoundManager.PlaySound(sounds["Rapid-Fire"]);
                    shootSignal.Invoke();
                    
                }
                else
                {
                    abilityActivated = false;
                }
                return;
            }

            MouseState mouse = Mouse.GetState();

            // if can use ability and pressed the button then activate and wait
            if (mouse.RightButton == ButtonState.Pressed && abilityCharge >= FULL_CHARGE)
                abilityActivated = true;

            // Detect Key presses that matches the current player input

            currentAnimationKey = IDLE_ANIMATION_KEY;

            foreach (var key in Input.GetKeys())
            {
                // add velocity to player position according to the key pressed
                if (Keyboard.GetState().IsKeyDown(key))
                {
                    _physics.position.X += (key == Input.Left) ? -_physics.speed : 0;
                    _physics.position.Y += (key == Input.Down) ? _physics.speed : 0;
                    _physics.position.X += (key == Input.Right) ? _physics.speed : 0;
                    _physics.position.Y += (key == Input.Up) ? -_physics.speed : 0;

                    SoundManager.PlaySound(sounds["Walk"]);
                    currentAnimationKey = MOVE_ANIMATION_KEY;
                }
            }

            // Shoot
            if (mouse.LeftButton == ButtonState.Pressed && bulletsCounter > 0)
            {
                currentAnimationKey = SHOOT_ANIMATION_KEY; // set animation to shooting
                shootDelayTimer += gameTime.ElapsedGameTime.Milliseconds; // count mili seconds passed when pressed fire
                if (shootDelayTimer > FIRE_RATE) // check timer according to fire rate and reset it when invoking the event
                {
                    shootDelayTimer = 0;
                    shootSignal.Invoke();
                    SoundManager.PlaySound(sounds["Shoot"]);
                }
            }

            // reset timer when not shooting
            if (mouse.LeftButton == ButtonState.Released)
            {
                shootDelayTimer = 0;
            }

            // Reload
            if(Keyboard.GetState().IsKeyDown(Input.Reload) && skills[1].CanUse)
            {
                SoundManager.PlaySound(sounds["Reload"]);
                currentAnimationKey = RELOAD_ANIMATION_KEY;
            }

            // Punch
            if (Keyboard.GetState().IsKeyDown(Input.Punch) && skills[2].CanUse)
            {
                SoundManager.PlaySound(sounds["Melee"]);
                currentAnimationKey = MELEE_ANIMATION_KEY;
            }

            // Teleport
            if (Keyboard.GetState().IsKeyDown(Input.Teleport) && skills[3].CanUse)
            {
                SoundManager.PlaySound(sounds["Warp"]);
                currentAnimationKey = WARP_ANIMATION_KEY;
            }
        }

        // function to wait for sometime before continuing
        Boolean WaitForTime(float WAIT_FOR)
        {
            timer += gameTime.ElapsedGameTime.TotalSeconds;

            if (timer >= WAIT_FOR)
            {
                currentAnimationKey = IDLE_ANIMATION_KEY;
                timer = 0;
                return true;
            }

            return false;
        }

        public override void TakeDamage(double damage)
        {
            base.TakeDamage(damage);

            if (IsAlive)
                SoundManager.PlaySound(sounds["Hurt"]);
        }

        // load different animation sprite sheet and create a new animation to add to the dictionary
        protected override void CreateAnimations(ContentManager Content)
        {
            var warpAnimation = Content.Load<Texture2D>(WARP_ANIMATION_KEY);

            var idleAnimation = Content.Load<Texture2D>("Sprites/"+WEAPON + "/" +  WEAPON + "_" + IDLE_ANIMATION_KEY);
            var moveAnimation = Content.Load<Texture2D> ("Sprites/" + WEAPON + "/" + WEAPON + "_" + MOVE_ANIMATION_KEY);
            var shootAnimation = Content.Load<Texture2D>("Sprites/" + WEAPON + "/" + WEAPON + "_" + SHOOT_ANIMATION_KEY);
            var reloadAnimation = Content.Load<Texture2D>("Sprites/" + WEAPON + "/" + WEAPON + "_" + RELOAD_ANIMATION_KEY);
            var meleeAnimation = Content.Load<Texture2D>("Sprites/" + WEAPON + "/" + WEAPON + "_" + MELEE_ANIMATION_KEY);

            if (WEAPON == "rifle")
            {
                _animations.Add(IDLE_ANIMATION_KEY, new Animation(idleAnimation, 20, 4, 5, 313, 207));
                _animations.Add(MOVE_ANIMATION_KEY, new Animation(moveAnimation, 20, 4, 5, 313, 206));
                _animations.Add(SHOOT_ANIMATION_KEY, new Animation(shootAnimation, 3, 1, 3, 312, 206));
                _animations.Add(RELOAD_ANIMATION_KEY, new Animation(reloadAnimation, 20, 5, 4, 332, 227, 80));
                _animations.Add(MELEE_ANIMATION_KEY, new Animation(meleeAnimation, 15, 3, 5, 358, 353));
            }
            if (WEAPON == "handgun")
            {
                _animations.Add(IDLE_ANIMATION_KEY, new Animation(idleAnimation, 20, 5, 4, 253, 216));
                _animations.Add(MOVE_ANIMATION_KEY, new Animation(moveAnimation, 20, 3, 7, 258, 220));
                _animations.Add(SHOOT_ANIMATION_KEY, new Animation(shootAnimation, 3, 1, 3, 255, 215, 150));
                _animations.Add(RELOAD_ANIMATION_KEY, new Animation(reloadAnimation, 15, 5, 3, 260, 230, 80));
                _animations.Add(MELEE_ANIMATION_KEY, new Animation(meleeAnimation, 15, 5, 3, 291, 256));
            }

            if (WEAPON == "shotgun")
            {
                _animations.Add(IDLE_ANIMATION_KEY, new Animation(idleAnimation, 20, 7, 3, 313, 207));
                _animations.Add(MOVE_ANIMATION_KEY, new Animation(moveAnimation, 20, 7, 3, 313, 206));
                _animations.Add(SHOOT_ANIMATION_KEY, new Animation(shootAnimation, 3, 1, 3, 312, 206, 300));
                _animations.Add(RELOAD_ANIMATION_KEY, new Animation(reloadAnimation, 20, 7, 3, 322, 217, 120));
                _animations.Add(MELEE_ANIMATION_KEY, new Animation(meleeAnimation, 15, 3, 5, 358, 353));
            }
            _animations.Add(WARP_ANIMATION_KEY, new Animation(warpAnimation, 25, 5, 5, 192, 192));
        }
    }
}