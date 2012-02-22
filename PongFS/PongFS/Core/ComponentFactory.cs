using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PongFS.Core
{
    public class ComponentFactory
    {
        private Game game;
        private static ComponentFactory factory = null;
        private Dictionary<String, IGameComponent> components = new Dictionary<string, IGameComponent>();

        private ComponentFactory() { }

        public static ComponentFactory getFactory()
        {
            if (factory == null)
            {
                factory = new ComponentFactory();
            }
            return factory;
        }

        public void Initialize(Game game)
        {
            this.game = game;
        }

        public void Add(string key, IGameComponent component)
        {
            if (key != null)
            {
                components.Add(key, component);
                game.Components.Add(component);
            }
        }

        public IGameComponent Get(string key)
        {
            IGameComponent value = null;
            components.TryGetValue(key, out value);
            return value;
        }
                    
    }
}
