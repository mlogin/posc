using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkyBall.Physics;
using SkyBall.SpriteSheet;
using SkyBall.Core;
using SkyBall.Config;

namespace SkyBall.Drawable
{
    public class DrawableGameObject
    {
        public event EventHandler OnOutOfBounds;

        protected Texture2D texture;
        public Vector2 InitialPosition { get; set; }
        public string Id { get; private set; }
        public Vector2 Position
        {
            get
            {
                return new Vector2(Rect.X, Rect.Y);
            }
            set
            {
                Rect = new Rectangle((int)value.X, (int)value.Y, width, height);
            }
        } // up-left corner of object
        public Vector2 Center
        {
            get { return new Vector2(Rect.Center.X, Rect.Center.Y); }
        }
        public Vector2 Speed { get; set; }
        protected float MaxSpeed { get; set; }
        public Vector2 Acceleration { get; set; }
        protected float MaxAcceleration { get; set; }
        public Vector2 Scaling { get; set; }
        public float Rotation { get; set; }
        public Color ModColor { get; set; }
        public Rectangle Rect{ get; private set;}
        public int Width { get { return width; } }
        public int Height { get { return height; } }
        protected List<Force> forces = new List<Force>();
        protected bool canMove;
        public Rectangle Bounds { get; set; }
        protected bool BounceOffBounds { get; set; }
        private int width, height;
        public bool IsVisible { get; set; }

        // animation
        public bool IsAnimated {get; set;}
        protected SpriteSheetLoader spriteSheetLoader;
        protected string currentAnimation;

        public DrawableGameObject(string id, Texture2D texture, bool isAnimated = false, SpriteSheetLoader spriteSheetLoader = null)
        {
            Id = id;
            this.texture = texture;
            ComponentFactory.getFactory().Add(id, this);
            IsVisible = true;
            IsAnimated = isAnimated;
            Scaling = new Vector2(1f);
            ModColor = Color.White;
            if (IsAnimated)
            {
                this.spriteSheetLoader = spriteSheetLoader;
            }
            SetTexture(texture);
        }

        private void SetTexture(Texture2D texture)
        {
            this.texture = texture;
            if (IsAnimated)
            {
                width = spriteSheetLoader.GetCurrentAnimationFrame().rect.Width;
                height = spriteSheetLoader.GetCurrentAnimationFrame().rect.Height;
            }
            else
            {
                width = texture.Width;
                height = texture.Height;
            }
        }

        public virtual void Update(GameTime gameTime)
        {
            // update animation frame
            if (IsAnimated && spriteSheetLoader != null)
            {
                Frame frame = spriteSheetLoader.GetNextAnimationFrame();
                Rect = new Rectangle(Rect.X, Rect.Y, frame.rect.Width, frame.rect.Height);
            }

            // apply forces
            foreach (Force force in forces.FindAll(x => x.enabled))
            {
                ApplyForce(force);
            }

            if (canMove)
            {
                Speed = new Vector2(MathHelper.Clamp(Speed.X + Acceleration.X, -MaxSpeed, MaxSpeed),
                                    MathHelper.Clamp(Speed.Y + Acceleration.Y, -MaxSpeed, MaxSpeed));
            }

            // bounderies
            if (Bounds.Width > 0 && Bounds.Height > 0 && !Bounds.Contains(Rect))
            {
                if (Rect.Left < Bounds.Left)
                {
                    Position = new Vector2(Bounds.Left , Position.Y);
                    Speed = new Vector2(BounceOffBounds ? -Speed.X : 0, Speed.Y);
                }
                else if (Rect.Right > Bounds.Right)
                {
                    Position = new Vector2(Bounds.Right - Rect.Width, Position.Y);
                    Speed = new Vector2(BounceOffBounds ? -Speed.X : 0, Speed.Y);
                }

                if (Rect.Top < Bounds.Top)
                {
                    Position = new Vector2(Position.X, Bounds.Top);
                    if (BounceOffBounds) Speed = new Vector2(Speed.X, -Speed.Y);
                }
                else if (Rect.Bottom > Bounds.Bottom)
                {
                    Position = new Vector2(Position.X, Bounds.Bottom - Height);
                    if (BounceOffBounds) Speed = new Vector2(Speed.X, -Speed.Y);
                }
                
                if(OnOutOfBounds != null) OnOutOfBounds(this, null);
            }

            Speed += Acceleration;
            Position += Speed;

        }

        private void ApplyForce(Force force)
        {
            Acceleration *= force.acceleration;
            Speed *= force.speed;
            if (Speed.LengthSquared() < 0.15f)
            {
                Speed = Vector2.Zero;
            }
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (IsVisible)
            {
                if (IsAnimated && spriteSheetLoader != null)
                {
                    spriteBatch.Draw(texture, Position, spriteSheetLoader.GetCurrentAnimationFrame().rect, ModColor, Rotation, Vector2.Zero, Scaling, SpriteEffects.None, 0);
                }
                else
                {
                    spriteBatch.Draw(texture, Position, null, ModColor, Rotation, Vector2.Zero, Scaling, SpriteEffects.None, 0);
                }
            }
        }

        protected void PlaceCenterAt(int x, int y)
        {
            Position = new Vector2(x - Width / 2, y - Height / 2);
        }

        protected void SetAnimation(string animationKey)
        {
            if (IsAnimated && spriteSheetLoader != null)
            {
                this.currentAnimation = animationKey;
                spriteSheetLoader.SetCurrentAnimation(animationKey);
            }
        }

    }
}
