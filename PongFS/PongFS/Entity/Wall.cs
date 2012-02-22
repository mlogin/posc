using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PongFS.Drawable;
using Microsoft.Xna.Framework.Graphics;
using PongFS.Core;

namespace PongFS.Entity
{
    public class Wall : DrawableGameObject
    {
        public int Life { get; set; }
        private int maxLife;
        private Color[] colors = new Color[]{
            Color.DarkGray, Color.Red, Color.OrangeRed, Color.Orange, Color.Yellow, Color.GreenYellow, Color.Green
        };
        
        public Character.PlayerPosition ScreenPosition {get;set;}

        public Wall(Game game, string id) : base(game, id) { }

        public override void Initialize()
        {
            base.Initialize();
            maxLife = colors.Length;
            Life = maxLife;
            OnReady += new EventHandler(Wall_OnReady);
        }

        void Wall_OnReady(object sender, EventArgs e)
        {
            Position = InitialPosition;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Ball ball = (Ball)ComponentFactory.getFactory().Get("ball");
            Rectangle hitRegion = new Rectangle(Rect.X + 5, Rect.Y, Rect.Width - 5, Rect.Height / 2);
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
                Life--;
                if (Life < 0)
                {
                    // event
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
