using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoSpaceShooter.Components
{
    public class RenderComponent : BaseComponent
    {
        public Texture2D texture;

        public RenderComponent(Texture2D t)
        {
            texture = t;
        }
    }
}
