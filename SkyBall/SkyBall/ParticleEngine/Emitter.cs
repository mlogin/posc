using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PongFS.Drawable;

namespace PongFS.Particles
{
    public class Emitter
    {
        public bool Enabled { get; set; }
        private Vector2 position;
        private Dictionary<string, Effect> effects = new Dictionary<string, Effect>();
        private Game game;
        private SpriteBatch spriteBatch;
        private DrawableGameObject source;

        public Emitter(Game game, SpriteBatch spriteBatch, DrawableGameObject source)
        {
            Init(game, spriteBatch);
            this.source = source;
            this.position = source.Position;
        }

        private void Init(Game game, SpriteBatch spriteBatch)
        {
            this.game = game;
            this.spriteBatch = spriteBatch;
        }

        internal void AddEffect(Effect.Template template, string key)
        {
            Effect effect = new Effect(game, spriteBatch, template);
            if (!effects.ContainsKey(key))
            {
                effects.Add(key, effect);
                Enabled = true;
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach (Effect effect in effects.Values)
            {
                effect.Update(source, gameTime);
            }
        }

        public void Draw()
        {
            foreach (Effect effect in effects.Values)
            {
                effect.Draw();
            }
        }


        internal void RemoveEffect(string key)
        {
            if(effects.ContainsKey(key)){
                effects.Remove(key);
            }
        }

        internal void DisableEffect(string key)
        {
            if (effects.ContainsKey(key))
            {
                effects[key].Enabled = false;
            }
        }

        internal void EnableEffect(string key)
        {
            if (effects.ContainsKey(key))
            {
                effects[key].Enabled = true;
            }
        }
    }
}
