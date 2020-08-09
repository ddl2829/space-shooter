using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSpaceShooter.Components;
using MonoSpaceShooter.Entities;
using MonoSpaceShooter.Utilities;
using SpaceShooter;

namespace MonoSpaceShooter.Systems
{
    public class LevelSystem : BaseSystem
    {
        public static int levelNumber = 0;
        public static int levelLength;
        private bool preppingLevel = false;

        public LevelSystem(World world) : base(world)
        {
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (preppingLevel)
            {
                return;
            }
            //we know it's time to seed a new level when no meteors or enemies are left
            List<Entity> enemies = world.GetEntities(new[] { typeof(EnemyComponent) });
            List<Entity> meteors = world.GetEntities(new[] { typeof(MeteorComponent) });
            if (enemies.Count == 0 && meteors.Count == 0)
            {
                levelNumber++;
                if (levelNumber > 1)
                {
                    Entity e = new Entity();
                    e.AddComponent(new NotificationComponent("Level Complete!", 3000, true));
                    world.AddEntity(e);
                    preppingLevel = true;
                    Task.Delay(5000).ContinueWith(delegate
                    {
                        Entity e = new Entity();
                        e.AddComponent(new NotificationComponent("Begin Level " + levelNumber, 3000, true));
                        world.AddEntity(e);

                        Task.Delay(3000).ContinueWith(delegate
                        {
                            BuildLevel();
                        });
                    });
                }
                else
                {
                    Entity e = new Entity();
                    e.AddComponent(new NotificationComponent("Good Luck!", 3000, true));
                    world.AddEntity(e);
                    //Load first level
                    BuildLevel();
                }
            }
        }

        private void BuildLevel()
        {
            //#if DEBUG
            //            Entity boss = new Entity();
            //            boss.AddComponent(new EnemyComponent(10000, 0));
            //            boss.AddComponent(new RenderComponent(Game1.instance.bossTexture));
            //            boss.AddComponent(new PositionComponent(new Vector2(Game1.instance.screenBounds.Width / 2, -100)));
            //            boss.AddComponent(new SpeedComponent(new Vector2(0, 1)));
            //            boss.AddComponent(new TakesDamageComponent(10 * levelNumber, DamageSystem.LASER));
            //            boss.AddComponent(new DealsDamageComponent(20, DamageSystem.ENEMY));
            //            boss.AddComponent(new BossEnemyComponent());
            //            world.AddEntity(boss);
            //            levelLength = -100;
            //#else
            levelLength = -1000;// +  (-1000 * levelNumber);

            Random rand = new Random();

            int j = 0;
            for (int l = 0; l > levelLength; l -= 1000)
            {
                //Initialize random meteors
                int randomAmt = 20 + (10 * j);
                for (int i = 0; i < randomAmt; i++)
                {
                    bool bigMeteor = (rand.Next() % 2 == 0) ? true : false;
                    Entity newMeteor = new Entity();
                    newMeteor.AddComponent(new MeteorComponent(bigMeteor));
                    newMeteor.AddComponent(new RenderComponent(bigMeteor ? Game1.instance.meteorBig : Game1.instance.meteorSmall));
                    newMeteor.AddComponent(new TakesDamageComponent(bigMeteor ? 10 : 5, DamageSystem.LASER));
                    newMeteor.AddComponent(new DealsDamageComponent(bigMeteor ? 10 : 5, DamageSystem.METEOR));
                    int[] speeds = new[] { 1, 1, 1, 2, 2, 2, 3, 3, 4, 5 };
                    newMeteor.AddComponent(new SpeedComponent(new Vector2(0, speeds[rand.Next(0, speeds.Length)])));
                    newMeteor.AddComponent(new PositionComponent(new Vector2(rand.Next(0, Game1.instance.screenBounds.Width), rand.Next(l - 1000, Math.Min(-100, l)))));
                    world.AddEntity(newMeteor);
                }

                int numberOfEnemies = 10 + (5 * j);
                for (int i = 0; i < numberOfEnemies; i++)
                {
                    Entity enemy = new Entity();
                    enemy.AddComponent(new EnemyComponent(Math.Max(rand.Next(2000, 5000) - 100 * j, 500), -rand.NextDouble() * 10000));
                    enemy.AddComponent(new RenderComponent(Game1.instance.enemyShip));
                    enemy.AddComponent(new PositionComponent(new Vector2(rand.Next(50, Game1.instance.screenBounds.Width - 50), rand.Next(l - 1000, Math.Min(-100, l)))));
                    enemy.AddComponent(new SpeedComponent(new Vector2(0, 1)));
                    enemy.AddComponent(new TakesDamageComponent(10, DamageSystem.LASER));
                    enemy.AddComponent(new DealsDamageComponent(20, DamageSystem.ENEMY));
                    world.AddEntity(enemy);
                }
                j++;
            }

            Entity boss = new Entity();
            boss.AddComponent(new EnemyComponent(Math.Max(1000 - (100 * levelNumber - 1), 200), 0));
            boss.AddComponent(new RenderComponent(Game1.instance.bossTexture));
            boss.AddComponent(new PositionComponent(new Vector2(Game1.instance.screenBounds.Width / 2, levelLength)));
            boss.AddComponent(new SpeedComponent(new Vector2(0, 1)));
            boss.AddComponent(new TakesDamageComponent(500 * levelNumber, DamageSystem.LASER));
            boss.AddComponent(new DealsDamageComponent(20, DamageSystem.ENEMY));
            boss.AddComponent(new BossEnemyComponent());
            world.AddEntity(boss);
//#endif
            preppingLevel = false;
        }

        public override void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            spriteBatch.Draw(Game1.instance.blank, new Rectangle(Game1.instance.screenBounds.Width - 158, 25, 150, 12), Color.Black);
            List<Entity> bossEntities = world.GetEntities(new[] { typeof(BossEnemyComponent) });
            if (bossEntities.Count > 0)
            {
                Entity boss = bossEntities[0];
                PositionComponent bossPosition = (PositionComponent)boss.components[typeof(PositionComponent)];
                double pct = (levelLength - bossPosition.position.Y) / levelLength;
                spriteBatch.Draw(Game1.instance.blank, new Rectangle(Game1.instance.screenBounds.Width - 159, 26, (int)(pct * 148), 10), Color.White);
            }
        }
    }
}
