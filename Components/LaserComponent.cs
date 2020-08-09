using System;
using Microsoft.Xna.Framework.Graphics;

namespace MonoSpaceShooter.Components
{
    public class LaserComponent : BaseComponent
    {
        public Texture2D explosionTexture;
        public LaserComponent(Texture2D explosion) : base()
        {
            explosionTexture = explosion;
        }
    }
}
