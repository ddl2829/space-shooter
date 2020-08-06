using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceShooter
{
    public class BackgroundElement
    {
        Rectangle screenBounds;
        Vector2 position;
        Vector2 motion;
        List<Texture2D> textures;
        public bool belowScreen = false;
        static int textureID = 1;
        int useTexture;

        public BackgroundElement(List<Texture2D> textures, Rectangle screenBounds)
        {
            this.textures = textures;
            this.screenBounds = screenBounds;
            Random rand = new Random();
            position = new Vector2(rand.Next(0, screenBounds.Width), rand.Next(-500, -100));
            //motion = new Vector2(0, rand.Next(3, 6));
            motion = Vector2.Zero;
            motion.Y = 3;
            textureID += 1;
            if (textureID > textures.Count - 1)
            {
                textureID = 0;
            }
            useTexture = textureID;
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            position += motion * gameTime.ElapsedGameTime.Milliseconds/10;
            if (position.Y > screenBounds.Height)
            {
                belowScreen = true;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(textures[useTexture], new Rectangle((int)position.X, (int)position.Y, textures[useTexture].Width, textures[useTexture].Height), Color.White);
        }
    }
}
