using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSpaceShooter.Components;
using MonoSpaceShooter.Entities;
using MonoSpaceShooter.Utilities;

namespace MonoSpaceShooter.Systems
{
    public class RenderSystem : BaseSystem
    {
        public RenderSystem(World world) : base(world)
        {
        }

        public override void Update(GameTime gametime)
        {
            List<Entity> explosions = world.GetEntities(new[] { typeof(ExplosionComponent), typeof(PositionComponent), typeof(RenderComponent) });
            foreach (Entity explosion in explosions)
            {
                PositionComponent pc = (PositionComponent)explosion.components[typeof(PositionComponent)];
                RenderComponent rc = (RenderComponent)explosion.components[typeof(RenderComponent)];
                ExplosionComponent ec = (ExplosionComponent)explosion.components[typeof(ExplosionComponent)];
                ec.elapsedTime += gametime.ElapsedGameTime.Milliseconds;
                if (ec.elapsedTime > ec.maxLife)
                {
                    world.RemoveEntity(explosion);
                    continue;
                }
            }

            List<Entity> notifications = world.GetEntities(new[] { typeof(NotificationComponent), typeof(PositionComponent) });
            foreach(Entity notification in notifications)
            {
                PositionComponent pc = (PositionComponent)notification.components[typeof(PositionComponent)];
                NotificationComponent ec = (NotificationComponent)notification.components[typeof(NotificationComponent)];
                ec.elapsedTime += gametime.ElapsedGameTime.Milliseconds;
                if(ec.elapsedTime > ec.maxLife)
                {
                    world.RemoveEntity(notification);
                    continue;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            List<Entity> sprites = world.GetEntities(new[] { typeof(RenderComponent), typeof(PositionComponent) });
            foreach(Entity drawable in sprites)
            {
                PositionComponent pc = (PositionComponent)drawable.components[typeof(PositionComponent)];
                RenderComponent rc = (RenderComponent)drawable.components[typeof(RenderComponent)];
                spriteBatch.Draw(rc.texture, new Rectangle((int)pc.position.X - (rc.texture.Width / 2), (int)pc.position.Y - (rc.texture.Height / 2), rc.texture.Width, rc.texture.Height), Color.White);
            }

            List<Entity> notifications = world.GetEntities(new[] { typeof(NotificationComponent), typeof(PositionComponent) });
            foreach (Entity notification in notifications)
            {
                PositionComponent pc = (PositionComponent)notification.components[typeof(PositionComponent)];
                NotificationComponent rc = (NotificationComponent)notification.components[typeof(NotificationComponent)];
                if(rc.centerText)
                {
                    spriteBatch.DrawString(
                        spriteFont,
                        rc.text,
                        new Vector2(
                            world.screenRect.Width / 2 - spriteFont.MeasureString(rc.text).X / 2,
                            world.screenRect.Height / 3 - spriteFont.MeasureString(rc.text).Y / 2
                        ),
                        rc.color
                    );
                } else
                {
                    spriteBatch.DrawString(spriteFont, rc.text, pc.position, rc.color);
                }
                
            }
        }
    }
}
