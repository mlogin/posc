using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using X2DPE;
using X2DPE.Helpers;
using PongFS.X2DPE;
//using PongFS.Particles;

namespace PongFS.Drawable
{
    public class Ball : DrawableGameObject
    {
        private const float START_SPEED = 3f;
        public static float MIN_SPEED = 3f;
        private Emitter fireEmitter, smokeEmitter;

        public Ball(Game game, string id) : base(game, id) { }

        public override void Initialize()
        {
            base.Initialize();
            InitParticleEmitters();
            Speed = new Vector2(- 1, START_SPEED);
            OnReady += new EventHandler(Ball_OnReady);
        }

        private void InitParticleEmitters()
        {
            fireEmitter = new Emitter();
            fireEmitter.Active = false;
            fireEmitter.TextureList.Add(Game.Content.Load<Texture2D>("images\\fire"));
            fireEmitter.RandomEmissionInterval = new RandomMinMax(10);
            fireEmitter.ParticleLifeTime = 1000;
            fireEmitter.ParticleDirection = new RandomMinMax(0);
            fireEmitter.ParticleSpeed = new RandomMinMax(0);
            fireEmitter.ParticleRotation = new RandomMinMax(360);
            fireEmitter.RotationSpeed = new RandomMinMax(0);
            fireEmitter.ParticleFader = new ParticleFader(true, true, 200);
            fireEmitter.ParticleScaler = new ParticleScaler(0.3f, 0.6f, 0, 500);
            fireEmitter.Position = new Vector2(140, 580);
            ParticleFactory.getFactory().Add("fire", fireEmitter);

            smokeEmitter = new Emitter();
            smokeEmitter.Active = true;
            smokeEmitter.TextureList.Add(Game.Content.Load<Texture2D>("images\\smoke"));
            smokeEmitter.RandomEmissionInterval = new RandomMinMax(10);
            smokeEmitter.ParticleLifeTime = 1200;
            smokeEmitter.ParticleDirection = new RandomMinMax(0.3);
            smokeEmitter.ParticleSpeed = new RandomMinMax(0.1);
            smokeEmitter.ParticleRotation = new RandomMinMax(360);
            smokeEmitter.RotationSpeed = new RandomMinMax(0);
            smokeEmitter.ParticleFader = new ParticleFader(false, true, 200);
            smokeEmitter.ParticleScaler = new ParticleScaler(0.15f, 0.3f, 0, 500);
            smokeEmitter.Position = new Vector2(140, 580);
            ParticleFactory.getFactory().Add("smoke", smokeEmitter);
        }

        void Ball_OnReady(object sender, EventArgs e)
        {
            PlaceCenterAt(screenWidth / 2, screenHeight / 2);
            Bounds = new Rectangle(Engine.WIN_BORDER, 0, screenWidth - Width - Engine.WIN_BORDER, screenHeight);
            BounceOffBounds = true; // for now
            OnOutOfBounds += new EventHandler(Ball_OnOutOfBounds);


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
            base.Update(gameTime);
            if (Speed.LengthSquared() > 40f)
            {
                fireEmitter.Active = true;
            }
            else
            {
                fireEmitter.Active = false;
            }
        }

    }
}
