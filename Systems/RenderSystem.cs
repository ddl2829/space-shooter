using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSpaceShooter.Components;
using MonoSpaceShooter.Entities;
using MonoSpaceShooter.Utilities;
using SpaceShooter;

namespace MonoSpaceShooter.Systems
{
    public class RenderSystem : BaseSystem
    {
        public RenderSystem(World world) : base(world)
        {
        }

        public override void Update(GameTime gametime)
        {
        }

        public override void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            List<Entity> sprites = world.GetEntities(new[] { typeof(RenderComponent), typeof(PositionComponent) });
            foreach(Entity drawable in sprites)
            {
                PositionComponent pc = (PositionComponent)drawable.components[typeof(PositionComponent)];
                RenderComponent rc = (RenderComponent)drawable.components[typeof(RenderComponent)];
                if (rc.visible)
                {
                    spriteBatch.Draw(rc.CurrentTexture, new Rectangle((int)pc.position.X - (rc.CurrentTexture.Width / 2), (int)pc.position.Y - (rc.CurrentTexture.Height / 2), rc.CurrentTexture.Width, rc.CurrentTexture.Height), Color.White);

                    if (drawable.HasComponent(typeof(PlayerComponent)))
                    {
                        PlayerComponent player = (PlayerComponent)drawable.components[typeof(PlayerComponent)];
                        if (drawable.HasComponent(typeof(ShieldedComponent)))
                        {
                            Texture2D shield = Game1.instance.playerShield;
                            spriteBatch.Draw(shield, new Rectangle((int)(pc.position.X - rc.CurrentTexture.Width / 2) - 25, (int)(pc.position.Y - rc.CurrentTexture.Height / 2) - 30, shield.Width, shield.Height), Color.White);
                        }
                    }
                }
            }

            List<Entity> players = world.GetEntities(new[] { typeof(PlayerComponent) });
            if (players.Count > 0)
            {
                Entity player = players[0];
                PlayerComponent pc = (PlayerComponent)player.components[typeof(PlayerComponent)];
                TakesDamageComponent ptdc = (TakesDamageComponent)player.components[typeof(TakesDamageComponent)];
                for (int i = 0; i < pc.lives; i++)
                {
                    spriteBatch.Draw(Game1.instance.playerLivesGraphic, new Rectangle(40 * i + 10, 10, Game1.instance.playerLivesGraphic.Width, Game1.instance.playerLivesGraphic.Height), Color.White);
                }

                string scoreText = "" + Math.Truncate(Game1.instance.playerScore);
                spriteBatch.DrawString(spriteFont, scoreText, new Vector2(world.screenRect.Width - spriteFont.MeasureString(scoreText).X - 30, 5), Color.White);

                spriteBatch.Draw(Game1.instance.blank, new Rectangle(8, 43, 150, 12), Color.Black);
                spriteBatch.Draw(Game1.instance.blank, new Rectangle(9, 44, 148, 10), Color.White);
                spriteBatch.Draw(Game1.instance.blank, new Rectangle(9, 44, (int)(((double)ptdc.health / ptdc.maxHealth) * 148), 10), Color.Red);

                if (player.HasComponent(typeof(HasShieldComponent)))
                {
                    HasShieldComponent hasShieldComponent = (HasShieldComponent)player.components[typeof(HasShieldComponent)];
                    spriteBatch.Draw(Game1.instance.blank, new Rectangle(8, 60, 150, 12), Color.Black);
                    spriteBatch.Draw(Game1.instance.blank, new Rectangle(9, 61, 148, 10), Color.White);
                    spriteBatch.Draw(Game1.instance.blank, new Rectangle(9, 61, (int)((hasShieldComponent.shieldPower / hasShieldComponent.maxShieldPower) * 148), 10), Color.Blue);

                    if (hasShieldComponent.shieldCooldown)
                    {
                        spriteBatch.Draw(Game1.instance.blank, new Rectangle(9, 61, (int)((hasShieldComponent.shieldPower / hasShieldComponent.maxShieldPower) * 148), 10), Color.Purple);
                    }
                }
            }
        }
    }
}
