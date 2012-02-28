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
using X2DPE.Helpers;
using PongFS.X2DPE;
using PongFS.Entity;

namespace PongFS.Drawable
{
    public class Character : DrawableGameObject
    {
        public enum PlayerPosition { Top, Bottom }

        private Vector2 INITIAL_MAX_SPEED = new Vector2(4f);
        private Vector2 INITIAL_ACCEL_SPEED = new Vector2(0.8f);
        private const int INITIAL_STRENGTH_GAINER = 1;
        private float strengthGainer;
        private const float LIFT_STEP = 0.1f;
        public float MAX_LIFT = 3f;
        public int MAX_STRENGTH = 150;
        private const int MAX_AREA_HEIGHT = 175;
        private Emitter powerEmitter, runEmitter, sparkEmitter;
        public float Strength{get;set;}
        public float Lift { get; set; }
        private bool isHitting, hasHit, isRunning, isAiming = true;
        private PlayerPosition playerPosition;
        private string powerTex;
        private Texture2D textureShadow;
        private bool isKeysReversed;
        public Power CurrentPower{get;set;}
        public bool IA { get; set; }
        public Character Opponent { get; set; }

        public PlayerPosition ScreenPosition {get;set;}

        public Character(Game game, string id) : base(game, id) { }

        public Character(Game game, string id, PlayerPosition playerPosition, string powerTex) : base(game, id)
        {
            this.playerPosition = playerPosition;
            this.powerTex = powerTex;
            strengthGainer = INITIAL_STRENGTH_GAINER;
            CurrentPower = Power.None;
        }

        public override void Initialize()
        {
            base.Initialize();
            InitParticleEmitters();
            animated = true;
            spriteSheetLoader = new SpriteSheetLoader(ScreenPosition == PlayerPosition.Top ? "facedown" : "faceup");
            spriteSheetLoader.OnAnimComplete += new EventHandler(spriteSheet_OnAnimComplete);
            setAnimation("idle");
            canMove = true;
            MaxSpeed = INITIAL_MAX_SPEED;
            MaxAcceleration = INITIAL_ACCEL_SPEED;
            forces.Add(Force.GroundFriction); // friction
            OnReady += new EventHandler(Character_OnReady);
        }

        private void InitParticleEmitters()
        {
            powerEmitter = new Emitter();
            powerEmitter.Active = false;
            powerEmitter.TextureList.Add(Game.Content.Load<Texture2D>(powerTex));
            powerEmitter.RandomEmissionInterval = new RandomMinMax(100);
            powerEmitter.ParticleLifeTime = 1000;
            powerEmitter.ParticleDirection = new RandomMinMax(60);
            powerEmitter.ParticleSpeed = new RandomMinMax(0.1);
            powerEmitter.ParticleRotation = new RandomMinMax(360);
            powerEmitter.RotationSpeed = new RandomMinMax(0.08);
            powerEmitter.ParticleFader = new ParticleFader(true, true, 20);
            powerEmitter.ParticleScaler = new ParticleScaler(0.3f, 30f, 0, 5000);
            powerEmitter.Position = Center;
            ParticleFactory.getFactory().Add("power-" + ScreenPosition.ToString(), powerEmitter);

            runEmitter = new Emitter();
            runEmitter.Active = true;
            runEmitter.TextureList.Add(Game.Content.Load<Texture2D>("images/ray"));
            runEmitter.RandomEmissionInterval = new RandomMinMax(15);
            runEmitter.ParticleLifeTime = 500;
            runEmitter.ParticleDirection = new RandomMinMax(0);
            runEmitter.ParticleSpeed = new RandomMinMax(0);
            runEmitter.ParticleRotation = new RandomMinMax(0);
            runEmitter.RotationSpeed = new RandomMinMax(0);
            runEmitter.ParticleFader = new ParticleFader(true, true, 100);
            runEmitter.ParticleScaler = new ParticleScaler(0.1f, 0.1f, 0, 10);
            runEmitter.Position = Center;
            ParticleFactory.getFactory().Add("run-" + ScreenPosition.ToString(), runEmitter);

            sparkEmitter = new Emitter();
            sparkEmitter.Active = false;
            sparkEmitter.TextureList.Add(Game.Content.Load<Texture2D>("images/dots"));
            sparkEmitter.RandomEmissionInterval = new RandomMinMax(200);
            sparkEmitter.ParticleLifeTime = 1500;
            sparkEmitter.ParticleDirection = new RandomMinMax(0.2);
            sparkEmitter.ParticleSpeed = new RandomMinMax(0.6);
            sparkEmitter.ParticleRotation = new RandomMinMax(60);
            sparkEmitter.RotationSpeed = new RandomMinMax(0.04);
            sparkEmitter.ParticleFader = new ParticleFader(false, true, 300);
            sparkEmitter.ParticleScaler = new ParticleScaler(1f,2.5f, 0, 10);
            sparkEmitter.Position = Center;
            ParticleFactory.getFactory().Add("shine-" + ScreenPosition.ToString(), sparkEmitter);

        }

