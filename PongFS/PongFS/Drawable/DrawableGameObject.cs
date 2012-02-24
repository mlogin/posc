using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PongFS.Physics;
using PongFS.SpriteSheet;
using PongFS.Core;

namespace PongFS.Drawable
{
    public class DrawableGameObject: DrawableGameComponent
    {
        public event EventHandler OnOutOfBounds;
        public event EventHandler OnReady;
        public SpriteBatch SpriteBatch { get; protected set; }
        protected Texture2D texture;
        public Vector2 InitialPosition { get; set; }
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
        protected Vector2 MaxSpeed { get; set; }
        public Vector2 Acceleration { get; set; }
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
        protected Random rnd = new Random();
        protected int screenWidth;
        protected int screenHeight;
        protected int width, height;
        protected string texKey, id;

        // animation
        protected bool animated = false;
        protected SpriteSheetLoader spriteSheetLoader;
        protected string currentAnimation;

        public DrawableGameObject(Game game, string key) : base(game)
        {
            this.texKey = key.Split('-')[0];
            this.id = key;
            ComponentFactory.getFactory().Add(key, this);
        }

        public override void Initialize()
        {
            screenWidth = Game.GraphicsDevice.PresentationParameters.BackBufferWidth;
            screenHeight = Game.GraphicsDevice.PresentationParameters.BackBufferHeight;
            Scaling = new Vector2(1f, 1f);
            ModColor = Color.White;
        }

        public virtual void LoadGraphics(SpriteBatch spriteBatch)
        {
            this.SpriteBatch = spriteBatch;
            SetTexture("images/" + texKey);
            base.LoadContent();
            if (OnReady != null) OnReady(this, null);
        }

        public override void Update(GameTime gameTime)
        {
            // update animation frame
            if (animated && spriteSheetLoader != null)
            {
                Frame frame = spriteSheetLoader.GetNextAnimationFrame();
                Rect = new Rectangle(Rect.X, Rect.Y, frame.rect.Width, frame.rect.Height);
            }

            // apply forces
            foreach (Force force in forces.FindAll(x => x.Enabled))
            {
                ApplyForce(force);
            }

            if (canMove)
            {
                Speed = new Vector2(MathHelper.Clamp(Speed.X + Acceleration.X, -MaxSpeed.X, MaxSpeed.X),
                                    MathHelper.Clamp(Speed.Y + Acceleration.Y, -MaxSpeed.Y, MaxSpeed.Y));
            }

            // bounderies
            if (Bounds.Width > 0 && Bounds.Height > 0 && !Bounds.Contains(Rect))
            {
                if (Rect.Left < Bounds.Left)
                {
                    Position = new Vector2(Bounds.Left, Position.Y);
                    if (BounceOffBounds) Speed = new Vector2(-Speed.X, Speed.Y);
                }
                else if (Rect.Right > Bounds.Right)
                {
                    Position = new Vector2(Bounds.Right - Width, Position.Y);
                    if (BounceOffBounds) Speed = new Vector2(-Speed.X, Speed.Y);
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
            Acceleration *= force.Acceleration;
            Speed *= force.Speed;
            if (Speed.LengthSquared() < 0.15f)
            {
                Speed = Vector2.Zero;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (animated && spriteSheetLoader != null)
            {
                SpriteBatch.Draw(texture, Position, spriteSheetLoader.GetCurrentAnimationFrame().rect, ModColor, Rotation, Vector2.Zero, Scaling, SpriteEffects.None, 0);
            }
            else
            {
                SpriteBatch.Draw(texture, Position, null, ModColor, Rotation, Vector2.Zero, Scaling, SpriteEffects.None, 0);
            }
        }

        protected void SetTexture(string texName)
        {
            texture = Game.Content.Load<Texture2D>(texName);
            if (animated)
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

        protected void PlaceCenterAt(int x, int y)
        {
            Position = new Vector2(x - Width / 2, y - Height / 2);
        }

        protected void setAnimation(string animationKey)
        {
            if (animated && spriteSheetLoader != null)
            {
                this.currentAnimation = animationKey;
                spriteSheetLoader.SetCurrentAnimation(animationKey);
            }
        }

    }
}
