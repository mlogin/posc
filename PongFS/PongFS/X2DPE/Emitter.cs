using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using X2DPE.Helpers;

namespace X2DPE
{
	public class Emitter
	{
		// TODO: multiple emitters per loop !!
		EmitterHelper emitterHelper = new EmitterHelper();

		public List<Particle> ParticleList { get; set; }
		public bool EmittedNewParticle { get; set; }
		public Particle LastEmittedParticle { get; set; }
		public List<Texture2D> TextureList { get; set; }
		public bool Active { get; set; }
		public int ParticleLifeTime { get; set; }
		public Vector2 Position { get; set; }
		public RandomMinMax ParticleDirection { get; set; }	// in degrees (0-359)
		public RandomMinMax ParticleSpeed { get; set; }	// in ms
		public RandomMinMax RandomEmissionInterval { get; set; }	// in ms
		public RandomMinMax ParticleRotation { get; set; }
		public RandomMinMax RotationSpeed { get; set; }
		public ParticleFader ParticleFader { get; set; } // Fader settings
		public ParticleScaler ParticleScaler { get; set; } // scale settings
		public int Opacity { get; set; }

		private int i = 0;
		private double emitterFrequency = 0;	// in ms
		private double timeSinceLastEmission = 0;

		public Emitter()
		{
			Active = true;
			ParticleList = new List<Particle>();
			TextureList = new List<Texture2D>();
			Opacity = 255;
		}

		public void UpdateParticles(GameTime gameTime)
		{
			EmittedNewParticle = false;
			if (gameTime.ElapsedGameTime.TotalMilliseconds > 0)
			{
				if (Active)
				{
					timeSinceLastEmission += gameTime.ElapsedGameTime.Milliseconds;

					if (emitterFrequency == 0 || timeSinceLastEmission >= emitterFrequency)
					{
						emitterFrequency = emitterHelper.RandomizedDouble(RandomEmissionInterval);
						if (emitterFrequency == 0)
						{
							throw new Exception("emitter frequency cannot be below 0.1d !!");
						}
						for (int i = 0; i < Math.Round(timeSinceLastEmission / emitterFrequency); i++)
						{
							EmitParticle();
						}
						timeSinceLastEmission = 0;
					}
				}
				else
				{
					emitterFrequency = 0;
				}

				foreach (Particle particle in ParticleList.ToArray())
				{
					float y = -1 * ((float)Math.Cos(MathHelper.ToRadians(particle.Direction))) * particle.Speed;
					float x = (float)Math.Sin(MathHelper.ToRadians(particle.Direction)) * particle.Speed;

					particle.TotalLifetime += gameTime.ElapsedGameTime.Milliseconds;
					particle.Position += new Vector2(x, y);
					particle.Rotation += particle.RotationSpeed;
					ParticleScaler.Scale(particle, ParticleLifeTime);
					particle.Fade = ParticleFader.Fade(particle, ParticleLifeTime);
					particle.Color = new Color(particle.Fade, particle.Fade, particle.Fade, particle.Fade);

					if (particle.TotalLifetime > ParticleLifeTime)
					{
						ParticleList.Remove(particle);
					}
				}
			}
		}

		private void EmitParticle()
		{
			if (i > TextureList.Count - 1) i = 0;

			Particle particle = new Particle(TextureList[i],
																			 Position,
																			 (float)emitterHelper.RandomizedDouble(ParticleSpeed),
																			 (float)emitterHelper.RandomizedDouble(ParticleDirection),
																			 MathHelper.ToRadians((float)emitterHelper.RandomizedDouble(ParticleRotation)),
																			 (float)emitterHelper.RandomizedDouble(RotationSpeed),
																			 Opacity);
			ParticleList.Add(particle);
			EmittedNewParticle = true;
			LastEmittedParticle = particle;
			i++;
		}

		public void DrawParticles(GameTime gameTime, SpriteBatch spriteBatch)
		{
			foreach (Particle particle in ParticleList)
			{
				spriteBatch.Draw(particle.Texture,
												 particle.Position,
												 null,
												 particle.Color,
												 particle.Rotation,
												 particle.Center,
												 particle.Scale,
												 SpriteEffects.None,
												 0);
			}
		}
	}
}
