using System.Collections.Generic;

namespace Okanshi.Dashboard.Models
{
	public class OkanshiMetric
	{
		public string Name { get; set; }
		public IEnumerable<OkanshiMeasurement> Measurements { get; set; }
		public float WindowSize { get; set; }
	}
}