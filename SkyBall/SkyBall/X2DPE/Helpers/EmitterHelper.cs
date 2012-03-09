using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SkyBall.Core;

namespace X2DPE.Helpers
{
	class EmitterHelper
	{
		public double RandomizedDouble(RandomMinMax randomMinMax)
		{
			double min = randomMinMax.Min;
			double max = randomMinMax.Max;

			if (min == max)
				return max;
			else
				return min + (Tools.Rnd.NextDouble() * (max - min));
		}
	}
}
