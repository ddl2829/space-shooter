using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoSpaceShooter.Components;
using MonoSpaceShooter.Entities;
using MonoSpaceShooter.Systems;
using SpaceShooter;

namespace MonoSpaceShooter.Utilities
{
    public class World
    {
        public Rectangle screenRect;
        List<BaseSystem> systems;
        List<Entity> entities;
        Dictionary<Type, List<Entity>> componentMap;
        //QuadTree<QuadStorable> quadtreeEntities;

        public World(Rectangle rect)
        {
            screenRect = rect;
            systems = new List<BaseSystem>();
            entities = new List<Entity>();
            componentMap = new Dictionary<Type, List<Entity>>();
            //quadtreeEntities = new QuadTree<QuadStorable>(rect);
        }

        public void AddSystem(BaseSystem system)
        {
            systems.Add(system);
        }

        public void RemoveSystem(BaseSystem system)
        {
            systems.Remove(system);
        }

        public void Clear()
        {
            entities.Clear();
            componentMap.Clear();
        }

        public void EntityLostComponent(Entity e, Type type)
        {
            componentMap[type].Remove(e);
        }

        public void EntityGainedComponent(Entity e, Type type)
        {
            componentMap[type].Add(e);
        }

        public List<Entity> GetEntities(Type[] withComponents)
        {
            return entities.FindAll(delegate (Entity e)
            {
                foreach(Type shouldHave in withComponents)
                {
                    if(componentMap.ContainsKey(shouldHave))
                    {
                        if(!componentMap[shouldHave].Contains(e))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            });
        }

        public void AddEntity(Entity e)
        {
            entities.Add(e);
            foreach(Type componentType in e.components.Keys)
            {
                if(!componentMap.ContainsKey(componentType))
                {
                    componentMap.Add(componentType, new List<Entity>());
                }
                componentMap[componentType].Add(e);
            }
        }

        public void RemoveEntity(Entity e)
        {
            entities.Remove(e);
            foreach (Type componentType in e.components.Keys)
            {
                if (componentMap.ContainsKey(componentType))
                {
                    componentMap[componentType].Remove(e);
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach(BaseSystem system in systems)
            {
                system.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            Console.WriteLine("World Draw Called");
            foreach (BaseSystem system in systems)
            {
                system.Draw(spriteBatch, spriteFont);
            }
        }
    }
}
