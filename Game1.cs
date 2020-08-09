using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MonoSpaceShooter.Components;
using MonoSpaceShooter.Entities;
using MonoSpaceShooter.Screens;
using MonoSpaceShooter.Systems;
using MonoSpaceShooter.Utilities;

namespace SpaceShooter
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        public static Game1 instance;

        GraphicsDeviceManager graphics;
        public Rectangle screenBounds;

        SpriteBatch spriteBatch;
        SpriteFont scoreFont;

        public int kills = 0;
        public double playerScore = 0;

        public Texture2D playerShield;
        public Texture2D playerLivesGraphic;

        public Texture2D background;
        public List<Texture2D> backgroundElements;

        public Texture2D blank;

        public Texture2D enemyShip;
        public Texture2D bossTexture;

        public Texture2D laserRed;
        public Texture2D laserGreen;

        public List<Texture2D> shipTextures;

        public Texture2D meteorBig;
        public Texture2D meteorSmall;

        //Explosions for laser-meteor collisions
        public Texture2D explosionTexture;
        public Texture2D explosionTextureGreen;

        Song backgroundMusic;

        public World world;
        List<BaseScreen> screens;


        public Game1()
        {
            instance = this;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            screenBounds = new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            backgroundElements = new List<Texture2D>();
            
            screens = new List<BaseScreen>();
        }

        public void PushScreen(BaseScreen screen)
        {
            screens.Add(screen);
        }

        public void PopScreen()
        {
            screens.RemoveAt(screens.Count - 1);
        }

        protected override void Initialize()
        {
            world = new World(screenBounds);
            world.AddSystem(new RenderSystem(world));
            world.AddSystem(new MovementSystem(world));
            world.AddSystem(new DamageSystem(world));
            world.AddSystem(new EnemyLogicSystem(world));
            world.AddSystem(new InputSystem(world));
            world.AddSystem(new ExplosionSystem(world));
            world.AddSystem(new NotificationSystem(world));
            world.AddSystem(new LevelSystem(world));
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
            bossTexture = Content.Load<Texture2D>("bossEnemy");

            backgroundMusic = Content.Load<Song>("loop-transit");

            //Ship textures
            shipTextures = new List<Texture2D>();
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

            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = .0f;

            MediaPlayer.Play(backgroundMusic);

            PushScreen(new BackgroundScreen());
            PushScreen(new StartScreen());
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            for(int i = screens.Count - 1; i >= 0; i--)
            {
                BaseScreen screen = screens[i];
                screen.Update(gameTime);
                if(screen.pausesBelow)
                {
                    break;
                }
            }

            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();

            foreach(BaseScreen screen in screens)
            {
                screen.Draw(spriteBatch, scoreFont);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
