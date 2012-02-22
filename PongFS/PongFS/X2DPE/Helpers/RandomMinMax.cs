using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace X2DPE.Helpers
{
	public class RandomMinMax
	{
		public double Min { get; set; }
		public double Max { get; set; }

		public RandomMinMax(double value)
			: this(value, value)
		{
		}

		public RandomMinMax(double min, double max)
		{
			Min = min;
			Max = max;
		}
	}
}
