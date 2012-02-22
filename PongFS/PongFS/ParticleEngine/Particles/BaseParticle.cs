using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PongFS.Drawable;

namespace PongFS.ParticleEngine.Particles
{
    public class BaseParticle
    {

        public virtual void Update(GameTime gameTime, DrawableGameObject source) { }

        public float BirthTime { get; set; }
        public float MaxAge { get; set; }
        public Vector2 OriginalPosition { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Speed { get; set; }
        public Vector2 Acceleration { get; set; }
        public float Scaling { get; set; }
        public float Rotation { get; set; }
        public Color ModColor { get; set; }
        public Color OriginalColor { get; set; }

        internal void Draw(SpriteBatch spriteBatch, Texture2D texture)
        {
            spriteBatch.Draw(texture, Position, null, ModColor, Rotation, Vector2.Zero, Scaling, SpriteEffects.None, 1);
        }

    }
}
