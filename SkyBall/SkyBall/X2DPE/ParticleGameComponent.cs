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


namespace X2DPE
{
	/// <summary>
	/// This is a game component that implements IUpdateable.
	/// </summary>
	public class ParticleComponent : DrawableGameComponent
	{
		private SpriteBatch spriteBatch;
		public List<Emitter> particleEmitterList;

		public ParticleComponent(Game game)
			: base(game)
		{
			// TODO: Construct any child components here
		}

		/// <summary>
		/// Allows the game component to perform any initialization it needs to before starting
		/// to run.  This is where it can query for any required services and load content.
		/// </summary>
		public override void Initialize()
		{
			particleEmitterList = new List<Emitter>();

			base.Initialize();
		}

		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(Game.GraphicsDevice);

			base.LoadContent();
		}

		protected override void UnloadContent()
		{
			base.UnloadContent();
		}

		/// <summary>
		/// Allows the game component to update itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		public override void Update(GameTime gameTime)
		{
			foreach (Emitter emitter in particleEmitterList)
			{
				emitter.UpdateParticles(gameTime);
			}
			base.Update(gameTime);
		}

		public override void Draw(GameTime gameTime)
		{
			spriteBatch.Begin();

			foreach (Emitter emitter in particleEmitterList)
			{
				emitter.DrawParticles(gameTime, spriteBatch);
			}
			
			spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
