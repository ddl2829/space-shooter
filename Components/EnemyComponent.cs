using System;
namespace MonoSpaceShooter.Components
{
    public class EnemyComponent : BaseComponent
    {
        public double shotInterval;
        public double timeSinceLastShot;
        public EnemyComponent(double interval, double randomOffset) : base()
        {
            shotInterval = interval;
            timeSinceLastShot = randomOffset;
        }
    }
}
