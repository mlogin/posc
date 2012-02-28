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
using PongFS.Entity;
using PongFS.X2DPE;

namespace PongFS
{
    public class Engine : Microsoft.Xna.Framework.Game
    {
        public const int WIN_BORDER = 3;
        public const int WIDTH = 500;
        public const int HEIGHT = 700;

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Player player1, player2;
        private Ball ball;
        private Texture2D background;
        private int screenWidth, screenHeight;
        private SpriteFont VideoFont;
        private PowerUp currentPowerUp;
        private Random rnd = new Random();
        private List<Song> songs = new List<Song>();
        private int currentTrackIndex;
        private const int NB_MUSIC_TRACKS = 5;

        public Engine()
        {
            graphics = new GraphicsDeviceManager(this);
            ComponentFactory.getFactory().Initialize(this);
            ParticleFactory.getFactory().Initialize(this);
            SoundFactory.getFactory().Initialize(this);
            Content.RootDirectory = "Content";
            currentTrackIndex = rnd.Next(NB_MUSIC_TRACKS);
        }

        protected override void Initialize()
        {
            player1 = new Player(this, new KeyboardLayout(KeyboardLayout.Default.Arrows), "p1", Character.PlayerPosition.Top, "images/spark");
            player2 = new Player(this, "p2", Character.PlayerPosition.Bottom, "images/spark2");
            ball = new Ball(this, "ball");
            currentPowerUp = new PowerUp(this, "powerups-" + ComponentFactory.getFactory().NewId());
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
            currentPowerUp.LoadGraphics(spriteBatch);
            for (int i = 1; i < 6; i++)
            {
                songs.Add(Content.Load<Song>("music/0" + i));
            }
            SoundFactory.getFactory().Add("shot1", false);
            SoundFactory.getFactory().Add("shot2", false);
            SoundFactory.getFactory().Add("shot3", false);
            SoundFactory.getFactory().Add("powerup", false);
            SoundFactory.getFactory().Add("explosion", false);
            SoundFactory.getFactory().Add("shields", false);
            SoundFactory.getFactory().Add("shieldhit", false);
            SoundFactory.getFactory().Add("charge", true);
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
            player1.SetOpponent(player2);
            player2.SetOpponent(player1);

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
            rnd.Next();
            player1.HandleKeys(keyboard);
            player2.HandleKeys(keyboard);
            UpdateMusic();
            base.Update(gameTime);
            if (!currentPowerUp.Displayed && rnd.Next(PowerUp.SPAWN_RND) == 1)
            {
                currentPowerUp.ReInitialize();
            }
            ParticleFactory.getFactory().Update(gameTime);
        }

        private void UpdateMusic()
        {
            if (MediaPlayer.State.Equals(MediaState.Stopped))
            {
                MediaPlayer.Play(songs[currentTrackIndex]);
                currentTrackIndex = (currentTrackIndex + 1) % (songs.Count - 1);
            }
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
