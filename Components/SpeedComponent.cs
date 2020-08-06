using System;
using Microsoft.Xna.Framework;

namespace MonoSpaceShooter.Components
{
    public class SpeedComponent : BaseComponent
    {
        public Vector2 motion;

        public SpeedComponent(Vector2 p) : base()
        {
            motion = p;
        }

        public SpeedComponent(int x, int y) : base()
        {
            motion = new Vector2(x, y);
        }
    }
}
