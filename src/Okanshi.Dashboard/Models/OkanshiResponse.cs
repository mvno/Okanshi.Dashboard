using System.Collections.Generic;

namespace Okanshi.Dashboard.Models
{
	public class OkanshiResponse
	{
		public string Version { get; set; }
		public IDictionary<string, dynamic> Data { get; set; }
	}
}