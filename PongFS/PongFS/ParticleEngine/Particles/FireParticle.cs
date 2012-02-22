using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PongFS.Particles;
using PongFS.Drawable;

namespace PongFS.ParticleEngine.Particles
{
    public class FireParticle : BaseParticle
    {

        public FireParticle(GameTime gameTime, DrawableGameObject source)
        {
            BirthTime = (float)gameTime.TotalGameTime.TotalMilliseconds;
            MaxAge = 1500f;
            OriginalColor = Color.Yellow;
            OriginalPosition = source.Position;
            Position = source.Position;
            Speed = source.Speed / 4;
            Rotation = 0;
            Scaling = 0.7f;
        }

        public override void Update(GameTime gameTime, DrawableGameObject source)
        {
            float now = (float)gameTime.TotalGameTime.TotalMilliseconds;
            float timeAlive = now - BirthTime;

            float relAge = timeAlive / MaxAge;
            Position = 0.5f * Acceleration * relAge * relAge + Speed * relAge + OriginalPosition;

            float invAge = 1.0f - relAge;
            Scaling = 1.0f - relAge / 2;
            ModColor = OriginalColor * invAge;

            Vector2 positionFromCenter = Position - OriginalPosition;
            float distance = positionFromCenter.Length();
        }
    }
}
