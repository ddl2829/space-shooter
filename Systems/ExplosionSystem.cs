using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoSpaceShooter.Components;
using MonoSpaceShooter.Entities;
using MonoSpaceShooter.Utilities;

namespace MonoSpaceShooter.Systems
{
    public class ExplosionSystem : BaseSystem
    {
        public ExplosionSystem(World world) : base(world)
        {
        }

        public override void Update(GameTime gametime)
        {
            List<Entity> explosions = world.GetEntities(new[] { typeof(ExplosionComponent) });
            foreach (Entity explosion in explosions)
            {
                ExplosionComponent ec = (ExplosionComponent)explosion.components[typeof(ExplosionComponent)];
                ec.elapsedTime += gametime.ElapsedGameTime.Milliseconds;
                if (ec.elapsedTime > ec.maxLife)
                {
                    world.RemoveEntity(explosion);
                    continue;
                }
            }
        }
    }
}
