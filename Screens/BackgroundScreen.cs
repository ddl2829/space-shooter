using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceShooter;

namespace MonoSpaceShooter.Screens
{
    public class BackgroundScreen : BaseScreen
    {
        List<BackgroundElement> backgroundObjects;

        public BackgroundScreen() : base()
        {
            backgroundObjects = new List<BackgroundElement>();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (backgroundObjects.Count < 15)
            {
                backgroundObjects.Add(new BackgroundElement(Game1.instance.backgroundElements, Game1.instance.screenBounds));
            }

            //Update background objects
            for (int i = backgroundObjects.Count - 1; i >= 0; i--)
            {
                backgroundObjects[i].Update(gameTime);
                if (backgroundObjects[i].belowScreen)
                {
                    backgroundObjects.RemoveAt(i);
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            base.Draw(spriteBatch, spriteFont);
            spriteBatch.Draw(Game1.instance.background, Game1.instance.screenBounds, Color.White);

            foreach (BackgroundElement element in backgroundObjects)
            {
                element.Draw(spriteBatch);
            }
        }
    }
}
