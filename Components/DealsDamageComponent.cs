using System;
namespace MonoSpaceShooter.Components
{
    public class DealsDamageComponent : BaseComponent
    {
        public int strength;
        public DealsDamageComponent(int str) : base()
        {
            strength = str;
        }
    }
}
