using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoSpaceShooter.Components;
using MonoSpaceShooter.Entities;
using MonoSpaceShooter.Utilities;
using SpaceShooter;

namespace MonoSpaceShooter.Systems
{
    public class EnemyLogicSystem : BaseSystem
    {
        public EnemyLogicSystem(World world) : base(world)
        {
        }

        public override void Update(GameTime gameTime)
        {
            List<Entity> players = world.GetEntities(new[] { typeof(PlayerComponent) });
            List<Entity> enemies = world.GetEntities(new[] { typeof(EnemyComponent), typeof(PositionComponent), typeof(SpeedComponent) });
            foreach(Entity enemy in enemies)
            {
                EnemyComponent ec = (EnemyComponent)enemy.components[typeof(EnemyComponent)];
                PositionComponent pc = (PositionComponent)enemy.components[typeof(PositionComponent)];
                SpeedComponent sc = (SpeedComponent)enemy.components[typeof(SpeedComponent)];

                sc.motion.Y = 1;
                if(players.Count == 0)
                {
                    sc.motion.X = 0;
                    continue;
                }

                PositionComponent playerPosition = (PositionComponent)players[0].components[typeof(PositionComponent)];

                if(pc.position.Y > playerPosition.position.Y)
                {
                    sc.motion.X = 0;
                    continue;
                }

                ec.timeSinceLastShot += gameTime.ElapsedGameTime.Milliseconds;
                if (ec.timeSinceLastShot >= ec.shotInterval)
                {
                    ec.timeSinceLastShot = 0;

                    Entity newLaser = new Entity();
                    newLaser.AddComponent(new RenderComponent(Game1.instance.laserGreen));
                    newLaser.AddComponent(new LaserComponent(Game1.instance.explosionTextureGreen));
                    newLaser.AddComponent(new SpeedComponent(new Vector2(0, 20)));
                    newLaser.AddComponent(new PositionComponent(new Vector2(pc.position.X - Game1.instance.laserGreen.Width / 2, pc.position.Y + 30)));
                    newLaser.AddComponent(new DealsDamageComponent(10, DamageSystem.ENEMY_LASER));
                    Game1.instance.world.AddEntity(newLaser);
                }

                float movement = pc.position.X - playerPosition.position.X;
                if (movement > 0)
                {
                    sc.motion.X = -.5f;
                }
                else
                {
                    sc.motion.X = .5f;
                }
            }
        }
    }
}
