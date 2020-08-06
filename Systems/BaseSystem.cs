using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSpaceShooter.Utilities;

namespace MonoSpaceShooter.Systems
{
    public class BaseSystem
    {
        public World world;

        public BaseSystem(World w)
        {
            world = w;
        }

        public virtual void Update(GameTime gameTime)
        {

        }

        public virtual void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont)
        {

        }
    }
}
