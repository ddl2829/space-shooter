using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoSpaceShooter;
using MonoSpaceShooter.Components;
using MonoSpaceShooter.Entities;

namespace SpaceShooter
{
    public class Player : Drawable
    {
        #region Variables

        public Vector2 position;
        Vector2 motion;

        float shipSpeed = 5.0f;

        public int lives;
        int maxLives = 5;

        public bool invincible;
        bool visible;
        public bool shielded;
        public bool shieldCooldown;

        double timeSinceRespawn;
        double timeInvincibleAfterRespawn = 3000;
        public double shieldPower = 3000;
        public double maxShieldPower = 3000;
        double shieldRegenRate = 0.3f;
        double shieldDepleteRate = 1.0f;

        List<Texture2D> textures;
        Texture2D shield = Game1.instance.playerShield;

        int currentTexture = 0;

        Rectangle screenBounds;

        public int laserLevel = 0;

        double lastFireTime = 0;

        KeyboardState keyboardState;

        #endregion

        #region Fields

        public Rectangle Bounds
        {
            get {
                if (shielded)
                {
                    return new Rectangle((int)position.X - 25, (int)position.Y - 30, shield.Width, shield.Height);
                }
                return new Rectangle((int)position.X, (int)position.Y, textures[currentTexture].Width, textures[currentTexture].Height);
            }
        }

        #endregion

        public Player(List<Texture2D> textures, Rectangle screenBounds) : base()
        {
            this.textures = textures;
            this.screenBounds = screenBounds;
            this.lives = maxLives;
            visible = true;
            shielded = false;
            shieldCooldown = false;
            setInStartPosition();
        }

        public void setInStartPosition()
        {
            currentTexture = 0;
            position.X = ((screenBounds.Width / 2) - (textures[currentTexture].Width / 2));
            position.Y = ((screenBounds.Height / 3) * 2 + (textures[currentTexture].Height / 2));
        }

        public void Update(GameTime gameTime)
        {
            if (!shielded && shieldPower < maxShieldPower)
            {
                shieldPower += shieldRegenRate * gameTime.ElapsedGameTime.Milliseconds;
            }
            if (shieldPower <= 0)
            {
                shielded = false;
                shieldCooldown = true;
                shieldPower = 0;
            }
            if (shieldPower >= maxShieldPower)
            {
                shieldPower = maxShieldPower;
                shieldCooldown = false;
            }
            if (shielded && !(timeSinceRespawn < timeInvincibleAfterRespawn))
            {
                shieldPower -= shieldDepleteRate * gameTime.ElapsedGameTime.Milliseconds;
            }

            bool upgradedLasers = false;
            if (Game1.instance.kills > 20 && laserLevel == 0)
            {
                laserLevel = 1;
                upgradedLasers = true;
            }
            if (Game1.instance.kills > 50 && laserLevel == 1)
            {
                laserLevel = 2;
                upgradedLasers = true;
            }
            if (Game1.instance.kills > 100 && laserLevel == 2)
            {
                laserLevel = 3;
                upgradedLasers = true;
            }

            if (upgradedLasers)
            {
                Entity e = new Entity();
                e.AddComponent(new PositionComponent(position));
                e.AddComponent(new NotificationComponent("Lasers Improved", 2000, true));
                Game1.instance.world.AddEntity(e);
            }

            timeSinceRespawn += gameTime.ElapsedGameTime.Milliseconds;
            if (timeSinceRespawn > timeInvincibleAfterRespawn && invincible)
            {
                invincible = false;
            }
            else if (timeSinceRespawn < timeInvincibleAfterRespawn && lives < maxLives)
            {
                if (timeSinceRespawn % 10 == 0)
                {
                    visible = false;
                }
                else
                {
                    visible = true;
                }
            }

            lastFireTime += gameTime.ElapsedGameTime.Milliseconds;
            motion = Vector2.Zero;
            keyboardState = Keyboard.GetState();

            if (!shieldCooldown)
            {
                if (keyboardState.IsKeyDown(Keys.LeftShift))
                {
                    Shield(true);
                }
                else
                {
                    Shield(false);
                }
            }

            if (keyboardState.IsKeyDown(Keys.Space) && !shielded)
            {
                Shoot(gameTime);
            }

            if (keyboardState.IsKeyDown(Keys.Left))
            {
                currentTexture = 1;
                motion.X = -1;
            }
            if (keyboardState.IsKeyDown(Keys.Right))
            {
                currentTexture = 2;
                motion.X = 1;
            }
            if (keyboardState.IsKeyDown(Keys.Up))
            {
                if (keyboardState.IsKeyUp(Keys.Left) && keyboardState.IsKeyUp(Keys.Right))
                {
                    currentTexture = 0;
                }
                motion.Y = -1;
            }
            if (keyboardState.IsKeyDown(Keys.Down))
            {
                if (keyboardState.IsKeyUp(Keys.Left) && keyboardState.IsKeyUp(Keys.Right))
                {
                    currentTexture = 0;
                }
                motion.Y = 1;
            }
            if (keyboardState.IsKeyUp(Keys.Left) && keyboardState.IsKeyUp(Keys.Right))
            {
                currentTexture = 0;
            }

            position += motion * shipSpeed;

            LockToScreen();
        }

