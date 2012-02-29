using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PongFS.Particles;
using PongFS.Drawable;

namespace PongFS.ParticleEngine.Particles
{
    public class SmokeParticle : BaseParticle
    {

        public SmokeParticle(GameTime gameTime, DrawableGameObject source)
        {
            BirthTime = (float)gameTime.TotalGameTime.TotalMilliseconds;
            MaxAge = 1500f;
            ModColor = Color.White;
            OriginalPosition = new Vector2(source.Rect.Center.X, source.Rect.Center.Y);
            Position = OriginalPosition;
            Rotation = ParticleFactory.Random.Next(360);
            Scaling = 0.3f;
        }

        public override void Update(GameTime gameTime, DrawableGameObject source)
        {
            float now = (float)gameTime.TotalGameTime.TotalMilliseconds;
            float timeAlive = now - BirthTime;

            float relAge = timeAlive / MaxAge;
            Position = 0.5f * Acceleration * relAge * relAge + Speed * relAge + OriginalPosition;

            float invAge = 1.0f - relAge;
            ModColor = new Color(new Vector4(invAge, invAge, invAge, invAge));

            Vector2 positionFromCenter = Position - OriginalPosition;
            float distance = positionFromCenter.Length();
        }
    }
}
