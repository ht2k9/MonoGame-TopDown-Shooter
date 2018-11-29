using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TopDownShooter.GameObjects;
using Microsoft.Xna.Framework.Content;
using TopDownShooter.Utilities;
using TopDownShooter.Logic;
using Database;
using TopDownShooter.Utils;
using Microsoft.Xna.Framework.Media;

namespace TopDownShooter
{
    class GameManager
    {
        private const int PUNCH_DISTANCE = 3;
        private const int SPAWN_BORDER_OFFSET = 100;

        private int zombiesKilledCounter;
        private int width;
        private int height;

        private Vector2 defaultZombieScale;

        private List<Zombie> zombies;
        private List<Blood> bloodAnimations;
        private List<Bullet> bulletsFired;

        private Random rng;
        private ContentManager Content;
        private Player currentPlayer;

        private double zombieSpawnTimer;
        private double zombieTimer;

        Texture2D bulletTexture;
        Texture2D chargeTexture;
        SpriteFont bigFont;

        private Room CurrentRoom;

        private string[] zombieTypes;

        public GameManager(ContentManager Content, Player target, int width, int height, Room room)
        { 
            rng = new Random();
            
            // get the current player
            currentPlayer = target;
            CurrentRoom = room;

            this.Content = Content;
            // width and height of the screen
            this.width = width;
            this.height = height;

            zombiesKilledCounter = 0;
            zombieTimer = 0;
            // 1000 mili second to spawn first zombie
            zombieSpawnTimer = 5000;
            // zombie scale
            defaultZombieScale = new Vector2(0.45f, 0.45f);
            // initialize lists
            zombies = new List<Zombie>();
            bloodAnimations = new List<Blood>();
            bulletsFired = new List<Bullet>();
            // load textures
            bulletTexture = Content.Load<Texture2D>("bullet");
            chargeTexture = Content.Load<Texture2D>("blank");
            bigFont = Content.Load<SpriteFont>("BigFont");

            SoundManager.LoopSound(Content.Load<Song>("Music/zombie-background"));


            zombieTypes = Data.GetZombie();
        }

        public void Update(GameTime gameTime)
        {
            SpawnZombie(gameTime);

            // if any blood is animated then we make it disapear
            bloodAnimations.RemoveAll((b) => { return b.Active == false; });

            // animate the bullets the player fired
            foreach (var bullet in bulletsFired)
            {
                bullet.Update(gameTime);
            }

            // animate all blood that is activated
            foreach (var blood in bloodAnimations)
            {
                blood.Update(gameTime);
            }

            // checks and remove the zombie with 0 health (or dead) from the list 
            for (int i = 0; i < zombies.Count; i++)
            {
                if (!zombies[i].IsAlive)
                {
                    // increase kill points
                    zombiesKilledCounter++;
                    // remove zombie from zombie list
                    zombies.RemoveAt(i);
                    // when removing from a list, the zombie count decreases
                    i--;
                }
            }
            // update the zombie (important to do it after, so we wont animate the dead zombies)
            foreach (var zombie in zombies)
            {
                zombie.Update(gameTime, currentPlayer.Position);
            }
            // Checks the collision between the bullets and the zombies
            for (int i = 0; i < bulletsFired.Count; i++)
            {
                // the current bullet
                var bullet = bulletsFired[i];
                for (int j = 0; j < zombies.Count; j++)
                {
                    // current zombie
                    var zombie = zombies[j];
                    // the distance between the zombie and the bullet
                    var dist = OwnMath.GetDirectionToPoint(bullet.Position.ToPoint(), zombie.Position.ToPoint());
                    // if the distance is less than 20 (about zombie width)
                    // and the bullet is not removed already for other reason
                    if (dist.Length() < 20 && bulletsFired.Count > i)
                    {
                        // bullet hits, add points to the fighter ability charge
                        currentPlayer.abilityCharge += (int)bullet.damage;
                        bulletsFired.RemoveAt(i);
                        zombie.TakeDamage(bullet.damage);
                        bloodAnimations.Add(new Blood(Content, zombie.Position));
                    }
                }

                for (int j = 0; j < CurrentRoom.collideables.Count; j++)
                {
                    var col = CurrentRoom.collideables[j];

                    if (bullet.Rectangle.Intersects(col.Rectangle))
                    {
                        bulletsFired.Remove(bullet);
                        if (col.TakeDamage())
                        {
                            CurrentRoom.collideables.Remove(col);
                            if (col.Type == Component.ObjectType.SPAWN_POINT)
                                CurrentRoom.spawnPlaces.Remove(col.Position);
                        }
                    }
                }

                // if the bullet reached the screen bounds, remove it from the list
                if (bullet.Position.X > width - 50 || bullet.Position.Y > height - 50 || bullet.Position.X < 50 || bullet.Position.Y < 50)
                {
                    // if the bullet is not removed already for other reason
                    if (bulletsFired.Count > i)
                        bulletsFired.RemoveAt(i);
                }
            }

            if (CurrentRoom.DoorOpened)
            {
                Rectangle r1 = new Rectangle((int)currentPlayer.Position.X, (int)currentPlayer.Position.Y, currentPlayer.GetAnimation().Width / 2, currentPlayer.GetAnimation().Height / 2);
                if (r1.Intersects(CurrentRoom.Exit.Rectangle))
                {
                    Data.SetScore(Data.GetScore() + zombiesKilledCounter);
                    CurrentRoom.LoadMap();
                    ResetGame();
                }
            }
        }

