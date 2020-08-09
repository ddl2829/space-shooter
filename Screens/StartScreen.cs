using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpaceShooter;

namespace MonoSpaceShooter.Screens
{
    public class StartScreen : BaseScreen
    {
        double timeSinceLastFlash = 0;
        double flashInterval = 500;
        bool flashing = false;

        public StartScreen() : base()
        {
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            KeyboardState keyboardState = Keyboard.GetState();

            timeSinceLastFlash += gameTime.ElapsedGameTime.Milliseconds;
            if (timeSinceLastFlash > flashInterval)
            {
                flashing = !flashing;
                timeSinceLastFlash = 0;
            }
            if (keyboardState.IsKeyDown(Keys.Enter))
            {
                Game1.instance.PopScreen();
                Game1.instance.PushScreen(new GameScreen());
                return;
            }
            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                Game1.instance.Exit();
            }
        }

        public override void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            base.Draw(spriteBatch, spriteFont);
            spriteBatch.DrawString(spriteFont, "Simple Space Shooter", new Vector2(Game1.instance.screenBounds.Width / 2 - spriteFont.MeasureString("Simple Space Shooter").X / 2, Game1.instance.screenBounds.Height / 4), Color.White);
            Color flashColor = flashing ? Color.White : Color.Yellow;
            spriteBatch.DrawString(spriteFont, "Press Enter to Play", new Vector2(Game1.instance.screenBounds.Width / 2 - spriteFont.MeasureString("Press Enter to Play").X / 2, Game1.instance.screenBounds.Height / 3 * 2), flashColor);
            spriteBatch.DrawString(spriteFont, "Press Escape to Quit", new Vector2(Game1.instance.screenBounds.Width / 2 - spriteFont.MeasureString("Press Escape to Quit").X / 2, Game1.instance.screenBounds.Height / 4 * 3), Color.White);
        }
    }
}
