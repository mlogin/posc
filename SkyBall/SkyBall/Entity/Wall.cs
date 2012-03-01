using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SkyBall.Drawable;
using Microsoft.Xna.Framework.Graphics;
using SkyBall.Core;
using X2DPE;
using X2DPE.Helpers;
using SkyBall.X2DPE;
using SkyBall.Config;

namespace SkyBall.Entity
{
    public class Wall : DrawableGameObject
    {
        public event EventHandler OnDestroyed;
        public int Life { get; set; }
        public int MaxLife { get; private set; }
        public bool IsBonus { get; set; }
        private Color[] colors = new Color[]{
            Color.DarkGray, Color.DarkRed, Color.Red, Color.OrangeRed, Color.Orange,  Color.DarkOrange, Color.Yellow, Color.Gold, Color.Goldenrod, Color.GreenYellow, Color.Green, Color.DarkGreen, Color.White
        };
        private Rectangle hitRegion;
        private Emitter explodeEmitter;
        public Player.Side IsFacing {get; private set;}
        private TimeSpan timeExplosion;

        public Wall(Vector2 position)
            : base(null, TextureFactory.getFactory().Get("wall"))
        {
            InitParticleEmitters();
            MaxLife = colors.Length - 1;
            Heal();
            Position = position;
            IsFacing = position.Y < GameConfig.WIDTH / 2 ? Player.Side.Up : Player.Side.Down;
            switch (IsFacing)
            {
                case Player.Side.Up:
                    hitRegion = new Rectangle(Rect.X + 2, Rect.Y, Rect.Width - 2, Rect.Height / 2);
                    break;
                case Player.Side.Down:
                    hitRegion = new Rectangle(Rect.X + 2, Rect.Y + Rect.Height / 2, Rect.Width - 2, Rect.Height / 2);
                    break;
            }
            explodeEmitter.Position = Center;
        }

        public void Heal()
        {
            Life = MaxLife;
        }

        private void InitParticleEmitters()
        {
            explodeEmitter = new Emitter
            {
                Active = false,
                RandomEmissionInterval = new RandomMinMax(200),
                ParticleLifeTime = 4000,
                ParticleDirection = new RandomMinMax(60),
                ParticleSpeed = new RandomMinMax(0.5),
                ParticleRotation = new RandomMinMax(0.3),
                RotationSpeed = new RandomMinMax(0.03),
                ParticleFader = new ParticleFader(true, true, 20),
                ParticleScaler = new ParticleScaler(0.3f, 50f, 0, 10000)
            };
            explodeEmitter.TextureList.Add(TextureFactory.getFactory().Get("fire"));
            ParticleFactory.getFactory().Add(explodeEmitter);
        }

        public override void Update(GameTime gameTime)
        {
            if (Life >= 0)
            {
                ModColor = colors[Life];
            }
            base.Update(gameTime);
            if (Life < 0 && gameTime.TotalGameTime - timeExplosion > TimeSpan.FromSeconds(3))
            {
                // player lost
                if (OnDestroyed != null) OnDestroyed(this, null);
            }
            Ball ball = (Ball)ComponentFactory.getFactory().Get("ball");
            if (hitRegion.Intersects(ball.Rect) && IsVisible)
            {
                SoundFactory.getFactory().PlaySound("shieldhit", true);
                int dir = IsFacing == Player.Side.Up ? 1 : -1;
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
                    IsVisible = false;
                    if (!IsBonus)
                    {
                        SoundFactory.getFactory().PlaySound("explosion", true);
                        if (!explodeEmitter.Active) timeExplosion = gameTime.TotalGameTime;
                        explodeEmitter.Active = true;
                    }
                }
                speedX = ball.Speed.X;
                ball.Speed = new Vector2(speedX, speedY);
            }


        }


    }
}
