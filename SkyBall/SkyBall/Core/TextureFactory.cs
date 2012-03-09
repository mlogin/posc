using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.IO;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace SkyBall.Core
{
    public class TextureFactory
    {
        private ContentManager content;
        private static TextureFactory factory = null;
        private Dictionary<String, Texture2D> textures = new Dictionary<string, Texture2D>();

        private TextureFactory() { }

        public static TextureFactory getFactory()
        {
            if (factory == null)
            {
                factory = new TextureFactory();
            }
            return factory;
        }

        public void Initialize(ContentManager content)
        {
            this.content = content;
        }

        public void Add(string key, string filename)
        {
            if (!textures.ContainsKey(key))
            {
                textures.Add(key, content.Load<Texture2D>(filename));
            }
        }

        public Texture2D Get(string key){
            return textures.ContainsKey(key) ? textures[key] : null;
        }

        internal void RemoveAll()
        {
            textures.Clear();
        }
    }
}
