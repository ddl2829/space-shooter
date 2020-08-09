using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSpaceShooter.Components;
using MonoSpaceShooter.Entities;
using MonoSpaceShooter.Utilities;

namespace MonoSpaceShooter.Systems
{
    public class NotificationSystem : BaseSystem
    {
        public NotificationSystem(World world) : base(world)
        {
        }

        public override void Update(GameTime gametime)
        {
            List<Entity> notifications = world.GetEntities(new[] { typeof(NotificationComponent) });
            foreach (Entity notification in notifications)
            {
                NotificationComponent notificationComponent = (NotificationComponent)notification.components[typeof(NotificationComponent)];
                notificationComponent.elapsedTime += gametime.ElapsedGameTime.Milliseconds;
                if (notificationComponent.elapsedTime > notificationComponent.maxLife)
                {
                    world.RemoveEntity(notification);
                    continue;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont)
        {

            List<Entity> notifications = world.GetEntities(new[] { typeof(NotificationComponent) });
            foreach (Entity notification in notifications)
            {
                NotificationComponent notificationComponent = (NotificationComponent)notification.components[typeof(NotificationComponent)];
                if (notificationComponent.centerText)
                {

                    spriteBatch.DrawString(
                        spriteFont,
                        notificationComponent.text,
                        new Vector2(
                            world.screenRect.Width / 2 - spriteFont.MeasureString(notificationComponent.text).X / 2,
                            world.screenRect.Height / 3 - spriteFont.MeasureString(notificationComponent.text).Y / 2
                        ),
                        notificationComponent.color
                    );
                }
                else
                {
                    if (notification.HasComponent(typeof(PositionComponent)))
                    {
                        PositionComponent pc = (PositionComponent)notification.components[typeof(PositionComponent)];
                        spriteBatch.DrawString(spriteFont, notificationComponent.text, pc.position, notificationComponent.color);
                    }
                }

            }
        }
    }
}
