using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.IO;

namespace PongFS.Core
{
    public class ComponentFactory
    {
        private Game game;
        private static ComponentFactory factory = null;
        private Dictionary<String, Object> components = new Dictionary<string, Object>();

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

        public void Add(string key, Object component)
        {
            if (key != null)
            {
                components.Add(key, component);
                if (component is IGameComponent)
                {
                    game.Components.Add((IGameComponent)component);
                }
            }
        }

        public Object Get(string key)
        {
            Object value = null;
            components.TryGetValue(key, out value);
            return value;
        }


        internal string NewId()
        {
            return Path.GetRandomFileName().Replace(".", "");
        }
    }
}
