using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace X2DPE.Helpers
{
	public class ParticleFader
	{
		public bool ParticleFadeIn { get; set; }
		public bool ParticleFadeOut { get; set; }
		public int FadeDelay { get; set; }

		public ParticleFader(bool particleFadeIn, bool particleFadeOut)
			: this(particleFadeIn, particleFadeOut, 0)
		{
		}

		public ParticleFader(bool particleFadeIn, bool particleFadeOut, int fadeDelay)
		{
			ParticleFadeIn = particleFadeIn;
			ParticleFadeOut = particleFadeOut;
			FadeDelay = fadeDelay;
		}

		public int Fade(Particle particle, float ParticleLifeTime)
		{
			int fade = particle.Fade;

			if (ParticleFadeIn && ParticleFadeOut)
			{
				FadeDelay = (int)ParticleLifeTime / 2;
			}

			// Fade in
			if (ParticleFadeIn)
			{
				if (particle.TotalLifetime <= ParticleLifeTime - FadeDelay)
				{
					fade = (int)(((particle.TotalLifetime) / (ParticleLifeTime - FadeDelay)) * particle.InitialOpacity);
				}
			}

			// Fade out
			if (ParticleFadeOut)
			{
				if (particle.TotalLifetime > FadeDelay)
				{
					fade = particle.InitialOpacity - (int)(((particle.TotalLifetime - FadeDelay) / (ParticleLifeTime - FadeDelay)) * particle.InitialOpacity);
				}
			}

			return fade;
		}
	}
}
