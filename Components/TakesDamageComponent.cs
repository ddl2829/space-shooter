using System;
namespace MonoSpaceShooter.Components
{
    public class TakesDamageComponent : BaseComponent
    {
        public int health;
        public TakesDamageComponent(int hp) : base()
        {
            health = hp;
        }
    }
}
