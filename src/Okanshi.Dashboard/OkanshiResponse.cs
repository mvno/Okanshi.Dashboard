using System.Collections.Generic;

namespace Okanshi.Dashboard
{
	public class OkanshiResponse
	{
		public string Version { get; set; }
		public IDictionary<string, dynamic> Data { get; set; }
	}
}