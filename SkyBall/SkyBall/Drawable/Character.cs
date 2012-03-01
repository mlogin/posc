using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SkyBall.Config;
using SkyBall.SpriteSheet;
using SkyBall.Physics;
using SkyBall.Core;
using X2DPE;
using X2DPE.Helpers;
using SkyBall.X2DPE;
using SkyBall.Entity;

namespace SkyBall.Drawable
{
    public class Character : DrawableGameObject
    {
        private float maxSpeed = 4f;
        private float maxAccel = 0.8f;

        public float Strength { get; set; }
        private float minStrengthStep = 1f;
        private float currentStrength;
        private float maxStrength = 150f;

        public float Lift { get; set; }
        private float liftStep = 0.1f;
        private float maxLift = 3f;

        private const int MAX_AREA_HEIGHT = 175;

        private Texture2D shadow;
        private Emitter powerEmitter, runEmitter, sparkEmitter;
        public Power CurrentPower { get; set; }
        private bool isHitting, hasHit, isRunning, isAiming, isKeysReversed;
        public Player Player { get; set; }

        public Character(Player player, Texture2D texture) : 
            base(null, texture, true, new SpriteSheetLoader(player.Placement == Player.Side.Up ? "facedown" : "faceup", "idle"))
        {
            Player = player;
            currentStrength = minStrengthStep;
            CurrentPower = Power.None;
            spriteSheetLoader.OnAnimComplete += new EventHandler(spriteSheet_OnAnimComplete);
            SetPosition();
            canMove = true;
            MaxSpeed = maxSpeed;
            MaxAcceleration = maxAccel;
            forces.Add(Force.GroundFriction);
            InitParticleEmitters();
            shadow = TextureFactory.getFactory().Get("shadow");
        }

        private void InitParticleEmitters()
        {
            powerEmitter = new Emitter
            {
                Active = false,
                RandomEmissionInterval = new RandomMinMax(100),
                ParticleLifeTime = 1000,
                ParticleDirection = new RandomMinMax(60),
                ParticleSpeed = new RandomMinMax(0.1),
                ParticleRotation = new RandomMinMax(360),
                RotationSpeed = new RandomMinMax(0.08),
                ParticleFader = new ParticleFader(true, true, 20),
                ParticleScaler = new ParticleScaler(0.3f, 30f, 0, 5000),
                Position = Center
            };
            powerEmitter.TextureList.Add(TextureFactory.getFactory().Get("spark"));
            ParticleFactory.getFactory().Add(powerEmitter);

            runEmitter = new Emitter
            {
                Active = true,
                RandomEmissionInterval = new RandomMinMax(15),
                ParticleLifeTime = 500,
                ParticleDirection = new RandomMinMax(0),
                ParticleSpeed = new RandomMinMax(0),
                ParticleRotation = new RandomMinMax(0),
                RotationSpeed = new RandomMinMax(0),
                ParticleFader = new ParticleFader(true, true, 100),
                ParticleScaler = new ParticleScaler(0.1f, 0.1f, 0, 10),
                Position = Center
            };
            runEmitter.TextureList.Add(TextureFactory.getFactory().Get("ray"));
            ParticleFactory.getFactory().Add(runEmitter);

            sparkEmitter = new Emitter
            {
                Active = false,
                RandomEmissionInterval = new RandomMinMax(200),
                ParticleLifeTime = 1500,
                ParticleDirection = new RandomMinMax(0.2),
                ParticleSpeed = new RandomMinMax(0.6),
                ParticleRotation = new RandomMinMax(60),
                RotationSpeed = new RandomMinMax(0.04),
                ParticleFader = new ParticleFader(false, true, 300),
                ParticleScaler = new ParticleScaler(1f, 2.5f, 0, 10),
                Position = Center
            };
            sparkEmitter.TextureList.Add(TextureFactory.getFactory().Get("dots"));
            ParticleFactory.getFactory().Add(sparkEmitter);

        }

        private void SetPosition()
        {
            if (Player.Placement == Player.Side.Up)
            {
                PlaceCenterAt(GameConfig.WIDTH / 2, Height);
                Bounds = new Rectangle(15, 0, GameConfig.WIDTH - Width , MAX_AREA_HEIGHT);
            }
            else
            {
                PlaceCenterAt(GameConfig.WIDTH / 2, GameConfig.HEIGHT - 2 * Height);
                Bounds = new Rectangle(GameConfig.WIN_BORDER * 2, GameConfig.HEIGHT - MAX_AREA_HEIGHT, GameConfig.WIDTH - Width - GameConfig.WIN_BORDER * 2, MAX_AREA_HEIGHT);
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
                if (Player.IsNPC)
                {
                    SetAnimation("idle");
                }
            }
        }

