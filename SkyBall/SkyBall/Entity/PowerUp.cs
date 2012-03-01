using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SkyBall.Drawable;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkyBall.Core;
using SkyBall.Config;

namespace SkyBall.Entity
{
    public class PowerUp : DrawableGameObject
    {
        public const double TIME_ACTIVE = 20;
        public static int SPAWN_RND = 1000;

        public bool Displayed { get; set; }
        public Power.PowerType PowerUpType { get; set; }
        private Rectangle SpriteRegion { get; set; }
        private Random rnd = new Random();
        public PowerUp(string id) : base(id,  TextureFactory.getFactory().Get("powerups"), false) {
        }

        public void ReInitialize()
        {
            Array types = Enum.GetValues(typeof(Power.PowerType));
            PowerUpType = (Power.PowerType)types.GetValue(rnd.Next(types.Length - 1));
            InitialPosition = new Vector2(rnd.Next(GameConfig.WIDTH), (GameConfig.HEIGHT - Height) / 2);
            float posX = (InitialPosition.X + 1) % GameConfig.WIDTH;
            float posY = 50 * (float)Math.Sin(posX * Math.PI / 250) + (GameConfig.HEIGHT - Height) / 2;
            Position = new Vector2(posX, posY);
            Displayed = true;
            SpriteRegion = new Rectangle((int)PowerUpType * 32, 0, 32, 32);
        }

        public int PowerUpSpeed { 
            get{
                switch (PowerUpType)
                {
                    default:
                    case Power.PowerType.HealSingle:
                    case Power.PowerType.Power:
                    case Power.PowerType.Speed:
                    case Power.PowerType.Wall:
                        return 3;
                    case Power.PowerType.ReverseKeys:
                    case Power.PowerType.RevealAim:
                    case Power.PowerType.Ice:
                    case Power.PowerType.Bend:
                        return 2;
                    case Power.PowerType.HealAll:
                    case Power.PowerType.Fortress:
                        return 1;
                }
            }
        }

        public TimeSpan LifeTime
        {
            get
            {
                switch (PowerUpType)
                {
                    case Power.PowerType.HealSingle:
                    case Power.PowerType.HealAll:
                    case Power.PowerType.Wall:
                    case Power.PowerType.Fortress:
                        return TimeSpan.Zero;
                    default:
                        return TimeSpan.FromSeconds(TIME_ACTIVE);
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (Displayed)
            {
                float posX = (Position.X + 1) % GameConfig.WIDTH;
                float posY = 50 * (float)Math.Sin(posX * Math.PI / 250) + (GameConfig.HEIGHT - Height) / 2;
                Position = new Vector2(posX, posY);

                // check for collisions
                Ball ball = (Ball)ComponentFactory.getFactory().Get("ball");
                Rectangle hitRegion = new Rectangle((int)Position.X, (int)Position.Y, 32, Height);
                if (hitRegion.Intersects(ball.Rect))
                {
                    string playerId = "p" + (ball.Speed.Y > 0 ? "1" : "2");
                    string opponentId = "p" + (ball.Speed.Y < 0 ? "1" : "2");
                    Player player = (Player)ComponentFactory.getFactory().Get(playerId);
                    Player opponent = (Player)ComponentFactory.getFactory().Get(opponentId);
                    if (PowerUpType == Power.PowerType.ReverseKeys || 
                        PowerUpType == Power.PowerType.Ice ||
                        PowerUpType == Power.PowerType.RevealAim) // CURSE POWER!
                    {
                        opponent.GrantPower(new Power { type = PowerUpType, acquired = gameTime.TotalGameTime });
                    }
                    else
                    {
                        player.GrantPower(new Power { type = PowerUpType, acquired = gameTime.TotalGameTime });
                    }
                    this.Displayed = false;
                }
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (Displayed)
            {
                spriteBatch.Draw(texture, Position, SpriteRegion, ModColor, Rotation, Vector2.Zero, Scaling, SpriteEffects.None, 0);
            }
        }

        
    }
}
