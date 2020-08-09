using System;
namespace MonoSpaceShooter.Components
{
    public class TakesDamageComponent : BaseComponent
    {
        public int health;
        public int takesDamageFromMask;
        public int maxHealth;
        public TakesDamageComponent(int hp, int mask) : base()
        {
            health = hp;
            maxHealth = hp;
            takesDamageFromMask = mask;
        }
    }
}
