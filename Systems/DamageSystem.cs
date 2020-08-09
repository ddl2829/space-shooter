using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoSpaceShooter.Components;
using MonoSpaceShooter.Entities;
using MonoSpaceShooter.Screens;
using MonoSpaceShooter.Utilities;
using SpaceShooter;

namespace MonoSpaceShooter.Systems
{
    public class DamageSystem : BaseSystem
    {
        public static int PLAYER = 1;
        public static int METEOR = 2;
        public static int ENEMY = 4;
        public static int LASER = 8;
        public static int ENEMY_LASER = 16;

        public DamageSystem(World world) : base(world)
        {
        }

        public override void Update(GameTime gameTime)
        {
            List<Entity> thingsThatDoDamage = world.GetEntities(new[] { typeof(DealsDamageComponent), typeof(PositionComponent), typeof(RenderComponent) });
            List<Entity> thingsThatTakeDamage = world.GetEntities(new[] { typeof(TakesDamageComponent), typeof(PositionComponent), typeof(RenderComponent) });

            foreach (Entity damageDealer in thingsThatDoDamage)
            {
                DealsDamageComponent ddc = (DealsDamageComponent)damageDealer.components[typeof(DealsDamageComponent)];
                PositionComponent dd_pc = (PositionComponent)damageDealer.components[typeof(PositionComponent)];
                RenderComponent dd_rc = (RenderComponent)damageDealer.components[typeof(RenderComponent)];

                Rectangle damageDealerRect = new Rectangle((int)dd_pc.position.X - (dd_rc.CurrentTexture.Width / 2), (int)dd_pc.position.Y - (dd_rc.CurrentTexture.Height / 2), dd_rc.CurrentTexture.Width, dd_rc.CurrentTexture.Height);

                foreach (Entity damageTaker in thingsThatTakeDamage)
                {
                    TakesDamageComponent tdc = (TakesDamageComponent)damageTaker.components[typeof(TakesDamageComponent)];

                    //Bitwise AND the collision masks, a value > 0 means we have objects that can be compared
                    if((tdc.takesDamageFromMask & ddc.damageTypeMask) == 0)
                    {
                        continue;
                    }

                    PositionComponent td_pc = (PositionComponent)damageTaker.components[typeof(PositionComponent)];
                    if(!damageTaker.HasComponent(typeof(RenderComponent)))
                    {
                        continue;
                    }
                    RenderComponent td_rc = (RenderComponent)damageTaker.components[typeof(RenderComponent)];
                    Rectangle damageTakerRect = new Rectangle((int)td_pc.position.X - (td_rc.CurrentTexture.Width / 2), (int)td_pc.position.Y - (td_rc.CurrentTexture.Height / 2), td_rc.CurrentTexture.Width, td_rc.CurrentTexture.Height);
                    if(damageDealerRect.Intersects(damageTakerRect))
                    {
                        if (!damageTaker.HasComponent(typeof(ShieldedComponent)))
                        {
                            tdc.health -= ddc.strength;
                        }

                        //Check if the damage dealer was a laser
                        if(damageDealer.HasComponent(typeof(LaserComponent)))
                        {
                            LaserComponent dd_lc = (LaserComponent)damageDealer.components[typeof(LaserComponent)];

                            //Spawn an explosion at the location of collision
                            Entity explosion = new Entity();
                            explosion.AddComponent(new RenderComponent(dd_lc.explosionTexture));
                            explosion.AddComponent(new PositionComponent(dd_pc.position));
                            explosion.AddComponent(new ExplosionComponent());
                            world.AddEntity(explosion);
                        }

                        world.RemoveEntity(damageDealer);

                        if (tdc.health <= 0)
                        {
                            if(damageTaker.HasComponent(typeof(PlayerComponent)))
                            {
                                PlayerComponent playerComp = (PlayerComponent)damageTaker.components[typeof(PlayerComponent)];
                                playerComp.lives -= 1;
                                damageTaker.RemoveComponent(typeof(RenderComponent));
                            } else
                            {
                                world.RemoveEntity(damageTaker);
                            }

                            if (damageDealer.HasComponent(typeof(LaserComponent)))
                            {
                                int credit = 1;
                                if (damageTaker.HasComponent(typeof(MeteorComponent)))
                                {
                                    MeteorComponent meteorComponent = (MeteorComponent)damageTaker.components[typeof(MeteorComponent)];
                                    if(meteorComponent.isBig)
                                    {
                                        credit = 2;
                                    }
                                }

                                Game1.instance.kills += 1;
                                double multiplier = 1 + Math.Log(GameScreen.timeStayedAlive / 1000);
                                double score = (credit * multiplier) * 100;
                                Game1.instance.playerScore += score;

                                Entity e = new Entity();
                                e.AddComponent(new PositionComponent(td_pc.position));
                                e.AddComponent(new NotificationComponent("+" + Math.Truncate(score), 200, false));
                                Game1.instance.world.AddEntity(e);
                            }

                            if (damageTaker.HasComponent(typeof(MeteorComponent)))
                            {
                                MeteorComponent meteorComponent = (MeteorComponent)damageTaker.components[typeof(MeteorComponent)];
                                //Spawn small meteors when big ones break
                                if (meteorComponent.isBig)
                                {
                                    Random rand = new Random();
                                    int randAmt = rand.Next(2, 6);
                                    for (int i = 0; i < randAmt; i++)
                                    {
                                        Entity newMeteor = new Entity();
                                        newMeteor.AddComponent(new MeteorComponent(false));
                                        newMeteor.AddComponent(new TakesDamageComponent(5, LASER));
                                        newMeteor.AddComponent(new DealsDamageComponent(5, METEOR));
                                        newMeteor.AddComponent(new RenderComponent(Game1.instance.meteorSmall));
                                        newMeteor.AddComponent(new SpeedComponent(new Vector2(rand.Next(-3, 3), rand.Next(2, 5))));
                                        newMeteor.AddComponent(new PositionComponent(new Vector2(td_pc.position.X, td_pc.position.Y)));
                                        world.AddEntity(newMeteor);
                                    }
                                }
                            }
                        }
                        
                        break;
                    }
                }
            }

        }
    }
}