        public override void LoadGraphics(SpriteBatch spriteBatch)
        {
            base.LoadGraphics(spriteBatch);
            textureShadow = Game.Content.Load<Texture2D>("images/shadow");
        }

        void Character_OnReady(object sender, EventArgs e)
        {
            if (ScreenPosition == PlayerPosition.Top)
            {
                Position = new Vector2(screenWidth / 2, Height);
                Bounds = new Rectangle(15, 0, screenWidth - Width , MAX_AREA_HEIGHT);
            }
            else
            {
                Position = new Vector2(screenWidth / 2, screenHeight - 2 * Height);
                Bounds = new Rectangle(Engine.WIN_BORDER * 2, screenHeight - MAX_AREA_HEIGHT, screenWidth - Width - Engine.WIN_BORDER * 2, MAX_AREA_HEIGHT);
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
                isAiming = false;
                if (IA)
                {
                    setAnimation("idle");
                }
            }
        }

        public void Aim()
        {
            if (IA)
            {
                Strength += strengthGainer / 2f;
                Lift += (Opponent.Center.X < screenWidth / 2 ? LIFT_STEP : -LIFT_STEP);
            }
            else
            {
                Strength += strengthGainer;
            }
            powerEmitter.RandomEmissionInterval = new RandomMinMax(MAX_STRENGTH + 10 - Strength);
            powerEmitter.ParticleScaler = new ParticleScaler(0.3f, Strength / 5, 0, 5000);
            if (Strength > MAX_STRENGTH)
            {
                Strength = MAX_STRENGTH;
                SoundFactory.getFactory().Play("charge", false);
            }
            canMove = false;
            isAiming = true;
            powerEmitter.Active = true;
        }

        public void Hit()
        {
            if (!canMove && !isHitting)
            {
                isHitting = true;
                hasHit = false;
                setAnimation(currentAnimation == "prepare_hitleft" ? "hit_left" : "hit_right");
                powerEmitter.Active = false;
                SoundFactory.getFactory().Stop("charge");
            }
        }
        
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Ball ball = (Ball)ComponentFactory.getFactory().Get("ball");

            if (IA)
            {
                if (ball.Rect.Intersects(Rect))
                {
                    Hit();
                }
            }

            if (CurrentPower.type != Power.PowerType.None)
            {

                if (gameTime.TotalGameTime.Subtract(CurrentPower.acquired).TotalSeconds > PowerUp.TIME_ACTIVE)
                {
                    CurrentPower = Power.None;
                    isKeysReversed = false;
                    forces.Clear();
                    forces.Add(Force.GroundFriction);
                    MaxSpeed = INITIAL_MAX_SPEED;
                    strengthGainer = INITIAL_STRENGTH_GAINER;
                    ball.IsCrazy = false;
                    sparkEmitter.Active = false;
                }
                else
                {
                    sparkEmitter.Active = true;
                    switch (CurrentPower.type)
                    {
                        case Power.PowerType.ReverseKeys:
                            isKeysReversed = true;
                            sparkEmitter.ModColor = Color.Red;
                            break;
                        case Power.PowerType.Ice:
                            forces.Clear();
                            forces.Add(Force.IceFriction);
                            sparkEmitter.ModColor = Color.Orange;
                            break;
                        case Power.PowerType.Speed:
                            MaxSpeed = INITIAL_MAX_SPEED * 2;
                            MaxAcceleration = INITIAL_ACCEL_SPEED * 2;
                            forces.Clear();
                            forces.Add(Force.HighSpeedFriction);
                            sparkEmitter.ModColor = Color.DarkGray;
                            break;
                        case Power.PowerType.Power:
                            strengthGainer = INITIAL_STRENGTH_GAINER * 2;
                            sparkEmitter.ModColor = Color.Green;
                            break;
                        case Power.PowerType.Bend:
                            ball.IsCrazy = true;
                            sparkEmitter.ModColor = Color.LightGray;
                            break;
                    }
                }
            }
            powerEmitter.Position = Center;
            runEmitter.Position = Center;
            sparkEmitter.Position = Center;
            isRunning = Speed.LengthSquared() > 0.1f;
            if (isRunning && !runEmitter.Active)
            {
                runEmitter.Active = true;
            }
            else if (!isRunning && runEmitter.Active)
            {
                runEmitter.Active = false;
            }

