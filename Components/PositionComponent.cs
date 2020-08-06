using System;
using Microsoft.Xna.Framework;

namespace MonoSpaceShooter.Components
{
    public class PositionComponent : BaseComponent
    {
        public Vector2 position;

        public PositionComponent(Vector2 p) : base()
        {
            position = p;
        }

        public PositionComponent(int x, int y) : base()
        {
            position = new Vector2(x, y);
        }
    }
}
