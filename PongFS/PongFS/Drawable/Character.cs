using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PongFS.Config;
using PongFS.SpriteSheet;
using PongFS.Physics;
using PongFS.Core;
using X2DPE;

namespace PongFS.Drawable
{
    public class Character : DrawableGameObject
    {
        public enum PlayerPosition { Top, Bottom }

        private const float ACCEL_SPEED = 0.8f;
        private const float LIFT_STEP = 0.1f;
        public float MAX_LIFT = 3f;
        public int MAX_STRENGTH = 150;
        private const int MAX_AREA_HEIGHT = 175;
        private Emitter powerEmitter;

        public int Strength{get;set;}
        public float Lift { get; set; }
        private bool isHitting, hasHit;

        public PlayerPosition ScreenPosition {get;set;}

        public Character(Game game, string id) : base(game, id) { }

        public override void Initialize()
        {
            base.Initialize();
            InitParticleEmitters();
            animated = true;
            spriteSheetLoader = new SpriteSheetLoader(ScreenPosition == PlayerPosition.Top ? "facedown" : "faceup");
            spriteSheetLoader.OnAnimComplete += new EventHandler(spriteSheet_OnAnimComplete);
            setAnimation("idle");
            canMove = true;
            MaxSpeed = new Vector2(4f, 4f);
            forces.Add(new Force { Speed = new Vector2(0.85f, 0.85f) }); // friction
            OnReady += new EventHandler(Character_OnReady);
        }

        private void InitParticleEmitters()
        {
            powerEmitter = new Emitter();
            powerEmitter.Active = false;
            powerEmitter.TextureList.Add(Game.Content.Load<Texture2D>("images\\fire"));
            powerEmitter.RandomEmissionInterval = new RandomMinMax(10);
            powerEmitter.ParticleLifeTime = 1000;
            powerEmitter.ParticleDirection = new RandomMinMax(0);
            powerEmitter.ParticleSpeed = new RandomMinMax(0);
            powerEmitter.ParticleRotation = new RandomMinMax(360);
            powerEmitter.RotationSpeed = new RandomMinMax(0);
            powerEmitter.ParticleFader = new ParticleFader(true, true, 200);
            powerEmitter.ParticleScaler = new ParticleScaler(0.3f, 0.6f, 0, 500);
            powerEmitter.Position = new Vector2(140, 580);
            ParticleFactory.getFactory().Add("fire", powerEmitter);
        }

        void Character_OnReady(object sender, EventArgs e)
        {
            if (ScreenPosition == PlayerPosition.Top)
            {
                Position = new Vector2(screenWidth / 2, Height);
                Bounds = new Rectangle(Engine.WIN_BORDER * 2, 0, screenWidth - Width, MAX_AREA_HEIGHT);
            }
            else
            {
                Position = new Vector2(screenWidth / 2, screenHeight - 2 * Height);
                Bounds = new Rectangle(Engine.WIN_BORDER * 2, screenHeight - MAX_AREA_HEIGHT, screenWidth - Width, MAX_AREA_HEIGHT);
            } 
        }

        void spriteSheet_OnAnimComplete(object sender, EventArgs e)
        {
            string anim = (string)sender;
            if (anim == "hit_left" || anim == "hit_right")
            {
                isHitting = false;
                Lift = 0f;
                Strength = 0;
                Speed = Vector2.Zero;
                canMove = true;
            }
        }

        public void Fortify()
        {
            Strength += 1;
            if (Strength > MAX_STRENGTH) Strength = MAX_STRENGTH;
            canMove = false;
        }

        public void Hit()
        {
            if (!canMove && !isHitting)
            {
                isHitting = true;
                hasHit = false;
                setAnimation(currentAnimation == "prepare_hitleft" ? "hit_left" : "hit_right");
            }
        }
        
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (isHitting && !hasHit)
            {
                Ball ball = (Ball)ComponentFactory.getFactory().Get("ball");
                Rectangle hitRegion = new Rectangle(Rect.X - 5, Rect.Y, Rect.Width + 15, Rect.Height);
                if (hitRegion.Intersects(ball.Rect))
                {
                    hasHit = true;
                    int dir = ScreenPosition == Character.PlayerPosition.Top ? 1 : -1;
                    float speedX, speedY;
                    speedY = dir * (Math.Abs(ball.Speed.Y) * 2f / 3 + Strength / 10);
                    if (speedY <= 0 && speedY > -Ball.MIN_SPEED)
                    {
                        speedY = -Ball.MIN_SPEED;
                    }
                    else if (speedY >= 0 && speedY < Ball.MIN_SPEED)
                    {
                        speedY = Ball.MIN_SPEED;
                    }
                    speedX = MathHelper.Clamp(Lift, -MAX_LIFT, MAX_LIFT);
                    ball.Speed = new Vector2(speedX, speedY);
                }
            }
        }

