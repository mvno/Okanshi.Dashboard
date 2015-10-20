using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Okanshi.Dashboard.Models;

namespace Okanshi.Dashboard
{
	public interface IGetMetrics
	{
		IEnumerable<Metric> Execute(string instanceName);
	}

	public class GetMetrics : IGetMetrics
	{
		private readonly IConfiguration _configuration;

		public GetMetrics(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public IEnumerable<Metric> Execute(string instanceName)
		{
			var webClient = new WebClient();
			var response = webClient.DownloadString(_configuration.GetAll().Single(x => x.Name.Equals(instanceName, StringComparison.OrdinalIgnoreCase)).Url);
			var jObject = JObject.Parse(response);
			var version = jObject.GetValueOrDefault("version", "0");

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

			throw new InvalidOperationException("Not supported version");
		}
	}
}