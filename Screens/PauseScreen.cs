using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using SpaceShooter;

namespace MonoSpaceShooter.Screens
{
    public class PauseScreen : BaseScreen
    {
        public PauseScreen() : base()
        {
            pausesBelow = true;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (KeyPressed(Keys.Escape))
            {
                MediaPlayer.Resume();
                Game1.instance.PopScreen();
                return;
            }
            if (keyboardState.IsKeyDown(Keys.Enter))
            {
                Game1.instance.PopScreen(); //pause screen
                Game1.instance.PopScreen(); //game screen
                Game1.instance.PushScreen(new GameOverScreen());

                MediaPlayer.Resume();
                return;
            }
        }

        public override void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            base.Draw(spriteBatch, spriteFont);
            spriteBatch.DrawString(spriteFont, "Paused", new Vector2((int)Game1.instance.screenBounds.Width / 2 - spriteFont.MeasureString("Paused").X / 2, (int)Game1.instance.screenBounds.Height / 3), Color.White);
            spriteBatch.DrawString(spriteFont, "Press Enter to End Game", new Vector2((int)Game1.instance.screenBounds.Width / 2 - spriteFont.MeasureString("Press Enter to End Game").X / 2, (int)Game1.instance.screenBounds.Height / 2), Color.White);
        }
    }
}
