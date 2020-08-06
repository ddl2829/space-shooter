using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSpaceShooter;
using MonoSpaceShooter.Components;
using MonoSpaceShooter.Entities;

namespace SpaceShooter
{
    public class Enemy : QuadStorable, Drawable
    {
        Texture2D texture;
        Vector2 position;
        Vector2 lastPosition;
        Vector2 motion;
        public bool visible = true;
        public float health = 80;
        public float baseHealth = 80;
        double shotInterval;
        double timeSinceLastShot;

        public Rectangle Bounds
        {
            get { return new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height); }
        }

        Rectangle QuadStorable.Rect
        {
            get { return Bounds; }
        }

        bool QuadStorable.HasMoved
        {
            get { return lastPosition == position; }
        }

        public Enemy(Texture2D texture, Vector2 position, double fireInterval, double fireDelay)
        {
            this.texture = texture;
            this.position = position;
            this.motion = Vector2.Zero;
            this.timeSinceLastShot = fireDelay;
            this.shotInterval = fireInterval;
            if (fireDelay > fireInterval)
            {
                this.shotInterval = fireDelay - fireInterval;
            }
        }

        public void Damage(float amount)
        {
            health -= amount;
            if (health <= 0)
            {
                visible = false;
                int credit = 3;
                Game1.instance.kills += credit;
                Game1.instance.playerScore += credit;
                Entity e = new Entity();
                e.AddComponent(new PositionComponent(position));
                e.AddComponent(new NotificationComponent("+" + credit * 100, 200, false));
                Game1.instance.world.AddEntity(e);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (visible)
            {
                spriteBatch.Draw(texture, position, Color.White);
            }
        }

        public void Update(GameTime gameTime)
        {
            if (Game1.instance.player.laserLevel != 0)
            {
                if (this.Bounds.Y < 0)
                {
                    this.health = this.baseHealth * 1.2f * Game1.instance.player.laserLevel;
                }
            }
            lastPosition = position;
            timeSinceLastShot += gameTime.ElapsedGameTime.Milliseconds;
            if (timeSinceLastShot >= shotInterval && position.Y < Game1.instance.player.position.Y)
            {
                Shoot();
            }

            if (position.Y > Game1.instance.screenBounds.Height || position.X > Game1.instance.screenBounds.Width || position.X < 0 - texture.Width)
            {
                visible = false;
            }

            motion.X = 0;
            motion.Y = 1;
            float movement = position.X - Game1.instance.player.position.X;
            if (position.Y > 0 && position.Y < Game1.instance.player.position.Y)
            {
                if (movement > 0)
                {
                    motion.X = -.5f;
                }
                else
                {
                    motion.X = .5f;
                }
            }
            position += motion * gameTime.ElapsedGameTime.Milliseconds / 10;
        }

        public void Shoot()
        {
            timeSinceLastShot = 0;
            Game1.instance.quadTree.Add(new EnemyLaser(Game1.instance.laserGreen, position));
        }
    }
}
