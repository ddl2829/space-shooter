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

                if (moveable.HasComponent(typeof(PlayerComponent)))
                {
                    RenderComponent renderComp = (RenderComponent)moveable.components[typeof(RenderComponent)];
                    if (pc.position.X < 0)
                    {
                        pc.position.X = 0;
                    }
                    if (pc.position.Y < 0)
                    {
                        pc.position.Y = 0;
                    }
                    if (pc.position.X > world.screenRect.Width - renderComp.CurrentTexture.Width)
                    {
                        pc.position.X = world.screenRect.Width - renderComp.CurrentTexture.Width;
                    }
                    if (pc.position.Y > world.screenRect.Height - renderComp.CurrentTexture.Height)
                    {
                        pc.position.Y = world.screenRect.Height - renderComp.CurrentTexture.Height;
                    }
                }

                if (moveable.HasComponent(typeof(LaserComponent)))
                {
                    //Despawn lasers shortly after they leave the screen
                    if(pc.position.Y < 0 || pc.position.Y > world.screenRect.Height || pc.position.X < 0 || pc.position.X > world.screenRect.Width)
                    {
                        world.RemoveEntity(moveable);
                    }
                }

                //Despawn anything going off the bottom of the screen
                if(pc.position.Y > world.screenRect.Height)
                {
                    world.RemoveEntity(moveable);
                }
            }
        }
    }
}
