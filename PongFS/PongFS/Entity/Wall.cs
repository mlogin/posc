using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PongFS.Drawable;
using Microsoft.Xna.Framework.Graphics;
using PongFS.Core;
using X2DPE;
using X2DPE.Helpers;
using PongFS.X2DPE;

namespace PongFS.Entity
{
    public class Wall : DrawableGameObject
    {
        public int Life { get; set; }
        private int maxLife;
        private Color[] colors = new Color[]{
            Color.DarkGray, Color.Red, Color.OrangeRed, Color.Orange, Color.Yellow, Color.GreenYellow, Color.Green
        };
        private Rectangle hitRegion;
        private Emitter explodeEmitter;
        public Character.PlayerPosition ScreenPosition {get;set;}
        private TimeSpan timeExplosion;

        public Wall(Game game, string id) : base(game, id) { }
        public event EventHandler OnDestroyed;

        private static int test = 0;

        public override void Initialize()
        {
            base.Initialize();
            InitParticleEmitters();
            maxLife = colors.Length;
            Life = maxLife;
            OnReady += new EventHandler(Wall_OnReady);
        }

        private void InitParticleEmitters()
        {
            explodeEmitter = new Emitter();
            explodeEmitter.Active = false;
            explodeEmitter.TextureList.Add(Game.Content.Load<Texture2D>("images/fire"));
            explodeEmitter.RandomEmissionInterval = new RandomMinMax(200);
            explodeEmitter.ParticleLifeTime = 4000;
            explodeEmitter.ParticleDirection = new RandomMinMax(60);
            explodeEmitter.ParticleSpeed = new RandomMinMax(0.5);
            explodeEmitter.ParticleRotation = new RandomMinMax(0.3);
            explodeEmitter.RotationSpeed = new RandomMinMax(0.03);
            explodeEmitter.ParticleFader = new ParticleFader(true, true, 20);
            explodeEmitter.ParticleScaler = new ParticleScaler(0.3f, 50f, 0, 10000);
            ParticleFactory.getFactory().Add("explode-" + id + "-" + ScreenPosition.ToString(), explodeEmitter);
        }

        void Wall_OnReady(object sender, EventArgs e)
        {
            Position = InitialPosition;
            switch (ScreenPosition)
            {
                case Character.PlayerPosition.Top:
                    hitRegion = new Rectangle(Rect.X + 2, Rect.Y, Rect.Width - 2, Rect.Height / 2);
                    break;
                case Character.PlayerPosition.Bottom:
                    hitRegion = new Rectangle(Rect.X + 2, Rect.Y + Rect.Height / 2, Rect.Width - 2, Rect.Height / 2);
                    break;
            }
            explodeEmitter.Position = Center;

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (Life < 0 && gameTime.TotalGameTime - timeExplosion > TimeSpan.FromSeconds(3))
            {
                // player lost
                if (OnDestroyed != null) OnDestroyed(this, null);
            }
            Ball ball = (Ball)ComponentFactory.getFactory().Get("ball");
            if (hitRegion.Intersects(ball.Rect))
            {
                int dir = ScreenPosition == Character.PlayerPosition.Top ? 1 : -1;
                float speedX, speedY;
                speedY = dir * (Math.Abs(ball.Speed.Y) * 2f / 3);
                if (speedY <= 0 && speedY > -Ball.MIN_SPEED)
                {
                    speedY = -Ball.MIN_SPEED;
                }
                else if (speedY >= 0 && speedY < Ball.MIN_SPEED)
                {
                    speedY = Ball.MIN_SPEED;
                }

                Life -= (int) Math.Ceiling(Math.Abs(ball.Speed.Y) / 5);
                if (Life < 0)
                {
                    if (!explodeEmitter.Active) timeExplosion = gameTime.TotalGameTime;
                    explodeEmitter.Active = true;
                }
                else
                {
                    ModColor = colors[Life];
                }
                speedX = ball.Speed.X;
                ball.Speed = new Vector2(speedX, speedY);
            }


        }


    }
}
