using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using PongFS.Drawable;

namespace PongFS.Particles
{
    public class ParticleFactory
    {
        private static ParticleFactory factory = null;
        public static Random Random = new Random();
        private List<Emitter> emitters = new List<Emitter>();
        private Game game;
        private SpriteBatch spriteBatch;

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
        
        public Emitter CreateParticleEmitter(SpriteBatch spriteBatch, DrawableGameObject source)
        {
            this.spriteBatch = spriteBatch;
            Emitter emitter = new Emitter(game, spriteBatch, source);
            emitters.Add(emitter);
            return emitter;
        }

        public void UpdateParticles(GameTime gameTime)
        {
            foreach (Emitter emitter in emitters)
            {
                emitter.Update(gameTime);
            }
        }

        public void DrawParticles()
        {
            foreach (Emitter emitter in emitters)
            {
                emitter.Draw();
            }
        }

    }
}
