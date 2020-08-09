using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoSpaceShooter.Components;
using MonoSpaceShooter.Entities;
using MonoSpaceShooter.Screens;
using MonoSpaceShooter.Utilities;
using SpaceShooter;

namespace MonoSpaceShooter.Systems
{
    public class InputSystem : BaseSystem
    {
        KeyboardState keyboardState;

        public InputSystem(World world) : base(world)
        {
        }

        public override void Update(GameTime gameTime)
        {
            keyboardState = Keyboard.GetState();

            Entity player;
            List<Entity> playerEntities = world.GetEntities(new[] { typeof(PlayerComponent) });
            if (playerEntities.Count == 0)
            {
                player = new Entity();
                PlayerComponent ppc = new PlayerComponent();
                player.AddComponent(ppc);
                player.AddComponent(new HasShieldComponent());
                player.AddComponent(new SpeedComponent(Vector2.Zero));
                player.AddComponent(new TakesDamageComponent(50, DamageSystem.ENEMY ^ DamageSystem.ENEMY_LASER ^ DamageSystem.METEOR));
                world.AddEntity(player);

                ppc.lives = ppc.maxLives;
            }
            else
            {
                player = playerEntities[0];
            }

            if(!player.HasComponent(typeof(RenderComponent)))
            {
                Game1.instance.kills = 0;

                PlayerComponent ppc = (PlayerComponent)player.components[typeof(PlayerComponent)];

                ppc.timeSinceRespawn = 0;
                ppc.laserLevel = 0;

                //Remove shield upgrade on respawn
                //player.RemoveComponent(typeof(HasShieldComponent));

                TakesDamageComponent ptdc = (TakesDamageComponent)player.components[typeof(TakesDamageComponent)];
                ptdc.health = ptdc.maxHealth;

                RenderComponent prc = new RenderComponent(Game1.instance.shipTextures.ToArray());
                player.AddComponent(prc);
                if (!player.HasComponent(typeof(PositionComponent)))
                {
                    player.AddComponent(new PositionComponent(new Vector2(
                        (world.screenRect.Width / 2) - (prc.CurrentTexture.Width / 2),
                        (world.screenRect.Height / 3) * 2 + (prc.CurrentTexture.Height / 2)
                    )));
                }
                else
                {
                    PositionComponent playerPositionComp = (PositionComponent)player.components[typeof(PositionComponent)];
                    playerPositionComp.position = new Vector2(
                        (world.screenRect.Width / 2) - (prc.CurrentTexture.Width / 2),
                        (world.screenRect.Height / 3) * 2 + (prc.CurrentTexture.Height / 2)
                    );
                }
            }

            PlayerComponent pc = (PlayerComponent)player.components[typeof(PlayerComponent)];
            SpeedComponent sc = (SpeedComponent)player.components[typeof(SpeedComponent)];
            RenderComponent rc = (RenderComponent)player.components[typeof(RenderComponent)];

            if(pc.lives == 0)
            {
                Game1.instance.PopScreen();
                Game1.instance.PushScreen(new GameOverScreen());
                return;
            }

            sc.motion = Vector2.Zero;
            pc.lastFireTime += gameTime.ElapsedGameTime.Milliseconds;

            if (player.HasComponent(typeof(HasShieldComponent)))
            {
                HasShieldComponent hasShieldComp = (HasShieldComponent)player.components[typeof(HasShieldComponent)];

                if (!player.HasComponent(typeof(ShieldedComponent)) && hasShieldComp.shieldPower < hasShieldComp.maxShieldPower)
                {
                    hasShieldComp.shieldPower += hasShieldComp.shieldRegenRate * gameTime.ElapsedGameTime.Milliseconds;
                }

                if (hasShieldComp.shieldPower >= hasShieldComp.maxShieldPower)
                {
                    hasShieldComp.shieldPower = hasShieldComp.maxShieldPower;
                    hasShieldComp.shieldCooldown = false;
                }
                if (player.HasComponent(typeof(ShieldedComponent)))
                {
                    hasShieldComp.shieldPower -= hasShieldComp.shieldDepleteRate * gameTime.ElapsedGameTime.Milliseconds;

                    if (hasShieldComp.shieldPower <= 0)
                    {
                        player.RemoveComponent(typeof(ShieldedComponent));
                        hasShieldComp.shieldCooldown = true;
                        hasShieldComp.shieldPower = 0;
                    }
                }

                if (!hasShieldComp.shieldCooldown)
                {
                    if (keyboardState.IsKeyDown(Keys.LeftShift) && hasShieldComp.shieldPower >= 0)
                    {
                        if (!player.HasComponent(typeof(ShieldedComponent)))
                        {
                            player.AddComponent(new ShieldedComponent());
                        }
                    }
                    else
                    {
                        player.RemoveComponent(typeof(ShieldedComponent));
                    }
                }
            }

            bool upgradedLasers = false;
            if (Game1.instance.kills > 10 && pc.laserLevel == 0)
            {
                pc.laserLevel = 1;
                upgradedLasers = true;
            }
            if (Game1.instance.kills > 20 && pc.laserLevel == 1)
            {
                pc.laserLevel = 2;
                upgradedLasers = true;
            }
            if (Game1.instance.kills > 40 && pc.laserLevel == 2)
            {
                pc.laserLevel = 3;
                upgradedLasers = true;
            }

            if (upgradedLasers)
            {
                Entity e = new Entity();
                e.AddComponent(new NotificationComponent("Lasers Improved", 2000, true));
                world.AddEntity(e);
            }

            if (keyboardState.IsKeyDown(Keys.Space) && !player.HasComponent(typeof(ShieldedComponent)))
            {
                Shoot(player);
            }

            if (keyboardState.IsKeyDown(Keys.Left))
            {
                rc.currentTexture = 1;
                sc.motion.X = -1;
            }
            if (keyboardState.IsKeyDown(Keys.Right))
            {
                rc.currentTexture = 2;
                sc.motion.X = 1;
            }
            if (keyboardState.IsKeyDown(Keys.Up))
            {
                if (keyboardState.IsKeyUp(Keys.Left) && keyboardState.IsKeyUp(Keys.Right))
                {
                    rc.currentTexture = 0;
                }
                sc.motion.Y = -1;
            }
            if (keyboardState.IsKeyDown(Keys.Down))
            {
                if (keyboardState.IsKeyUp(Keys.Left) && keyboardState.IsKeyUp(Keys.Right))
                {
                    rc.currentTexture = 0;
                }
                sc.motion.Y = 1;
            }
            if (keyboardState.IsKeyUp(Keys.Left) && keyboardState.IsKeyUp(Keys.Right))
            {
                rc.currentTexture = 0;
            }

            sc.motion *= 5;
        }

