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
			var version = jObject.GetValueOrDefault("Version", "-1");

			if (version.Equals("-1", StringComparison.OrdinalIgnoreCase))
			{
				var deserializeObject = JsonConvert.DeserializeObject<IDictionary<string, dynamic>>(response);
				return deserializeObject
					.Select(x => new OkanshiMetric
					{
						Name = x.Key,
						Measurements = x.Value.measurements.ToObject<IEnumerable<OkanshiMeasurement>>(),
						WindowSize = x.Value.windowSize.ToObject<long>()
					}.ToMetric());
			}

			if (version.Equals("0", StringComparison.OrdinalIgnoreCase))
			{
				var deserializeObject = jObject.GetValue("Data").ToObject<IDictionary<string, dynamic>>();
				return deserializeObject
					.Select(x => new OkanshiMetric
					{
						Name = x.Key,
						Measurements = x.Value.measurements.ToObject<IEnumerable<OkanshiMeasurement>>(),
						WindowSize = x.Value.windowSize.ToObject<long>()
					}.ToMetric());
			}

			throw new InvalidOperationException("Not supported version");
		}
	}
}