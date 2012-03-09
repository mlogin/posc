using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.IO;

namespace SkyBall.Core
{
    public class ComponentFactory
    {
        private static ComponentFactory factory = null;
        private Dictionary<String, Object> components = new Dictionary<string, Object>();
        private static int idCounter = 0;

        private ComponentFactory() { }

        public static ComponentFactory getFactory()
        {
            if (factory == null)
            {
                factory = new ComponentFactory();
            }
            return factory;
        }

        public void Add(string key, Object component)
        {
            string id = key == null ? NewId() : key; 
            if (!components.ContainsKey(id))
            {
                components.Add(id, component);
            }
        }

        public Object Get(string key)
        {
            return components.ContainsKey(key) ? components[key] : null;
        }

        private string NewId()
        {
            idCounter++;
            return "cmp-" + idCounter.ToString();
        }

        internal void RemoveAll()
        {
            components.Clear();
        }
    }
}
