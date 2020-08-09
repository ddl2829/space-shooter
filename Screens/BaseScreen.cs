using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoSpaceShooter.Screens
{
    public class BaseScreen
    {
        public bool pausesBelow = false;
        protected KeyboardState previousKeyboardState = Keyboard.GetState();
        protected KeyboardState keyboardState = Keyboard.GetState();

        public BaseScreen()
        {
        }

        public virtual void Update(GameTime gameTime)
        {
            previousKeyboardState = keyboardState;
            keyboardState = Keyboard.GetState();
        }

        public virtual void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont)
        {

        }

        public bool KeyPressed(Keys key)
        {
            return previousKeyboardState.IsKeyUp(key) && keyboardState.IsKeyDown(key);
        }
    }
}
