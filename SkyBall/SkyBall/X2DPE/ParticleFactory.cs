using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using X2DPE;

namespace SkyBall.X2DPE
{
    public class ParticleFactory
    {
        private static ParticleFactory factory = null;
        private Dictionary<String, Emitter> emitters = new Dictionary<string, Emitter>();
        private static int idCounter = 0;

        private ParticleFactory() { }

        public static ParticleFactory getFactory()
        {
            if (factory == null)
            {
                factory = new ParticleFactory();
            }
            return factory;
        }

        public void Add(string key, Emitter emitter)
        {
            string id = key == null ? NewId() : key;
            if (!emitters.ContainsKey(id))
            {
                emitters.Add(id, emitter);
            }
        }

        public void Add(Emitter emitter)
        {
            Add(null, emitter);
        }

        public Emitter Get(string key)
        {
            return emitters.ContainsKey(key) ? emitters[key] : null;
        }

        public void Update(GameTime gameTime)
        {
            foreach (Emitter emitter in emitters.Values)
            {
                emitter.UpdateParticles(gameTime);
            }
        }


        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (Emitter emitter in emitters.Values)
            {
                emitter.DrawParticles(gameTime, spriteBatch);
            }
        }

        private string NewId()
        {
            idCounter++;
            return "emi-" + idCounter.ToString();
        }
    }
}
