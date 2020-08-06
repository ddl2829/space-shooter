using System;
namespace MonoSpaceShooter.Components
{
    public class ExplosionComponent : BaseComponent
    {
        public int elapsedTime = 0;
        public int maxLife = 200;

        public ExplosionComponent() : base()
        {
        }
    }
}
