using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSpaceShooter;

namespace SpaceShooter
{
    public class Laser : QuadStorable, Drawable
    {
        #region Variables

        public Vector2 motion;
        public Vector2 position;
        public float speed = 20.0f;
        public bool visible = true;
        public int laserLevel;
        public Texture2D texture;
        public Rectangle bounds;

        #endregion

        #region Fields

        public float Damage
        {
            get { return (laserLevel + 1) * 10; }
        }

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
            get { return true; }
        }


        #endregion


        public Laser(Texture2D texture, Vector2 position, int laserLevel)
        {
            motion = Vector2.Zero;
            this.position = position;
            this.texture = texture;
            this.bounds = texture.Bounds;
            this.visible = true;
            this.laserLevel = laserLevel;
            motion.Y = -1;
        }
        public virtual void Update()
        {
            if (position.Y < 0)
            {
                visible = false;
            }
            position += motion * speed;
        }
        public virtual void Update(GameTime gameTime)
        {
            if (position.Y < 0 || position.X > Game1.instance.screenBounds.Width || position.X < 0 - texture.Width)
            {
                visible = false;
            }
            position += motion * speed;
        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (visible)
            {
                spriteBatch.Draw(texture, position, Color.White);
            }
        }
    }
}