        void ResetGame ()
        {
            currentPlayer.Position = CurrentRoom.Enterance;

            zombiesKilledCounter = 0;
            zombieTimer = 0;
            // 1000 mili second to spawn first zombie
            zombieSpawnTimer = 5000;
        }

        void SpawnZombie(GameTime gameTime)
        {
            var pos = CurrentRoom.GetSpawnPosition();
            // add seconds to zombie timer
            if (pos == Vector2.Zero)
            {
                if (zombies.Count == 0)
                {
                    CurrentRoom.DoorOpened = true;
                    SoundManager.StopSound();
                }
                return;
            }

            zombieTimer += gameTime.ElapsedGameTime.Milliseconds;
            // check if the timer has reached a some time
            // and zombies still spawning and didn't reach the current number
            if (zombieTimer >= zombieSpawnTimer)
            {
                // get random zombie number to spawn (not all at once)
                for (int i = 0; i < rng.Next(1, 3); i++)
                {
                    // creates a new zombie
                    Zombie z = new Zombie(Content, width, height, zombieTypes[rng.Next(0, CurrentRoom.currentLevel)]);
                    z.Position = pos;
                    // pass function to zombie event
                    z.zombieAttack += ZombieAttack;
                    // add zombie to the zombies list
                    zombies.Add(z);
                }
                // get random time to spawn next zombie
                zombieSpawnTimer = rng.Next(1500, 3500);
                zombieTimer = 0;
            }
        }

        private void ZombieAttack()
        {
            // Deal damage to the target when zombie attacks
            currentPlayer.TakeDamage(zombies.FirstOrDefault().Damage);
        }

        public void PunchZombies()
        {
            Rectangle r1 = new Rectangle((int)currentPlayer.Position.X, (int)currentPlayer.Position.Y, currentPlayer.GetAnimation().Width / 2, currentPlayer.GetAnimation().Height / 2);

            // create rectangle for the fighter and other for each zombie to check if it intersects when punching
            // if yes, the zombie takes 10 melee damage
            foreach (Zombie z in zombies)
            {
                Rectangle r2 = new Rectangle((int) z.Position.X, (int) z.Position.Y, z.GetAnimation().Width / 2, z.GetAnimation().Height / 2);
                
               if (r1.Intersects(r2))
                {
                    z.TakeDamage(10);
                    return;
                }
            }
        }

        public void ShootZombies()
        {
            // Create a bullet and add the damage that it will deal to the zombie
            Bullet bullet = new Bullet(bulletTexture, currentPlayer);
            bulletsFired.Add(bullet);
            currentPlayer.bulletsCounter--;
        }

        public void Draw(GameTime gameTime, SpriteFont font, SpriteBatch spriteBatch)
        {
            // For most cases, to write on screen, we pass the default font and the position with a colro to draw
            spriteBatch.Draw(Content.Load<Texture2D>("blank"), 
                new Rectangle(0, 0, width, height), 
                Color.Black);

            CurrentRoom.Draw(gameTime, spriteBatch);

            // Draw the current zombie points (the kills) 
            spriteBatch.DrawString(font, "KILLS: " + zombiesKilledCounter, new Vector2(250, 10), Color.Gold);
            // Draw the current zombie points (the kills) 
            spriteBatch.Draw(chargeTexture, new Rectangle(10, 10, (int) currentPlayer.Health, 20), Color.Red);
            // draw the target charge meter using the abilityCharge variable from the current player
            spriteBatch.Draw(chargeTexture, new Rectangle(10, 25, currentPlayer.abilityCharge, 20), Color.BlueViolet);

            // draw the player magazine
            for (int i = 0; i < currentPlayer.bulletsCounter; i++)
            {
                spriteBatch.Draw(bulletTexture, new Vector2(width - 50, 10+i*10), Color.White);
            }
            // pass to each zombie draw function
            foreach (var zombie in zombies)
            {
                zombie.Draw(spriteBatch);
            }
            // pass to each blood draw function in it's class
            foreach (var blood in bloodAnimations)
            {
                blood.Draw(spriteBatch);
            }
            // pass to each bullet in the list to draw
            foreach (var bullet in bulletsFired)
            {
                bullet.Draw(gameTime, spriteBatch);
            }
        }
    }

}

