using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSpaceShooter.Components;
using MonoSpaceShooter.Entities;
using MonoSpaceShooter.Utilities;

namespace MonoSpaceShooter.Systems
{
    public class MovementSystem : BaseSystem
    {
        public MovementSystem(World world) : base(world)
        {
        }

        public override void Update(GameTime gameTime)
        {
            List<Entity> movables = world.GetEntities(new[] { typeof(SpeedComponent), typeof(PositionComponent) });
            foreach (Entity moveable in movables)
            {
                PositionComponent pc = (PositionComponent)moveable.components[typeof(PositionComponent)];
                SpeedComponent rc = (SpeedComponent)moveable.components[typeof(SpeedComponent)];
                pc.position = pc.position + rc.motion;
            }
        }
    }
}
