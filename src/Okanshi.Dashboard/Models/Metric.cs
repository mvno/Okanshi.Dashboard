using System.Collections.Generic;

namespace Okanshi.Dashboard.Models
{
	public class Metric
	{
		public string Name { get; set; }
		public IEnumerable<Measurement> Measurements { get; set; }
		public float WindowSize { get; set; }
	}
}