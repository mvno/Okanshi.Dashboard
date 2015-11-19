using System;
using System.Collections.Generic;

namespace Okanshi.Dashboard.Models
{
	public class Metric
	{
		public string Name { get; set; }
		public long WindowSize { get; set; }
		public IEnumerable<Measurement<DateTime, decimal>> Measurements { get; set; }
	}
}