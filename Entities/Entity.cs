using System;
using System.Collections.Generic;
using MonoSpaceShooter.Components;
using MonoSpaceShooter.Utilities;

namespace MonoSpaceShooter.Entities
{
    public class Entity
    {
        public readonly Dictionary<Type, BaseComponent> components;
        public World world = null;

        public Entity()
        {
            components = new Dictionary<Type, BaseComponent>();
        }

        public void AddComponent(BaseComponent component)
        {
            if(components.ContainsKey(component.GetType()))
            {
                throw new Exception("Duplicate component " + component.GetType().ToString());
            }
            components.Add(component.GetType(), component);
            if (world != null)
            {
                world.EntityGainedComponent(this, component.GetType());
            }
        }

        public void RemoveComponent(Type type)
        {
            components.Remove(type);
            if(world != null)
            {
                world.EntityLostComponent(this, type);
            }
        }

        public bool HasComponent(Type type)
        {
            return components.ContainsKey(type);
        }
    }
}
