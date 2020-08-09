using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoSpaceShooter.Systems;
using SpaceShooter;

namespace MonoSpaceShooter.Screens
{
    public class GameOverScreen : BaseScreen
    {
        bool flashing = false;
        double timeSinceLastFlash = 0;
        double flashInterval = 500;

        public GameOverScreen() : base()
        {
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            timeSinceLastFlash += gameTime.ElapsedGameTime.Milliseconds;
            if (timeSinceLastFlash > flashInterval)
            {
                flashing = !flashing;
                timeSinceLastFlash = 0;
            }

            if (KeyPressed(Keys.Enter))
            {
                Game1.instance.PopScreen(); //this screen
                Game1.instance.PrepareLevel();
                Game1.instance.playerScore = 0;
                Game1.instance.kills = 0;
                Game1.instance.PushScreen(new GameScreen());
                return;
            }
            if (KeyPressed(Keys.Escape))
            {
                Game1.instance.Exit();
            }
        }

        public override void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            base.Draw(spriteBatch, spriteFont);

            spriteBatch.DrawString(spriteFont, "Game Over", new Vector2((int)Game1.instance.screenBounds.Width / 2 - spriteFont.MeasureString("Game Over").X / 2, 100), Color.White);
            spriteBatch.DrawString(spriteFont, "Score: " + Math.Truncate(Game1.instance.playerScore), new Vector2((int)Game1.instance.screenBounds.Width / 2 - spriteFont.MeasureString("Score: " + Math.Truncate(Game1.instance.playerScore)).X / 2, 120), Color.White);
            spriteBatch.DrawString(spriteFont, "Level: " + LevelSystem.levelNumber, new Vector2((int)Game1.instance.screenBounds.Width / 2 - spriteFont.MeasureString("Level: " + LevelSystem.levelNumber).X / 2, 140), Color.White);
            Color flashColor = flashing ? Color.White : Color.Yellow;
            spriteBatch.DrawString(spriteFont, "Press Enter to Play Again", new Vector2((int)Game1.instance.screenBounds.Width / 2 - spriteFont.MeasureString("Press Enter to Play Again").X / 2, 260), flashColor);
            spriteBatch.DrawString(spriteFont, "Press Escape to Quit", new Vector2((int)Game1.instance.screenBounds.Width / 2 - spriteFont.MeasureString("Press Escape to Quit").X / 2, 290), Color.White);

        }
    }
}
