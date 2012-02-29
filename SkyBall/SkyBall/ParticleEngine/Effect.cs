using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PongFS.ParticleEngine.Particles;
using PongFS.Drawable;

namespace PongFS.Particles
{
    public class Effect
    {
        private const float EMISSION_STEP = 0.1f;
        public enum Template{Fire, Smoke}

        private Game game;
        private SpriteBatch spriteBatch;
        private List<BaseParticle> particles = new List<BaseParticle>();
        private float emissionTimer;
        private Random rnd = new Random();
        private Template tpl;
        public Vector2 Force { get; set; } // change by force here
        public Texture2D Texture { get; set; }
        public float EmissionSpeed { get; set; }
        public float MaxAge{get;set;}
        public Color ModColor { get; set; }
        public int Intensity { get; set; }
        public bool Enabled { get; set; }

        public Effect(Game game, SpriteBatch spriteBatch, Template tpl)
        {
            this.game = game;
            this.spriteBatch = spriteBatch;
            this.tpl = tpl;
            Enabled = true;
            switch (tpl)
            {
                case Template.Smoke:
                    Texture = game.Content.Load<Texture2D>("images/smoke");
                    MaxAge = 1000f;
                    EmissionSpeed = 0f;
                    Intensity = 6;
                    break;
                case Template.Fire:
                    Texture = game.Content.Load<Texture2D>("images/fire");
                    EmissionSpeed = 0f;
                    MaxAge = 500f;
                    Intensity = 1;
                    break;
            }
        }

        internal void Update(DrawableGameObject source, GameTime gameTime)
        {
            float now = (float)gameTime.TotalGameTime.TotalMilliseconds;
            emissionTimer += EMISSION_STEP;
            if (Enabled && emissionTimer > EmissionSpeed)
            {
                emissionTimer = 0;
                for (int i = 0; i < Intensity; i++)
                {

                    switch (tpl)
                    {
                        case Template.Smoke:
                            particles.Add(new SmokeParticle(gameTime, source));
                            break;
                        case Template.Fire:
                            particles.Add(new FireParticle(gameTime, source));
                            break;
                    }
                }
            }

            for (int i = particles.Count - 1; i >= 0; i--)
            {
                BaseParticle particle = particles[i];
                float timeAlive = now - particle.BirthTime;
                if (timeAlive > particle.MaxAge)
                {
                    particles.RemoveAt(i);
                }
                else
                {
                    particle.Update(gameTime, source);
                }
            }
        }

        public void Draw()
        {
            foreach (BaseParticle particle in particles)
            {
                particle.Draw(spriteBatch, Texture);
            }
        }
            
    }
}
