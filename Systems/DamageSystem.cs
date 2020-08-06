using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoSpaceShooter.Components;
using MonoSpaceShooter.Entities;
using MonoSpaceShooter.Utilities;
using SpaceShooter;

namespace MonoSpaceShooter.Systems
{
    public class DamageSystem : BaseSystem
    {
        public DamageSystem(World world) : base(world)
        {
        }

        public override void Update(GameTime gameTime)
        {
            List<Entity> thingsThatDoDamage = world.GetEntities(new[] { typeof(DealsDamageComponent), typeof(PositionComponent), typeof(RenderComponent) });
            List<Entity> thingsThatTakeDamage = world.GetEntities(new[] { typeof(TakesDamageComponent), typeof(PositionComponent), typeof(RenderComponent) });
            foreach(Entity damageDealer in thingsThatDoDamage)
            {
                DealsDamageComponent ddc = (DealsDamageComponent)damageDealer.components[typeof(DealsDamageComponent)];
                PositionComponent dd_pc = (PositionComponent)damageDealer.components[typeof(PositionComponent)];
                RenderComponent dd_rc = (RenderComponent)damageDealer.components[typeof(RenderComponent)];
                Rectangle damageDealerRect = new Rectangle((int)dd_pc.position.X, (int)dd_pc.position.Y, dd_rc.texture.Width, dd_rc.texture.Height);
                foreach (Entity damageTaker in thingsThatTakeDamage)
                {
                    TakesDamageComponent tdc = (TakesDamageComponent)damageTaker.components[typeof(TakesDamageComponent)];
                    PositionComponent td_pc = (PositionComponent)damageTaker.components[typeof(PositionComponent)];
                    RenderComponent td_rc = (RenderComponent)damageTaker.components[typeof(RenderComponent)];
                    Rectangle damageTakerRect = new Rectangle((int)td_pc.position.X, (int)td_pc.position.Y, td_rc.texture.Width, td_rc.texture.Height);
                    if(damageDealerRect.Intersects(damageTakerRect))
                    {
                        tdc.health -= ddc.strength;
                        if(tdc.health <= 0)
                        {
                            world.RemoveEntity(damageTaker);
                            if(damageTaker.HasComponent(typeof(MeteorComponent)))
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
                                        newMeteor.AddComponent(new TakesDamageComponent(20));
                                        newMeteor.AddComponent(new RenderComponent(Game1.instance.meteorSmall));
                                        newMeteor.AddComponent(new SpeedComponent(new Vector2(rand.Next(-3, 3), rand.Next(0, 2))));
                                        newMeteor.AddComponent(new PositionComponent(new Vector2(td_pc.position.X, td_pc.position.Y)));
                                        world.AddEntity(newMeteor);
                                    }
                                }
                            }
                        }
                        world.RemoveEntity(damageDealer);
                        break;
                    }
                }
            }

        }
    }
}
