using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using PongFS.Config;
using PongFS.Drawable;
using PongFS.Core;
//using PongFS.Particles;
using PongFS.Entity;
using PongFS.X2DPE;

namespace PongFS
{
    public class Engine : Microsoft.Xna.Framework.Game
    {
        public const int WIN_BORDER = 3;
        public const int WIDTH = 500;
        public const int HEIGHT = 700;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Player player1, player2;
        Ball ball;
        Texture2D background;
        int screenWidth, screenHeight;
        SpriteFont VideoFont;


        public Engine()
        {
            graphics = new GraphicsDeviceManager(this);
            ComponentFactory.getFactory().Initialize(this);
            ParticleFactory.getFactory().Initialize(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            player1 = new Player(this, new KeyboardLayout(KeyboardLayout.Default.Arrows), "player 1", Character.PlayerPosition.Top, "images/spark");
            player2 = new Player(this, "player 2", Character.PlayerPosition.Bottom, "images/spark2");
            ball = new Ball(this, "ball");
            graphics.PreferredBackBufferWidth = WIDTH;
            graphics.PreferredBackBufferHeight = HEIGHT;
            graphics.ApplyChanges();
            Window.Title = "PongFS!";
            FrameRateCounter FrameRateCounter = new FrameRateCounter(this, "fonts/default");
            this.Components.Add(FrameRateCounter);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            background = Content.Load<Texture2D>("images/background");
            ball.LoadGraphics(spriteBatch);
            for (int i = 0; i < player1.Walls.Count; i++)
            {
                player1.Walls[i].LoadGraphics(spriteBatch);
            }
            for (int i = 0; i < player2.Walls.Count; i++)
            {
                player2.Walls[i].LoadGraphics(spriteBatch);
            }
            player1.Sprite.LoadGraphics(spriteBatch);
            player2.Sprite.LoadGraphics(spriteBatch);
            VideoFont = Content.Load<SpriteFont>("fonts/default");
            
            screenWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
            screenHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboard = Keyboard.GetState();
            player1.HandleKeys(keyboard, ball);
            player2.HandleKeys(keyboard, ball);
            base.Update(gameTime);
            ParticleFactory.getFactory().Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            DrawBackground();
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.Additive);
            ParticleFactory.getFactory().Draw(gameTime, spriteBatch);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            base.Draw(gameTime);
            spriteBatch.End();

        }

        private void DrawBackground()
        {
            Rectangle screenRectangle = new Rectangle(0, 0, screenWidth, screenHeight);
            spriteBatch.Draw(background, screenRectangle, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 1);
        }
    }
}