        public void HandleKeys(KeyboardState keyboard, KeyboardLayout kb, Ball ball)
        {

            if (!isHitting)
            {
                if (keyboard.IsKeyDown(kb.KeyUp) && !keyboard.IsKeyDown(kb.KeyRight) && !keyboard.IsKeyDown(kb.KeyDown) && !keyboard.IsKeyDown(kb.KeyLeft) && !keyboard.IsKeyDown(kb.KeyAction) && currentAnimation != "run_up")
                {
                    setAnimation("run_up");
                }
                else if (keyboard.IsKeyDown(kb.KeyUp) && keyboard.IsKeyDown(kb.KeyRight) && !keyboard.IsKeyDown(kb.KeyDown) && !keyboard.IsKeyDown(kb.KeyLeft) && !keyboard.IsKeyDown(kb.KeyAction) && currentAnimation != "run_upright")
                {
                    setAnimation("run_upright");
                }
                else if (!keyboard.IsKeyDown(kb.KeyUp) && keyboard.IsKeyDown(kb.KeyRight) && !keyboard.IsKeyDown(kb.KeyDown) && !keyboard.IsKeyDown(kb.KeyLeft) && !keyboard.IsKeyDown(kb.KeyAction) && currentAnimation != "run_right")
                {
                    setAnimation("run_right");
                }
                else if (!keyboard.IsKeyDown(kb.KeyUp) && keyboard.IsKeyDown(kb.KeyRight) && keyboard.IsKeyDown(kb.KeyDown) && !keyboard.IsKeyDown(kb.KeyLeft) && !keyboard.IsKeyDown(kb.KeyAction) && currentAnimation != "run_downright")
                {
                    setAnimation("run_downright");
                }
                else if (!keyboard.IsKeyDown(kb.KeyUp) && !keyboard.IsKeyDown(kb.KeyRight) && keyboard.IsKeyDown(kb.KeyDown) && !keyboard.IsKeyDown(kb.KeyLeft) && !keyboard.IsKeyDown(kb.KeyAction) && currentAnimation != "run_down")
                {
                    setAnimation("run_down");
                }
                else if (!keyboard.IsKeyDown(kb.KeyUp) && !keyboard.IsKeyDown(kb.KeyRight) && keyboard.IsKeyDown(kb.KeyDown) && keyboard.IsKeyDown(kb.KeyLeft) && !keyboard.IsKeyDown(kb.KeyAction) && currentAnimation != "run_downleft")
                {
                    setAnimation("run_downleft");
                }
                else if (!keyboard.IsKeyDown(kb.KeyUp) && !keyboard.IsKeyDown(kb.KeyRight) && !keyboard.IsKeyDown(kb.KeyDown) && keyboard.IsKeyDown(kb.KeyLeft) && !keyboard.IsKeyDown(kb.KeyAction) && currentAnimation != "run_left")
                {
                    setAnimation("run_left");
                }
                else if (keyboard.IsKeyDown(kb.KeyUp) && !keyboard.IsKeyDown(kb.KeyRight) && !keyboard.IsKeyDown(kb.KeyDown) && keyboard.IsKeyDown(kb.KeyLeft) && !keyboard.IsKeyDown(kb.KeyAction) && currentAnimation != "run_upleft")
                {
                    setAnimation("run_upleft");
                }
                else if (!keyboard.IsKeyDown(kb.KeyUp) && !keyboard.IsKeyDown(kb.KeyRight) && !keyboard.IsKeyDown(kb.KeyDown) && !keyboard.IsKeyDown(kb.KeyLeft) && !keyboard.IsKeyDown(kb.KeyAction) && currentAnimation != "idle")
                {
                    setAnimation("idle");
                }

                if (canMove)
                {
                    if (keyboard.IsKeyDown(kb.KeyLeft))
                    {
                        Speed += new Vector2(-ACCEL_SPEED, 0);
                    }
                    if (keyboard.IsKeyDown(kb.KeyRight))
                    {
                        Speed += new Vector2(ACCEL_SPEED, 0);
                    }
                    if (keyboard.IsKeyDown(kb.KeyUp))
                    {
                        Speed += new Vector2(0, -ACCEL_SPEED);
                    }
                    if (keyboard.IsKeyDown(kb.KeyDown))
                    {
                        Speed += new Vector2(0, ACCEL_SPEED);
                    }
                }
                
                if (keyboard.IsKeyDown(kb.KeyAction))
                {
                    Fortify();
                    if (currentAnimation != "prepare_hitleft" && currentAnimation != "prepare_hitright")
                    {
                        if (keyboard.IsKeyDown(kb.KeyLeft))
                        {
                            setAnimation("prepare_hitleft");
                        }
                        else
                        {
                            setAnimation("prepare_hitright");
                        }
                        // particle system
                    }

                    if (keyboard.IsKeyDown(kb.KeyLeft))
                    {
                        Lift = MathHelper.Clamp(Lift - LIFT_STEP, -MAX_LIFT, MAX_LIFT);
                    } else if (keyboard.IsKeyDown(kb.KeyRight))
                    {
                        Lift = MathHelper.Clamp(Lift + LIFT_STEP, -MAX_LIFT, MAX_LIFT);
                    }
                }
                else if (keyboard.IsKeyUp(kb.KeyAction))
                {
                    Hit();
                }
            }
        }
    }
}
