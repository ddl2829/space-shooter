using System;
namespace MonoSpaceShooter.Components
{
    public class DealsDamageComponent : BaseComponent
    {
        public int strength;
        public int damageTypeMask;
        public DealsDamageComponent(int str, int mask) : base()
        {
            strength = str;
            damageTypeMask = mask;
        }
    }
}
