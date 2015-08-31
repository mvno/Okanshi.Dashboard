using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Okanshi.Dashboard.Models;

namespace Okanshi.Dashboard
{
	public interface IGetMetrics
	{
		IEnumerable<Metric> Deserialize(string response);
	}

	public class GetMetrics : IGetMetrics
	{
		public IEnumerable<Metric> Deserialize(string response)
		{
			var jObject = JObject.Parse(response);
			JToken versionToken;
			jObject.TryGetValue("version", out versionToken);
			var version = "0";
			if (versionToken != null && versionToken.HasValues)
			{
				version = versionToken.Value<string>();
			}

			if (version.Equals("0", StringComparison.OrdinalIgnoreCase))
			{
				var deserializeObject = JsonConvert.DeserializeObject<IDictionary<string, dynamic>>(response);
				return deserializeObject
					.Select(x => new Metric
					{
						Name = x.Key,
						Measurements = x.Value.measurements.ToObject<IEnumerable<Measurement>>(),
						WindowSize = x.Value.windowSize.ToObject<float>()
					});
			}

			return Enumerable.Empty<Metric>();
		} 
	}
}