        private void Shoot(Entity player)
        {
            PlayerComponent pc = (PlayerComponent)player.components[typeof(PlayerComponent)];
            PositionComponent posc = (PositionComponent)player.components[typeof(PositionComponent)];

            if (pc.lastFireTime > 50)
            {
                Texture2D laser = Game1.instance.laserRed;
                if (pc.laserLevel >= 1)
                {
                    laser = Game1.instance.laserGreen;
                }

                Texture2D expTexToUse = pc.laserLevel == 0 ? Game1.instance.explosionTexture : Game1.instance.explosionTextureGreen;

                if (pc.laserLevel < 2)
                {
                    Entity newLaser = new Entity();
                    newLaser.AddComponent(new RenderComponent(laser));
                    newLaser.AddComponent(new LaserComponent(expTexToUse));
                    newLaser.AddComponent(new SpeedComponent(new Vector2(0, -20)));
                    newLaser.AddComponent(new PositionComponent(new Vector2(posc.position.X, posc.position.Y - laser.Height / 2 - 40)));
                    newLaser.AddComponent(new DealsDamageComponent(pc.laserLevel + 1, DamageSystem.LASER));
                    Game1.instance.world.AddEntity(newLaser);
                }
                if (pc.laserLevel >= 2)
                {
                    Entity newLaser1 = new Entity();
                    newLaser1.AddComponent(new RenderComponent(laser));
                    newLaser1.AddComponent(new LaserComponent(expTexToUse));
                    newLaser1.AddComponent(new SpeedComponent(new Vector2(0, -20)));
                    newLaser1.AddComponent(new PositionComponent(new Vector2(posc.position.X - 10, posc.position.Y - laser.Height / 2 - 40)));
                    newLaser1.AddComponent(new DealsDamageComponent(pc.laserLevel + 1, DamageSystem.LASER));
                    Game1.instance.world.AddEntity(newLaser1);

                    Entity newLaser2 = new Entity();
                    newLaser2.AddComponent(new RenderComponent(laser));
                    newLaser2.AddComponent(new LaserComponent(expTexToUse));
                    newLaser2.AddComponent(new SpeedComponent(new Vector2(0, -20)));
                    newLaser2.AddComponent(new PositionComponent(new Vector2(posc.position.X + 10, posc.position.Y - laser.Height / 2 - 40)));
                    newLaser2.AddComponent(new DealsDamageComponent(pc.laserLevel + 1, DamageSystem.LASER));
                    Game1.instance.world.AddEntity(newLaser2);
                }
                if (pc.laserLevel >= 3)
                {
                    Entity newLaser1 = new Entity();
                    newLaser1.AddComponent(new RenderComponent(laser));
                    newLaser1.AddComponent(new LaserComponent(expTexToUse));
                    newLaser1.AddComponent(new SpeedComponent(new Vector2(-10, -20)));
                    newLaser1.AddComponent(new PositionComponent(new Vector2(posc.position.X - 10, posc.position.Y - laser.Height / 2 - 40)));
                    newLaser1.AddComponent(new DealsDamageComponent(pc.laserLevel + 1, DamageSystem.LASER));
                    Game1.instance.world.AddEntity(newLaser1);

                    Entity newLaser2 = new Entity();
                    newLaser2.AddComponent(new RenderComponent(laser));
                    newLaser2.AddComponent(new LaserComponent(expTexToUse));
                    newLaser2.AddComponent(new SpeedComponent(new Vector2(10, -20)));
                    newLaser2.AddComponent(new PositionComponent(new Vector2(posc.position.X + 10, posc.position.Y - laser.Height / 2 - 40)));
                    newLaser2.AddComponent(new DealsDamageComponent(pc.laserLevel + 1, DamageSystem.LASER));
                    Game1.instance.world.AddEntity(newLaser2);
                }
                pc.lastFireTime = 0;
            }
        }
    }
}
