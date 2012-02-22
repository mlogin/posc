using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace X2DPE.Helpers
{
	public class ParticleScaler
	{
		public float ScaleFrom { get; set; }
		public float ScaleTo { get; set; }
		public int ScaleStartTime { get; set; }	// Scale start time in milliseconds
		public int ScaleEndTime { get; set; }	// Scale end time in milliseconds

		public ParticleScaler(bool active, float scale)
			: this(active, scale, scale, 0, 0)
		{
		}

		public ParticleScaler(float scaleFrom, float scaleTo, int scaleStartTime, int scaleEndTime)
			: this(true, scaleFrom, scaleTo, scaleStartTime, scaleEndTime)
		{
		}

		private ParticleScaler(bool active, float scaleFrom, float scaleTo, int scaleStartTime, int scaleEndTime)
		{
			ScaleFrom = scaleFrom;
			ScaleTo = scaleTo;
			ScaleStartTime = scaleStartTime;
			ScaleEndTime = scaleEndTime;
		}

		public void Scale(Particle particle, float ParticleLifeTime)
		{
			if (particle.TotalLifetime < ScaleStartTime)
			{
				particle.Scale = ScaleFrom;
			}
			else if (particle.TotalLifetime >= ScaleEndTime)
			{
				particle.Scale = ScaleTo;
			}
			else
			{
				particle.Scale = ScaleFrom + (((particle.TotalLifetime - ScaleStartTime) / (ScaleEndTime - ScaleStartTime)) * (ScaleTo - ScaleFrom));
			}
		}
	}
}
