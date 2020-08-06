using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoSpaceShooter;
using MonoSpaceShooter.Components;
using MonoSpaceShooter.Entities;
using MonoSpaceShooter.Screens;
using MonoSpaceShooter.Systems;
using MonoSpaceShooter.Utilities;

namespace SpaceShooter
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
       
        enum gameState { Loading, StartMenu, Running, Paused, GameOver };

        #region Variables

        gameState state;

        public static Game1 instance;

        public QuadTree<QuadStorable> quadTree;

        GraphicsDeviceManager graphics;
        public Rectangle screenBounds;

        SpriteBatch spriteBatch;
        SpriteFont scoreFont;

        public Player player;

        //To prevent holding down a button from changing state rapidly
        double stateChangeDelay = 100;
        double timeSinceStateChange = 0;

        bool flashing = false;
        double timeSinceLastFlash = 0;
        double flashInterval = 500;

        public int kills = 0;
        public int playerScore = 0;

        #region Textures

        public Texture2D playerShield;
        Texture2D playerLivesGraphic;

        Texture2D background;
        List<Texture2D> backgroundElements;

        Texture2D blank;

        public Texture2D enemyShip;
        
        public Texture2D laserRed;
        public Texture2D laserGreen;
        
        public Texture2D meteorBig;
        public Texture2D meteorSmall;

        //Explosions for laser-meteor collisions
        Texture2D explosionTexture;
        Texture2D explosionTextureGreen;

        Song backgroundMusic;

        public World world;
        Stack<BaseScreen> screens;

        #endregion

        public List<BackgroundElement> backgroundObjects;

        #endregion

        #region Fields

        public bool CanChangeState
        {
            get
            {
                if (timeSinceStateChange > stateChangeDelay)
                {
                    timeSinceStateChange = 0;
                    return true;
                }
                return false;
            }
        }


        #endregion


        public Game1()
        {
            instance = this;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            screenBounds = new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            backgroundElements = new List<Texture2D>();
            backgroundObjects = new List<BackgroundElement>();
            screens = new Stack<BaseScreen>();
            state = gameState.Loading;
        }

        public void PushScreen(BaseScreen screen)
        {
            screens.Push(screen);
        }

        public void PopScreen()
        {
            screens.Pop();
        }

        protected override void Initialize()
        {
            world = new World(screenBounds);
            world.AddSystem(new RenderSystem(world));
            world.AddSystem(new MovementSystem(world));
            world.AddSystem(new DamageSystem(world));
            quadTree = new QuadTree<QuadStorable>(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            //Spritefont for scores & notifications
            scoreFont = Content.Load<SpriteFont>("score");
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Purple background
            background = Content.Load<Texture2D>("backgroundColor");
            backgroundElements.Add(Content.Load<Texture2D>("speedLine"));
            backgroundElements.Add(Content.Load<Texture2D>("starBig"));
            backgroundElements.Add(Content.Load<Texture2D>("starSmall"));
            blank = Content.Load<Texture2D>("blank");

            enemyShip = Content.Load<Texture2D>("enemyShip");

            backgroundMusic = Content.Load<Song>("loop-transit");

            //Ship textures
            List<Texture2D> shipTextures = new List<Texture2D>();
            shipTextures.Add(Content.Load<Texture2D>("player"));
            shipTextures.Add(Content.Load<Texture2D>("playerleft"));
            shipTextures.Add(Content.Load<Texture2D>("playerright"));
            playerLivesGraphic = Content.Load<Texture2D>("life");
            playerShield = Content.Load<Texture2D>("shield");

            //Lasers
            laserRed = Content.Load<Texture2D>("laserRed");
            laserGreen = Content.Load<Texture2D>("laserGreen");

            //Meteors
            meteorBig = Content.Load<Texture2D>("meteorBig");
            meteorSmall = Content.Load<Texture2D>("meteorSmall");

            //Explosions
            explosionTexture = Content.Load<Texture2D>("laserRedShot");
            explosionTextureGreen = Content.Load<Texture2D>("laserGreenShot");

            player = new Player(shipTextures, screenBounds);
            
            PrepareLevel();

            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = .0f;

            MediaPlayer.Play(backgroundMusic);

            state = gameState.StartMenu;
        }

        protected override void UnloadContent()
        {
        }

        private void PrepareLevel()
        {
            quadTree.Clear();

            player.Reset();

            kills = 0;
            playerScore = 0;

            world.Clear();

            //Initialize random meteors
            Random rand = new Random();
            int randomAmt = rand.Next(100, 300);
            for (int i = 0; i < randomAmt; i++)
            {
                bool bigMeteor = (rand.Next() % 2 == 0) ? true : false;
                //float speed = !bigMeteor ? rand.Next(2, 8) : rand.Next(1, 4);
                //Meteor newmeteor = new Meteor(bigMeteor, speed, new Vector2(rand.Next(0, screenBounds.Width), rand.Next(-10000, 0)));
                //quadTree.Add(newmeteor);
                Entity newMeteor = new Entity();
                newMeteor.AddComponent(new MeteorComponent(bigMeteor));
                newMeteor.AddComponent(new RenderComponent(bigMeteor ? meteorBig : meteorSmall));
                newMeteor.AddComponent(new TakesDamageComponent(bigMeteor ? 50 : 20));
                //newMeteor.AddComponent(new DealsDamageComponent(bigMeteor ? 50 : 20));
                newMeteor.AddComponent(new SpeedComponent(new Vector2(0, rand.Next(1, 3))));
                newMeteor.AddComponent(new PositionComponent(new Vector2(rand.Next(0, screenBounds.Width), rand.Next(-10000, 0))));
                world.AddEntity(newMeteor);
            }

            int randomEnemies = rand.Next(30, 100);
            for (int i = 0; i < randomEnemies; i++)
            {
                Enemy newEnemy = new Enemy(enemyShip, new Vector2(rand.Next(0, screenBounds.Width), rand.Next(-10000, 0)), rand.Next(2, 4) * 1000, rand.Next(1, 10) / 3 * 100);
                quadTree.Add(newEnemy);
            }
        }

        protected override void Update(GameTime gameTime)
        {
            timeSinceStateChange += gameTime.ElapsedGameTime.Milliseconds;
            //Game level keybindings
            KeyboardState keyboardState = Keyboard.GetState();

            if (state != gameState.Paused)
            {
                UpdateBackground(gameTime);
            }

            switch(state)
            {
                case gameState.Loading:
                    {
                        LoadingUpdate();
                        break;
                    }
                case gameState.Paused:
                    {
                        PausedUpdate(keyboardState);
                        break;
                    }
                case gameState.GameOver:
                    {
                        GameOverUpdate(gameTime, keyboardState);
                        break;
                    }
                case gameState.StartMenu:
                    {
                        StartMenuUpdate(gameTime, keyboardState);
                        break;
                    }
                case gameState.Running: 
                    {
                        RunningUpdate(gameTime, keyboardState);
                        break;
                    }
                default:
                    break;
            }
            base.Update(gameTime);
        }

        private void EditorUpdate()
        {
            throw new NotImplementedException();
        }

        private void LoadingUpdate()
        {
            return;
        }

        private void RunningUpdate(GameTime gameTime, KeyboardState keyboardState)
        {
            if (keyboardState.IsKeyDown(Keys.M))
            {
                MediaPlayer.Volume = 0.0f;
            }

            world.Update(gameTime);

            if (CanChangeState)
            {
                if (keyboardState.IsKeyDown(Keys.Escape))
                {
                    MediaPlayer.Pause();
                    state = gameState.Paused;
                    return;
                }
            }
            if (player.lives < 0)
            {
                state = gameState.GameOver;
                return;
            }

            //Update player position, check keypresses
            player.Update(gameTime);

            List<QuadStorable> itemsToRemove = new List<QuadStorable>();

            foreach (QuadStorable item in quadTree.GetAllObjects())
            {
                if (item is Meteor meteor)
                {
                    meteor.Update(gameTime);
                    if (!meteor.visible)
                    {
                        itemsToRemove.Add(meteor);
                    }
                    if (meteor.Moved)
                    {
                        quadTree.Move(meteor);
                    }
                }
                if (item is Enemy enemy)
                {
                    enemy.Update(gameTime);
                    if (!enemy.visible)
                    {
                        itemsToRemove.Add(enemy);
                    }
                    quadTree.Move(enemy);
                }
                if (item is Laser laser)
                {
                    laser.Update(gameTime);

                    //Get meteors near this laser
                    List<QuadStorable> meteorsNearThisLaser = quadTree.GetObjects<Meteor>(laser.Bounds);
                    foreach (Meteor nearLaser in meteorsNearThisLaser)
                    {
                        if (laser.visible && laser is EnemyLaser && nearLaser.visible && laser.Bounds.Intersects(nearLaser.Bounds))
                        {
                            laser.visible = false;
                        }

                        if (laser.visible && nearLaser.visible && !(laser is EnemyLaser) && laser.Bounds.Intersects(nearLaser.Bounds))
                        {
                            laser.visible = false;
                            nearLaser.Damage(laser.Damage);
                            Texture2D expTexToUse = player.laserLevel == 0 ? explosionTexture : explosionTextureGreen;
                            Entity explosion = new Entity();
                            explosion.AddComponent(new RenderComponent(expTexToUse));
                            explosion.AddComponent(new PositionComponent(laser.position));
                            explosion.AddComponent(new ExplosionComponent());
                            world.AddEntity(explosion);
                        }
                    }

                    //Get enemies near the laser
                    List<QuadStorable> enemiesNearThisLaser = quadTree.GetObjects<Enemy>(laser.Bounds);
                    foreach (Enemy nearEnemy in enemiesNearThisLaser)
                    {
                        if (laser.visible && nearEnemy.visible && !(laser is EnemyLaser) && laser.Bounds.Intersects(nearEnemy.Bounds))
                        {
                            laser.visible = false;
                            nearEnemy.Damage(laser.Damage);
                            Texture2D expTexToUse = player.laserLevel == 0 ? explosionTexture : explosionTextureGreen;
                            Entity explosion = new Entity();
                            explosion.AddComponent(new RenderComponent(expTexToUse));
                            explosion.AddComponent(new PositionComponent(laser.position));
                            explosion.AddComponent(new ExplosionComponent());
                            world.AddEntity(explosion);
                        }
                    }

                    if (!laser.visible)
                    {
                        itemsToRemove.Add(laser);
                    }


                    quadTree.Move(laser);
                }
            }

            foreach (QuadStorable item in itemsToRemove)
            {
                quadTree.Remove(item);
            }

            itemsToRemove.Clear();

            //Get list of enemies near the player
            List<QuadStorable> nearbyEnemies = quadTree.GetObjects<Enemy>(player.Bounds);
            foreach (Enemy enemy in nearbyEnemies)
            {
                //Enemy-Player collision
                if (enemy.visible && enemy.Bounds.Intersects(player.Bounds))
                {
                    if (player.shielded)
                    {
                        enemy.Damage(50);
                    }
                    else
                    {
                        player.lives = player.lives - 1;
                        enemy.visible = false;
                        player.Respawn();
                    }
                }
            }
            //Get list of lasers near the player
            List<QuadStorable> nearbyLasers = quadTree.GetObjects<Laser>(player.Bounds);
            foreach (Laser laser in nearbyLasers)
            {
                //Laser-Player collision
                if (laser.visible && laser is EnemyLaser && laser.Bounds.Intersects(player.Bounds))
                {
                    if (player.shielded)
                    {
                        laser.visible = false;
                    }
                    else
                    {
                        player.lives = player.lives - 1;
                        laser.visible = false;
                        player.Respawn();
                    }
                }
            }
            //List of meteors near the player
            List<QuadStorable> nearbyMeteors = quadTree.GetObjects<Meteor>(player.Bounds);
            foreach (Meteor meteor in nearbyMeteors)
            {
                //Meteor-player collision
                if (meteor.visible && meteor.Bounds.Intersects(player.Bounds) && !player.invincible)
                {
                    player.lives = player.lives - 1;
                    meteor.visible = false;
                    player.Respawn();
                }

                if (meteor.visible && meteor.Bounds.Intersects(player.Bounds) && player.shielded)
                {
                    meteor.Damage(50);
                }
            }            
        }

        private void UpdateBackground(GameTime gameTime)
        {
            //Update background elements
            if (backgroundObjects.Count < 15)
            {
                backgroundObjects.Add(new BackgroundElement(backgroundElements, screenBounds));
            }

            //Update background objects
            for (int i = backgroundObjects.Count - 1; i >= 0; i--)
            {
                backgroundObjects[i].Update(gameTime);
                if (backgroundObjects[i].belowScreen)
                {
                    backgroundObjects.RemoveAt(i);
                }
            }
        }

        private void PausedUpdate(KeyboardState keyboardState)
        {
            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                if (CanChangeState)
                {
                    MediaPlayer.Resume();
                    state = gameState.Running;
                }
                return;
            }
            if (keyboardState.IsKeyDown(Keys.Enter))
            {
                if (CanChangeState)
                {
                    state = gameState.GameOver;
                    MediaPlayer.Resume();
                }
                return;
            }
        }

        private void GameOverUpdate(GameTime gameTime, KeyboardState keyboardState)
        {
            timeSinceLastFlash += gameTime.ElapsedGameTime.Milliseconds;
            if (timeSinceLastFlash > flashInterval)
            {
                flashing = !flashing;
                timeSinceLastFlash = 0;
            }

            if (CanChangeState)
            {
                if (keyboardState.IsKeyDown(Keys.Enter))
                {
                    PrepareLevel();
                    state = gameState.Running;
                    return;
                }
                if (keyboardState.IsKeyDown(Keys.Escape))
                {
                    Exit();
                }
            }
        }

        private void StartMenuUpdate(GameTime gameTime, KeyboardState keyboardState)
        {
            timeSinceLastFlash += gameTime.ElapsedGameTime.Milliseconds;
            if (timeSinceLastFlash > flashInterval)
            {
                flashing = !flashing;
                timeSinceLastFlash = 0;
            }
            if (keyboardState.IsKeyDown(Keys.Enter))
            {
                state = gameState.Running;
                return;
            }
            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                Exit();
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();

            BackgroundDraw();

            switch (state)
            {
                case gameState.GameOver:
                {
                    GameOverDraw();
                    break;
                }
                case gameState.StartMenu:
                {
                    StartMenuDraw();
                    break;
                }
                case gameState.Paused:
                {
                    RunningDraw();
                    PausedDraw();
                    break;
                }
                case gameState.Running:
                {
                    RunningDraw();
                    break;
                }
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }

        private void BackgroundDraw()
        {
            spriteBatch.Draw(background, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);

            foreach (BackgroundElement element in backgroundObjects)
            {
                element.Draw(spriteBatch);
            }
        }

        private void GameOverDraw()
        {
            spriteBatch.DrawString(scoreFont, "Game Over", new Vector2((int)screenBounds.Width / 2 - scoreFont.MeasureString("Game Over").X / 2, (int)screenBounds.Height / 4), Color.White);
            spriteBatch.DrawString(scoreFont, "Score: " + playerScore * 100, new Vector2((int)screenBounds.Width / 2 - scoreFont.MeasureString("Score: " + playerScore * 100).X / 2, (int)screenBounds.Height / 4 + scoreFont.MeasureString("Score: " + playerScore * 100).Y), Color.White);
            Color flashColor = flashing ? Color.White : Color.Yellow;
            spriteBatch.DrawString(scoreFont, "Press Enter to Play Again", new Vector2((int)screenBounds.Width / 2 - scoreFont.MeasureString("Press Enter to Play Again").X / 2, (int)screenBounds.Height / 3 * 2), flashColor);
            spriteBatch.DrawString(scoreFont, "Press Escape to Quit", new Vector2((int)screenBounds.Width / 2 - scoreFont.MeasureString("Press Escape to Quit").X / 2, (int)screenBounds.Height / 4 * 3), Color.White);

        }

        private void StartMenuDraw()
        {
            spriteBatch.DrawString(scoreFont, "Simple Space Shooter", new Vector2((int)screenBounds.Width / 2 - scoreFont.MeasureString("Simple Space Shooter").X / 2, (int)screenBounds.Height / 4), Color.White);
            Color flashColor = flashing ? Color.White : Color.Yellow;
            spriteBatch.DrawString(scoreFont, "Press Enter to Play", new Vector2((int)screenBounds.Width / 2 - scoreFont.MeasureString("Press Enter to Play").X / 2, (int)screenBounds.Height / 3 * 2), flashColor);
            spriteBatch.DrawString(scoreFont, "Press Escape to Quit", new Vector2((int)screenBounds.Width / 2 - scoreFont.MeasureString("Press Escape to Quit").X / 2, (int)screenBounds.Height / 4 * 3), Color.White);

        }

        private void PausedDraw()
        {
            spriteBatch.DrawString(scoreFont, "Paused", new Vector2((int)screenBounds.Width / 2 - scoreFont.MeasureString("Paused").X / 2, (int)screenBounds.Height / 3), Color.White);
            spriteBatch.DrawString(scoreFont, "Press Enter to End Game", new Vector2((int)screenBounds.Width / 2 - scoreFont.MeasureString("Press Enter to End Game").X / 2, (int)screenBounds.Height / 2), Color.White);
        }

        private void RunningDraw()
        {
            player.Draw(spriteBatch);

            world.Draw(spriteBatch, scoreFont);

            List<QuadStorable> drawables = quadTree.GetObjects<Drawable>(this.screenBounds);
            foreach (Drawable item in drawables)
            {
                item.Draw(spriteBatch);
            }

            for (int i = 0; i < player.lives; i++)
            {
                spriteBatch.Draw(playerLivesGraphic, new Rectangle(40 * i + 10, 10, playerLivesGraphic.Width, playerLivesGraphic.Height), Color.White);
            }

            string scoreText = "" + playerScore * 100;
            spriteBatch.DrawString(scoreFont, scoreText, new Vector2(screenBounds.Width - scoreFont.MeasureString(scoreText).X - 30, 5), Color.White);

            spriteBatch.Draw(blank, new Rectangle(8, 43, (int)player.maxShieldPower / 30 + 4, 24), Color.Black);
            spriteBatch.Draw(blank, new Rectangle(10, 45, (int)player.maxShieldPower / 30, 20), Color.White);
            spriteBatch.Draw(blank, new Rectangle(10, 45, (int)player.shieldPower / 30, 20), Color.Blue);

            if (player.shieldCooldown)
            {
                spriteBatch.Draw(blank, new Rectangle(10, 45, (int)player.shieldPower / 30, 20), Color.Red);
            }

#if DEBUG
            //quadTree.DrawQuad(spriteBatch, quadTree.RootQuad, 0);
#endif
        }
    }
}