        private void Shield(bool active)
        {
            if (active)
            {
                if (shieldPower >= 0)
                {
                    shielded = true;
                    invincible = true;
                    return;
                }
            }
            shielded = false;
            invincible = false;
        }

        private void LockToScreen()
        {
            if (position.X < 0)
            {
                position.X = 0;
            }
            if (position.Y < 0)
            {
                position.Y = 0;
            }
            if (position.X > screenBounds.Width - textures[currentTexture].Width)
            {
                position.X = screenBounds.Width - textures[currentTexture].Width;
            }
            if (position.Y > screenBounds.Height - textures[currentTexture].Height)
            {
                position.Y = screenBounds.Height - textures[currentTexture].Height;
            }
        }

        private void Shoot(GameTime gameTime)
        {
            if (lastFireTime > 50)
            {
                Texture2D laser = Game1.instance.laserRed;
                if (laserLevel >= 1)
                {
                    laser = Game1.instance.laserGreen;
                }
                if (laserLevel < 2)
                {
                    Entity newLaser = new Entity();
                    newLaser.AddComponent(new RenderComponent(laser));
                    newLaser.AddComponent(new SpeedComponent(new Vector2(0, -20)));
                    newLaser.AddComponent(new PositionComponent(new Vector2(position.X + textures[currentTexture].Width / 2 - laser.Width / 2, position.Y - 30)));
                    newLaser.AddComponent(new DealsDamageComponent(laserLevel));
                    Game1.instance.world.AddEntity(newLaser);

                    //Laser newLaser = new Laser(laser, new Vector2(position.X + textures[currentTexture].Width / 2 - laser.Width / 2, position.Y - 30), laserLevel);
                    //Game1.instance.quadTree.Add(newLaser);
                }
                if (laserLevel == 2)
                {
                    Laser newLaser1 = new Laser(laser, new Vector2(position.X + textures[currentTexture].Width / 3 - laser.Width / 2, position.Y - 30), laserLevel);
                    Laser newLaser2 = new Laser(laser, new Vector2(position.X + textures[currentTexture].Width / 3 * 2 - laser.Width / 2, position.Y - 30), laserLevel);
                    Game1.instance.quadTree.Add(newLaser1);
                    Game1.instance.quadTree.Add(newLaser2);
                }
                if (laserLevel == 3)
                {
                    Laser newLaser1 = new Laser(laser, new Vector2(position.X + textures[currentTexture].Width / 3 - laser.Width / 2, position.Y - 30), laserLevel);
                    Laser newLaser2 = new Laser(laser, new Vector2(position.X + textures[currentTexture].Width / 3 * 2 - laser.Width / 2, position.Y - 30), laserLevel);
                    Game1.instance.quadTree.Add(newLaser1);
                    Game1.instance.quadTree.Add(newLaser2);
                    Laser right = new Laser(laser, new Vector2(position.X + textures[currentTexture].Width / 3 - laser.Width / 2, position.Y - 30), laserLevel);
                    Laser left = new Laser(laser, new Vector2(position.X + textures[currentTexture].Width / 3 * 2 - laser.Width / 2, position.Y - 30), laserLevel);
                    right.motion.X = 1;
                    right.motion.Y = -1;
                    left.motion.X = -1;
                    left.motion.Y = -1;
                    Game1.instance.quadTree.Add(right);
                    Game1.instance.quadTree.Add(left);
                }
                lastFireTime = 0;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (visible)
            {
                if (shielded)
                {
                    spriteBatch.Draw(shield, new Rectangle((int)position.X - 25, (int)position.Y - 30, shield.Width, shield.Height), Color.White);
                }
                spriteBatch.Draw(textures[currentTexture], this.position, Color.White);
            }
        }

        public void Reset()
        {
            this.lives = maxLives;
            setInStartPosition();
            laserLevel = 0;
            shieldPower = maxShieldPower;
            shieldCooldown = false;
        }

        public void Respawn()
        {
            timeSinceRespawn = 0;
            setInStartPosition();
            invincible = true;
            Game1.instance.kills = 0;
            laserLevel = 0;
            shieldCooldown = false;
            shieldPower = maxShieldPower;
            List<QuadStorable> enemies = Game1.instance.quadTree.GetObjects<Enemy>(Game1.instance.screenBounds);
            foreach (Enemy enemy in enemies)
            {
                if (enemy.Bounds.Y < 0)
                {
                    enemy.health = enemy.baseHealth;
                }
            }

            List<QuadStorable> meteors = Game1.instance.quadTree.GetObjects<Meteor>(Game1.instance.screenBounds);
            foreach (Meteor meteor in meteors)
            {
                if (meteor.Bounds.Y < 0)
                {
                    meteor.health = meteor.baseHealth;
                }
            }
        }
    }
}
