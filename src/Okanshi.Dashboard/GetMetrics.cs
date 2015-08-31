using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
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
			var deserializeObject = JsonConvert.DeserializeObject<IDictionary<string, dynamic>>(response);
			return deserializeObject
				.Select(x => new Metric
				{
					Name = x.Key,
					Measurements = x.Value.measurements.ToObject<IEnumerable<Measurement>>(),
					WindowSize = x.Value.windowSize.ToObject<float>()
				});
		} 
	}
}