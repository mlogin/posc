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
        private Game game;
        private static ParticleFactory factory = null;
        private Dictionary<String, Emitter> emitters = new Dictionary<string, Emitter>();

        private ParticleFactory() { }

        public static ParticleFactory getFactory()
        {
            if (factory == null)
            {
                factory = new ParticleFactory();
            }
            return factory;
        }

        public void Initialize(Game game)
        {
            this.game = game;
        }

        public void Add(string key, Emitter component)
        {
            if (key != null)
            {
                emitters.Add(key, component);
            }
        }

        public Emitter Get(string key)
        {
            Emitter value = null;
            emitters.TryGetValue(key, out value);
            return value;
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
    }
}
