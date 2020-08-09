using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoSpaceShooter.Components
{
    public class RenderComponent : BaseComponent
    {
        public int currentTexture = 0;
        public List<Texture2D> textures;
        public bool visible = true;

        public Texture2D CurrentTexture
        {
            get
            {
                return textures[currentTexture];
            }
        }

        public RenderComponent(Texture2D t)
        {
            textures = new List<Texture2D>();
            textures.Add(t);
        }

        public RenderComponent(Texture2D[] t)
        {
            textures = new List<Texture2D>(t);
        }
    }
}
