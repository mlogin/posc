using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SkyBall.Entity;
using SkyBall.Drawable;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Media;
using SkyBall.Core;
using SkyBall.X2DPE;
using SkyBall.Config;


namespace SkyBall
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    class GameplayScreen : GameScreen
    {
        ContentManager content;
        SpriteFont gameFont;

        Random random = new Random();

        float pauseAlpha;

        public const int WIN_BORDER = 3;
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

        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }


        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            ComponentFactory.getFactory().Initialize(ScreenManager.Game);
            ParticleFactory.getFactory().Initialize(ScreenManager.Game);
            SoundFactory.getFactory().Initialize(ScreenManager.Game);
            currentTrackIndex = rnd.Next(NB_MUSIC_TRACKS);

            player1 = new Player(ScreenManager.Game, new KeyboardLayout(KeyboardLayout.Default.Arrows), "p1", Character.PlayerPosition.Top, "images/spark");
            player2 = new Player(ScreenManager.Game, "p2", Character.PlayerPosition.Bottom, "images/spark2");
            ball = new Ball(ScreenManager.Game, "ball");
            currentPowerUp = new PowerUp(ScreenManager.Game, "powerups-" + ComponentFactory.getFactory().NewId());
            graphics.PreferredBackBufferWidth = GameConfig.WIDTH;
            graphics.PreferredBackBufferHeight = GameConfig.HEIGHT;
            graphics.ApplyChanges();
            FrameRateCounter FrameRateCounter = new FrameRateCounter(ScreenManager.Game, "fonts/default");
            ScreenManager.Game.Components.Add(FrameRateCounter);

            gameFont = content.Load<SpriteFont>("fonts/default");

            spriteBatch = new SpriteBatch(ScreenManager.GraphicsDevice);
            background = ScreenManager.Game.Content.Load<Texture2D>("images/background");
            ball.LoadGraphics(spriteBatch);
            currentPowerUp.LoadGraphics(spriteBatch);
            for (int i = 1; i < 6; i++)
            {
                songs.Add(ScreenManager.Game.Content.Load<Song>("music/0" + i));
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

            VideoFont = ScreenManager.Game.Content.Load<SpriteFont>("fonts/default");

            screenWidth = GameConfig.WIDTH;
            screenHeight = GameConfig.HEIGHT;

            ScreenManager.Game.ResetElapsedTime();
        }


        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            content.Unload();
        }


        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

            if (IsActive)
            {
                UpdateMusic();
                rnd.Next();
                if (!currentPowerUp.Displayed && rnd.Next(PowerUp.SPAWN_RND) == 1)
                {
                    currentPowerUp.ReInitialize();
                }
                ParticleFactory.getFactory().Update(gameTime);

            }
        }

        private void UpdateMusic()
        {
            if (MediaPlayer.State.Equals(MediaState.Stopped))
            {
                MediaPlayer.Play(songs[currentTrackIndex]);
                currentTrackIndex = (currentTrackIndex + 1) % (songs.Count - 1);
            }
        }

        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
            GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected &&
                                       input.GamePadWasConnected[playerIndex];

            if (input.IsPauseGame(ControllingPlayer) || gamePadDisconnected)
            {
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }
            else
            {
                player1.HandleKeys(keyboardState);
                player2.HandleKeys(keyboardState);

            }
        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {

            ScreenManager.GraphicsDevice.Clear(Color.Black);

            // Our player and enemy are both actually just text strings.
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();
            DrawBackground();
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.Additive);
            ParticleFactory.getFactory().Draw(gameTime, spriteBatch);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            base.Draw(gameTime);
            spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }

        private void DrawBackground()
        {
            Rectangle screenRectangle = new Rectangle(0, 0, screenWidth, screenHeight);
            spriteBatch.Draw(background, screenRectangle, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 1);
        }
        
    }
}