        public void Aim()
        {
            if (Player.IsNPC)
            {
                Strength += currentStrength / 2f;
                Lift += (Player.Opponent.Sprite.Center.X < GameConfig.WIDTH / 2 ? liftStep : -liftStep);
            }
            else
            {
                Strength += currentStrength;
            }
            powerEmitter.RandomEmissionInterval = new RandomMinMax(maxStrength + 10 - Strength);
            powerEmitter.ParticleScaler = new ParticleScaler(0.3f, Strength / 5, 0, 5000);
            if (Strength > maxStrength)
            {
                Strength = maxStrength;
                SoundFactory.getFactory().PlaySound("charge");
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
                SetAnimation(currentAnimation == "prepare_hitleft" ? "hit_left" : "hit_right");
                powerEmitter.Active = false;
                SoundFactory.getFactory().Stop("charge");
            }
        }
        
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Ball ball = (Ball)ComponentFactory.getFactory().Get("ball");

            if (Player.IsNPC)
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
                    MaxSpeed = maxSpeed;
                    currentStrength = minStrengthStep;
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
                            MaxSpeed = maxSpeed * 2;
                            MaxAcceleration = maxAccel * 2;
                            forces.Clear();
                            forces.Add(Force.HighSpeedFriction);
                            sparkEmitter.ModColor = Color.DarkGray;
                            break;
                        case Power.PowerType.Power:
                            currentStrength = minStrengthStep * 2;
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
                        SoundFactory.getFactory().PlaySound("shot1");
                    }
                    else if (ball.Speed.LengthSquared() > Ball.FIRE_SPEED && ball.Speed.LengthSquared() < Ball.PLASMA_SPEED)
                    {
                        SoundFactory.getFactory().PlaySound("shot2");
                    }
                    else
                    {
                        SoundFactory.getFactory().PlaySound("shot3");
                    }
                }
            }
        }

        private Vector2 GetAimDirection(Ball ball)
        {
            int dir = Player.Placement == Player.Side.Up ? 1 : -1;
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
            speedX = MathHelper.Clamp(Lift, -maxLift, maxLift);
            return new Vector2(speedX, speedY);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //if (CurrentPower.type == Power.PowerType.RevealAim && isAiming)
            //{
            //    Primitives2d prim = new Primitives2d(Game.GraphicsDevice, spriteBatch);
            //    Ball ball = (Ball)ComponentFactory.getFactory().Get("ball");
            //    Vector2 Direction = GetAimDirection(ball);
            //    prim.DrawLine((int)Center.X, (int)Center.Y, (int)(Center.X + Direction.X * 300), (int) (Center.Y + Direction.Y * 300), new Color(1f, 0, 0, 0.2f));
            //}
            Vector2 shadowPosition = new Vector2(Position.X - Width/2, Position.Y + 24);
            base.Draw(gameTime, spriteBatch);
            spriteBatch.Draw(shadow, shadowPosition, null, Color.White, 0, Vector2.Zero, new Vector2((float)2 * Width / shadow.Width, 0.8f), SpriteEffects.None, 0.9f);

        }


        public void HandleKeys(KeyboardState keyboard, KeyboardLayout kb)
        {
            KeyboardLayout k = isKeysReversed ? kb.Reversed() : kb;

            if (!isHitting)
            {
                if (keyboard.IsKeyDown(k.KeyUp) && !keyboard.IsKeyDown(k.KeyRight) && !keyboard.IsKeyDown(k.KeyDown) && !keyboard.IsKeyDown(k.KeyLeft) && !keyboard.IsKeyDown(k.KeyAction) && currentAnimation != "run_up")
                {
                    SetAnimation("run_up");
                }
                else if (keyboard.IsKeyDown(k.KeyUp) && keyboard.IsKeyDown(k.KeyRight) && !keyboard.IsKeyDown(k.KeyDown) && !keyboard.IsKeyDown(k.KeyLeft) && !keyboard.IsKeyDown(k.KeyAction) && currentAnimation != "run_upright")
                {
                    SetAnimation("run_upright");
                }
                else if (!keyboard.IsKeyDown(k.KeyUp) && keyboard.IsKeyDown(k.KeyRight) && !keyboard.IsKeyDown(k.KeyDown) && !keyboard.IsKeyDown(k.KeyLeft) && !keyboard.IsKeyDown(k.KeyAction) && currentAnimation != "run_right")
                {
                    SetAnimation("run_right");
                }
                else if (!keyboard.IsKeyDown(k.KeyUp) && keyboard.IsKeyDown(k.KeyRight) && keyboard.IsKeyDown(k.KeyDown) && !keyboard.IsKeyDown(k.KeyLeft) && !keyboard.IsKeyDown(k.KeyAction) && currentAnimation != "run_downright")
                {
                    SetAnimation("run_downright");
                }
                else if (!keyboard.IsKeyDown(k.KeyUp) && !keyboard.IsKeyDown(k.KeyRight) && keyboard.IsKeyDown(k.KeyDown) && !keyboard.IsKeyDown(k.KeyLeft) && !keyboard.IsKeyDown(k.KeyAction) && currentAnimation != "run_down")
                {
                    SetAnimation("run_down");
                }
                else if (!keyboard.IsKeyDown(k.KeyUp) && !keyboard.IsKeyDown(k.KeyRight) && keyboard.IsKeyDown(k.KeyDown) && keyboard.IsKeyDown(k.KeyLeft) && !keyboard.IsKeyDown(k.KeyAction) && currentAnimation != "run_downleft")
                {
                    SetAnimation("run_downleft");
                }
                else if (!keyboard.IsKeyDown(k.KeyUp) && !keyboard.IsKeyDown(k.KeyRight) && !keyboard.IsKeyDown(k.KeyDown) && keyboard.IsKeyDown(k.KeyLeft) && !keyboard.IsKeyDown(k.KeyAction) && currentAnimation != "run_left")
                {
                    SetAnimation("run_left");
                }
                else if (keyboard.IsKeyDown(k.KeyUp) && !keyboard.IsKeyDown(k.KeyRight) && !keyboard.IsKeyDown(k.KeyDown) && keyboard.IsKeyDown(k.KeyLeft) && !keyboard.IsKeyDown(k.KeyAction) && currentAnimation != "run_upleft")
                {
                    SetAnimation("run_upleft");
                }
                else if (!keyboard.IsKeyDown(k.KeyUp) && !keyboard.IsKeyDown(k.KeyRight) && !keyboard.IsKeyDown(k.KeyDown) && !keyboard.IsKeyDown(k.KeyLeft) && !keyboard.IsKeyDown(k.KeyAction) && currentAnimation != "idle")
                {
                    SetAnimation("idle");
                }

                if (canMove)
                {
                    if (keyboard.IsKeyDown(k.KeyLeft))
                    {
                        Speed += new Vector2(-MaxAcceleration, 0);
                    }
                    if (keyboard.IsKeyDown(k.KeyRight))
                    {
                        Speed += new Vector2(MaxAcceleration, 0);
                    }
                    if (keyboard.IsKeyDown(k.KeyUp))
                    {
                        Speed += new Vector2(0, -MaxAcceleration);
                    }
                    if (keyboard.IsKeyDown(k.KeyDown))
                    {
                        Speed += new Vector2(0, MaxAcceleration);
                    }
                }
                
                if (keyboard.IsKeyDown(k.KeyAction))
                {
                    Aim();
                    if (currentAnimation != "prepare_hitleft" && currentAnimation != "prepare_hitright")
                    {
                        if (keyboard.IsKeyDown(k.KeyLeft))
                        {
                            SetAnimation("prepare_hitleft");
                        }
                        else
                        {
                            SetAnimation("prepare_hitright");
                        }
                    }

                    if (keyboard.IsKeyDown(k.KeyLeft))
                    {
                        Lift = MathHelper.Clamp(Lift - liftStep, -maxLift, maxLift);
                    } else if (keyboard.IsKeyDown(k.KeyRight))
                    {
                        Lift = MathHelper.Clamp(Lift + liftStep, -maxLift, maxLift);
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
            if ((ball.Speed.Y < 0 && Player.Placement == Entity.Player.Side.Down) ||
                (ball.Speed.Y > 0 && Player.Placement == Entity.Player.Side.Up))
            {
                // the ball is leaving, go back to middle
                Rectangle safeZone = new Rectangle(Bounds.Width / 2 - 40, Bounds.Y, 80, Bounds.Height);
                if (!Rect.Intersects(safeZone))
                {
                    int dir = Rect.X < safeZone.X ? 1 : -1;
                    Speed += new Vector2(dir * MaxAcceleration, 0);
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
                if ((Position.X < hitMinX || Position.X > hitMaxX) && Position.X + Speed.X < GameConfig.WIDTH - 40)
                {
                    int dir = Position.X < hitMinX ? 1 : -1;
                    Speed += new Vector2(dir * MaxAcceleration, 0);
                }
                else
                {
                    if (ballXPosHorizPlane > 0 && ballXPosHorizPlane < GameConfig.WIDTH) Aim();
                    if (currentAnimation != "prepare_hitleft" && currentAnimation != "prepare_hitright")
                    {
                        if (ball.Position.X < Position.X)
                        {
                            SetAnimation("prepare_hitleft");
                        }
                        else
                        {
                            SetAnimation("prepare_hitright");
                        }
                    }
                }
            }
        }

    }
}
