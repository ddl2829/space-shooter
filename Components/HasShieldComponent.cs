using System;
namespace MonoSpaceShooter.Components
{
    public class HasShieldComponent : BaseComponent
    {
        public bool shieldCooldown = false;
        public double shieldPower = 3000;
        public double maxShieldPower = 3000;
        public double shieldRegenRate = 0.3f;
        public double shieldDepleteRate = 1.0f;

        public HasShieldComponent() : base()
        {
        }
    }
}
