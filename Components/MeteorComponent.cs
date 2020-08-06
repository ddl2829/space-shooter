using System;
namespace MonoSpaceShooter.Components
{
    public class MeteorComponent : BaseComponent
    {
        public bool isBig;
        public MeteorComponent(bool big) : base()
        {
            isBig = big;
        }
    }
}
