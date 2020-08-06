using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSpaceShooter;
using MonoSpaceShooter.Components;
using MonoSpaceShooter.Entities;

namespace SpaceShooter
{
    public class Meteor : QuadStorable, Drawable
    {
        #region Variables

        Vector2 lastPosition;
        Vector2 position;
        Vector2 motion = Vector2.Zero;
        bool isLarge;
        float speed;
        Texture2D texture;
        public float health;
        public float baseHealth;
        float rotation;
        static float masterRotation = 0.0f;
        public bool visible;
        bool credited = false;

        #endregion

        #region Fields

        Rectangle QuadStorable.Rect
        {
            get { return Bounds; }
        }

        public bool Moved
        {
            get { return (this as QuadStorable).HasMoved; }
        }

        bool QuadStorable.HasMoved
        {
            get { return lastPosition == position; }
        }

        public Rectangle Bounds
        {
            get { return new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height); }
        }

        #endregion

        public void Damage(float amount)
        {
            health -= amount;
            if (health <= 0 && isLarge)
            {
                SpawnSmallMeteors();
            }
            if (!credited && health <= 0)
            {
                int credit = !isLarge ? 1 : 2;
                Game1.instance.kills += credit;
                Game1.instance.playerScore += credit;
                credited = true;

                Entity e = new Entity();
                e.AddComponent(new PositionComponent(position));
                e.AddComponent(new NotificationComponent("+" + credit * 100, 200, false));
                Game1.instance.world.AddEntity(e);

                //Game1.instance.notifications.Add(new Notification("+"+credit*100, 200, position));
            }
        }

        private void SpawnSmallMeteors()
        {
            Random rand = new Random();
            int randAmt = rand.Next(2, 6);
            for (int i = 0; i < randAmt; i++)
            {
                Meteor newMeteor = new Meteor(false, rand.Next(2, 8), this.position);
                newMeteor.motion = new Vector2(rand.Next(-3,3), rand.Next(0,2));
                Game1.instance.quadTree.Add(newMeteor);
            }
        }

        public Meteor(bool isLarge, float speed, Vector2 position)
        {
            masterRotation += .2f;
            this.rotation = masterRotation;
            this.isLarge = isLarge;
            this.texture = !isLarge ? Game1.instance.meteorSmall : Game1.instance.meteorBig;
            this.speed = speed;
            health = !isLarge ? 20 : 50;
            baseHealth = !isLarge ? 20 : 50;
            this.position = position;
            visible = true;
            motion.Y = 1;
        }

        public void Update()
        {
            if (Game1.instance.player.laserLevel != 0)
            {
                if (this.Bounds.Y < 0)
                {
                    this.health = this.baseHealth * 1.2f * Game1.instance.player.laserLevel;
                }
            }
            lastPosition = position;
            if (health <= 0)
            {
                visible = false;
            }
            if (position.Y > Game1.instance.screenBounds.Height)
            {
                visible = false;
            }
            motion.Normalize();
            position += motion * this.speed;
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
            if (health <= 0)
            {
                visible = false;
            }
            if (position.Y > Game1.instance.screenBounds.Height || position.X > Game1.instance.screenBounds.Width || position.X < 0-texture.Width)
            {
                visible = false;
            }
            motion.Normalize();
            position += motion * this.speed;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (visible)
            {
                spriteBatch.Draw(texture, position, Color.White);
            }
        }
    }
}
