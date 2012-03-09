using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using X2DPE;
using X2DPE.Helpers;
using SkyBall.X2DPE;
using SkyBall.Core;
using SkyBall.Config;

namespace SkyBall.Drawable
{
    public class Ball : DrawableGameObject
    {
        private const float START_SPEED = 3f;
        public static float MIN_SPEED = 3f;
        public static float FIRE_SPEED = 40f;
        public static float PLASMA_SPEED = 100f;
        private Emitter fireEmitter, plasmaEmitter, smokeEmitter;
        public bool IsCrazy;

        public Ball(string id) : base(id, TextureFactory.getFactory().Get("ball")) {
            InitParticleEmitters();
            Speed = new Vector2((float) Tools.Rnd.NextDouble() * 4 - 2, START_SPEED);
            PlaceCenterAt(GameConfig.WIDTH / 2, GameConfig.HEIGHT / 2);
            Bounds = new Rectangle(GameConfig.WIN_BORDER, 0, GameConfig.WIDTH - Width - GameConfig.WIN_BORDER, GameConfig.HEIGHT);
            BounceOffBounds = true; // for now
            OnOutOfBounds += new EventHandler(Ball_OnOutOfBounds);
        }

        private void InitParticleEmitters()
        {
            fireEmitter = new Emitter
            {
                Active = false,
                RandomEmissionInterval = new RandomMinMax(10),
                ParticleLifeTime = 1000,
                ParticleDirection = new RandomMinMax(0),
                ParticleSpeed = new RandomMinMax(0),
                ParticleRotation = new RandomMinMax(360),
                RotationSpeed = new RandomMinMax(0),
                ParticleFader = new ParticleFader(true, true, 200),
                ParticleScaler = new ParticleScaler(0.3f, 0.6f, 0, 500),
                Position = new Vector2(140, 580)
            };
            fireEmitter.TextureList.Add(TextureFactory.getFactory().Get("fire"));
            ParticleFactory.getFactory().Add(fireEmitter);

            plasmaEmitter = new Emitter
            {
                Active = false,
                RandomEmissionInterval = new RandomMinMax(5),
                ParticleLifeTime = 1000,
                ParticleDirection = new RandomMinMax(0),
                ParticleSpeed = new RandomMinMax(0.02),
                ParticleRotation = new RandomMinMax(360),
                RotationSpeed = new RandomMinMax(0),
                ParticleFader = new ParticleFader(true, true, 200),
                ParticleScaler = new ParticleScaler(0.3f, 1f, 0, 500),
                Position = new Vector2(140, 580)
            };
            plasmaEmitter.TextureList.Add(TextureFactory.getFactory().Get("plasma"));
            ParticleFactory.getFactory().Add(plasmaEmitter);

            smokeEmitter = new Emitter
            {
                Active = true,
                RandomEmissionInterval = new RandomMinMax(10),
                ParticleLifeTime = 1200,
                ParticleDirection = new RandomMinMax(0.3),
                ParticleSpeed = new RandomMinMax(0.1),
                ParticleRotation = new RandomMinMax(360),
                RotationSpeed = new RandomMinMax(0),
                ParticleFader = new ParticleFader(false, true, 200),
                ParticleScaler = new ParticleScaler(0.15f, 0.3f, 0, 500),
                Position = new Vector2(140, 580)
            };
            smokeEmitter.TextureList.Add(TextureFactory.getFactory().Get("smoke"));
            ParticleFactory.getFactory().Add(smokeEmitter);
        }

        void Ball_OnOutOfBounds(object sender, EventArgs e)
        {
            //if (Rect.X < Bounds.X || Rect.X > Bounds.Width)
            //{
            //    Speed = new Vector2(-Speed.X, Speed.Y);
            //}
            //else
            //{
            //    // a player hit top / bottom
            //}
        }

        public override void Update(GameTime gameTime)
        {
            Speed = new Vector2(Speed.X + (IsCrazy ? 2*(float)Math.Cos(Speed.X):0), Speed.Y);
            base.Update(gameTime);
            smokeEmitter.Position = Center;
            fireEmitter.Position = Center;
            plasmaEmitter.Position = Center;
            float sq = Speed.LengthSquared();

            if (sq < FIRE_SPEED)
            {
                fireEmitter.Active = false;
                plasmaEmitter.Active = false;
            }
            else if (sq > FIRE_SPEED && sq < PLASMA_SPEED)
            {
                fireEmitter.Active = true;
                plasmaEmitter.Active = false;
            }
            else if (sq > PLASMA_SPEED)
            {
                fireEmitter.Active = false;
                plasmaEmitter.Active = true;
            }
        }

    }
}