            if (isHitting && !hasHit)
            {
                Rectangle hitRegion = new Rectangle(Rect.X - 5, Rect.Y, Rect.Width + 15, Rect.Height);
                if (hitRegion.Intersects(ball.Rect))
                {
                    hasHit = true;
                    ball.Speed = GetAimDirection(ball);
                    if (ball.Speed.LengthSquared() < Ball.FIRE_SPEED)
                    {
                        SoundFactory.getFactory().Play("shot1", true);
                    }
                    else if (ball.Speed.LengthSquared() > Ball.FIRE_SPEED && ball.Speed.LengthSquared() < Ball.PLASMA_SPEED)
                    {
                        SoundFactory.getFactory().Play("shot2", true);
                    }
                    else
                    {
                        SoundFactory.getFactory().Play("shot3", true);
                    }
                }
            }
        }

        private Vector2 GetAimDirection(Ball ball)
        {
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
            return new Vector2(speedX, speedY);
        }

        public override void Draw(GameTime gameTime)
        {
            if (CurrentPower.type == Power.PowerType.RevealAim && isAiming)
            {
                Primitives2d prim = new Primitives2d(Game.GraphicsDevice, SpriteBatch);
                Ball ball = (Ball)ComponentFactory.getFactory().Get("ball");
                Vector2 Direction = GetAimDirection(ball);
                prim.DrawLine((int)Center.X, (int)Center.Y, (int)(Center.X + Direction.X * 300), (int) (Center.Y + Direction.Y * 300), new Color(1f, 0, 0, 0.2f));
            }
            Vector2 shadowPosition = new Vector2(Position.X - Width/2, Position.Y + 24);
            base.Draw(gameTime);
            SpriteBatch.Draw(textureShadow, shadowPosition, null, Color.White, 0, Vector2.Zero, new Vector2((float)2 * Width / textureShadow.Width, 0.8f), SpriteEffects.None, 0.9f);

        }


