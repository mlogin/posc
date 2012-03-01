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
        private Player player1, player2;
        private Ball ball;
        private Texture2D background;
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
            {
                content = new ContentManager(ScreenManager.Game.Services, "Content");
            }

            LoadTextures();
            LoadSounds();
            
            player1 = new Player("player1", Player.Side.Up, new KeyboardLayout(KeyboardLayout.Default.Arrows));
            player2 = new Player("player2", Player.Side.Down);
            player1.Opponent = player2;
            player2.Opponent = player1;
            ball = new Ball("ball");
            currentPowerUp = new PowerUp("powerup");

            FrameRateCounter FrameRateCounter = new FrameRateCounter(ScreenManager.Game, "fonts/default");
            ScreenManager.Game.Components.Add(FrameRateCounter);

            gameFont = content.Load<SpriteFont>("fonts/default");

            currentTrackIndex = rnd.Next(NB_MUSIC_TRACKS);
            for (int i = 1; i < 6; i++)
            {
                songs.Add(ScreenManager.Game.Content.Load<Song>("music/0" + i));
            }

            background = TextureFactory.getFactory().Get("background");
            VideoFont = ScreenManager.Game.Content.Load<SpriteFont>("fonts/default");

            ScreenManager.Game.ResetElapsedTime();
        }

        private void LoadSounds()
        {
            SoundFactory.getFactory().Initialize(content);
            SoundFactory.getFactory().AddSound("shot1", "fx/shot1", false);
            SoundFactory.getFactory().AddSound("shot2", "fx/shot2", false);
            SoundFactory.getFactory().AddSound("shot3", "fx/shot3", false);
            SoundFactory.getFactory().AddSound("powerup", "fx/powerup", false);
            SoundFactory.getFactory().AddSound("explosion", "fx/explosion", false);
            SoundFactory.getFactory().AddSound("shields", "fx/shields", false);
            SoundFactory.getFactory().AddSound("shieldhit", "fx/shieldhit", false);
            SoundFactory.getFactory().AddSound("charge", "fx/charge", true);
        }

        private void LoadTextures()
        {
            TextureFactory.getFactory().Initialize(content);
            TextureFactory.getFactory().Add("background", "images/background");
            TextureFactory.getFactory().Add("ball", "images/ball");
            TextureFactory.getFactory().Add("dots", "images/dots");
            TextureFactory.getFactory().Add("fire", "images/fire");
            TextureFactory.getFactory().Add("plasma", "images/plasma");
            TextureFactory.getFactory().Add("player", "images/player");
            TextureFactory.getFactory().Add("powerups", "images/powerups");
            TextureFactory.getFactory().Add("ray", "images/ray");
            TextureFactory.getFactory().Add("shadow", "images/shadow");
            TextureFactory.getFactory().Add("smoke", "images/smoke");
            TextureFactory.getFactory().Add("spark", "images/spark");
            TextureFactory.getFactory().Add("spark2", "images/spark2");
            TextureFactory.getFactory().Add("wall", "images/wall");
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
                ball.Update(gameTime);
                player1.Update(gameTime);
                player2.Update(gameTime);
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

            ScreenManager.SpriteBatch.Begin();
            DrawBackground();
            ScreenManager.SpriteBatch.End();

            ScreenManager.SpriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.Additive);
            ParticleFactory.getFactory().Draw(gameTime, ScreenManager.SpriteBatch);
            ScreenManager.SpriteBatch.End();

            ScreenManager.SpriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            ball.Draw(gameTime, ScreenManager.SpriteBatch);
            player1.Draw(gameTime, ScreenManager.SpriteBatch);
            player2.Draw(gameTime, ScreenManager.SpriteBatch);

            ScreenManager.SpriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }

        private void DrawBackground()
        {
            Rectangle screenRectangle = new Rectangle(0, 0, GameConfig.WIDTH, GameConfig.HEIGHT);
            ScreenManager.SpriteBatch.Draw(background, screenRectangle, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 1);
        }
        
    }
}
