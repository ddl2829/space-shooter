using System;
using Microsoft.Xna.Framework;

namespace MonoSpaceShooter.Components
{
    public class NotificationComponent : BaseComponent
    {
        static int colorSelect = 0;
        public string text;
        public int elapsedTime = 0;
        public int maxLife = 200;
        public Color color;
        public bool centerText = false;

        public NotificationComponent(string t, int lifeSpan, bool centered) : base()
        {
            centerText = centered;
            maxLife = lifeSpan;
            text = t;
            colorSelect++;
            if(colorSelect > 4 || centered)
            {
                colorSelect = 0;
            }
            switch (colorSelect)
            {
                case 0:
                    color = Color.White;
                    break;
                case 1:
                    color = Color.Blue;
                    break;
                case 2:
                    color = Color.Green;
                    break;
                case 3:
                    color = Color.Red;
                    break;
                case 4:
                    color = Color.Purple;
                    break;
            }
        }
    }
}