        public void HandleKeys(KeyboardState keyboard, KeyboardLayout kb)
        {
            KeyboardLayout k = isKeysReversed ? kb.Reversed() : kb;

            if (!isHitting)
            {
                if (keyboard.IsKeyDown(k.KeyUp) && !keyboard.IsKeyDown(k.KeyRight) && !keyboard.IsKeyDown(k.KeyDown) && !keyboard.IsKeyDown(k.KeyLeft) && !keyboard.IsKeyDown(k.KeyAction) && currentAnimation != "run_up")
                {
                    setAnimation("run_up");
                }
                else if (keyboard.IsKeyDown(k.KeyUp) && keyboard.IsKeyDown(k.KeyRight) && !keyboard.IsKeyDown(k.KeyDown) && !keyboard.IsKeyDown(k.KeyLeft) && !keyboard.IsKeyDown(k.KeyAction) && currentAnimation != "run_upright")
                {
                    setAnimation("run_upright");
                }
                else if (!keyboard.IsKeyDown(k.KeyUp) && keyboard.IsKeyDown(k.KeyRight) && !keyboard.IsKeyDown(k.KeyDown) && !keyboard.IsKeyDown(k.KeyLeft) && !keyboard.IsKeyDown(k.KeyAction) && currentAnimation != "run_right")
                {
                    setAnimation("run_right");
                }
                else if (!keyboard.IsKeyDown(k.KeyUp) && keyboard.IsKeyDown(k.KeyRight) && keyboard.IsKeyDown(k.KeyDown) && !keyboard.IsKeyDown(k.KeyLeft) && !keyboard.IsKeyDown(k.KeyAction) && currentAnimation != "run_downright")
                {
                    setAnimation("run_downright");
                }
                else if (!keyboard.IsKeyDown(k.KeyUp) && !keyboard.IsKeyDown(k.KeyRight) && keyboard.IsKeyDown(k.KeyDown) && !keyboard.IsKeyDown(k.KeyLeft) && !keyboard.IsKeyDown(k.KeyAction) && currentAnimation != "run_down")
                {
                    setAnimation("run_down");
                }
                else if (!keyboard.IsKeyDown(k.KeyUp) && !keyboard.IsKeyDown(k.KeyRight) && keyboard.IsKeyDown(k.KeyDown) && keyboard.IsKeyDown(k.KeyLeft) && !keyboard.IsKeyDown(k.KeyAction) && currentAnimation != "run_downleft")
                {
                    setAnimation("run_downleft");
                }
                else if (!keyboard.IsKeyDown(k.KeyUp) && !keyboard.IsKeyDown(k.KeyRight) && !keyboard.IsKeyDown(k.KeyDown) && keyboard.IsKeyDown(k.KeyLeft) && !keyboard.IsKeyDown(k.KeyAction) && currentAnimation != "run_left")
                {
                    setAnimation("run_left");
                }
                else if (keyboard.IsKeyDown(k.KeyUp) && !keyboard.IsKeyDown(k.KeyRight) && !keyboard.IsKeyDown(k.KeyDown) && keyboard.IsKeyDown(k.KeyLeft) && !keyboard.IsKeyDown(k.KeyAction) && currentAnimation != "run_upleft")
                {
                    setAnimation("run_upleft");
                }
                else if (!keyboard.IsKeyDown(k.KeyUp) && !keyboard.IsKeyDown(k.KeyRight) && !keyboard.IsKeyDown(k.KeyDown) && !keyboard.IsKeyDown(k.KeyLeft) && !keyboard.IsKeyDown(k.KeyAction) && currentAnimation != "idle")
                {
                    setAnimation("idle");
                }

                if (canMove)
                {
                    if (keyboard.IsKeyDown(k.KeyLeft))
                    {
                        Speed += new Vector2(-MaxAcceleration.X, 0);
                    }
                    if (keyboard.IsKeyDown(k.KeyRight))
                    {
                        Speed += new Vector2(MaxAcceleration.X, 0);
                    }
                    if (keyboard.IsKeyDown(k.KeyUp))
                    {
                        Speed += new Vector2(0, -MaxAcceleration.Y);
                    }
                    if (keyboard.IsKeyDown(k.KeyDown))
                    {
                        Speed += new Vector2(0, MaxAcceleration.Y);
                    }
                }
                
                if (keyboard.IsKeyDown(k.KeyAction))
                {
                    Aim();
                    if (currentAnimation != "prepare_hitleft" && currentAnimation != "prepare_hitright")
                    {
                        if (keyboard.IsKeyDown(k.KeyLeft))
                        {
                            setAnimation("prepare_hitleft");
                        }
                        else
                        {
                            setAnimation("prepare_hitright");
                        }
                    }

                    if (keyboard.IsKeyDown(k.KeyLeft))
                    {
                        Lift = MathHelper.Clamp(Lift - LIFT_STEP, -MAX_LIFT, MAX_LIFT);
                    } else if (keyboard.IsKeyDown(k.KeyRight))
                    {
                        Lift = MathHelper.Clamp(Lift + LIFT_STEP, -MAX_LIFT, MAX_LIFT);
                    }
                }
                else if (keyboard.IsKeyUp(k.KeyAction))
                {
                    Hit();
                }
            }
        }

        internal void HandleIA()
        {
            Ball ball = (Ball)ComponentFactory.getFactory().Get("ball");
            if ((ball.Speed.Y < 0 && playerPosition == Character.PlayerPosition.Bottom) ||
                (ball.Speed.Y > 0 && playerPosition == Character.PlayerPosition.Top))
            {
                // the ball is leaving, go back to middle
                Rectangle safeZone = new Rectangle(Bounds.Width / 2 - 40, Bounds.Y, 80, Bounds.Height);
                if (!Rect.Intersects(safeZone))
                {
                    int dir = Rect.X < safeZone.X ? 1 : -1;
                    Speed += new Vector2(dir * MaxAcceleration.X, 0);
                }

            }
            else // ball is coming back towards us, go meet it
            {
                float ballYDistance = Math.Abs(ball.Position.Y - Position.Y);
                float stepsToHorizPlane = ballYDistance / Math.Abs(ball.Speed.Y);
                float distXMadeByBall = ball.Speed.X * stepsToHorizPlane;
                float ballXPosHorizPlane = ball.Position.X + distXMadeByBall;
                float hitMinX = ballXPosHorizPlane - 10;
                float hitMaxX = ballXPosHorizPlane + 10;
                if ((Position.X < hitMinX || Position.X > hitMaxX) && Position.X + Speed.X < screenWidth - 40)
                {
                    int dir = Position.X < hitMinX ? 1 : -1;
                    Speed += new Vector2(dir * MaxAcceleration.X, 0);
                }
                else
                {
                    if(ballXPosHorizPlane > 0 && ballXPosHorizPlane < screenWidth) Aim();
                    if (currentAnimation != "prepare_hitleft" && currentAnimation != "prepare_hitright")
                    {
                        if (ball.Position.X < Position.X)
                        {
                            setAnimation("prepare_hitleft");
                        }
                        else
                        {
                            setAnimation("prepare_hitright");
                        }
                    }
                }
            }
        }

    }
}
