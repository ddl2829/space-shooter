using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using SpaceShooter;

namespace MonoSpaceShooter.Screens
{
    public class GameScreen : BaseScreen
    {
        public static int timeStayedAlive = 0;

        public GameScreen() : base()
        {
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            timeStayedAlive += gameTime.ElapsedGameTime.Milliseconds;

            if (KeyPressed(Keys.M))
            {
                MediaPlayer.Volume = 0.0f;
            }

            Game1.instance.world.Update(gameTime);

            if (KeyPressed(Keys.Escape))
            {
                MediaPlayer.Pause();
                Game1.instance.PushScreen(new PauseScreen());
                return;
            }
        }

        public override void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            base.Draw(spriteBatch, spriteFont);
            Game1.instance.world.Draw(spriteBatch, spriteFont);
        }
    }
}
