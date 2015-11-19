using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Okanshi.Dashboard.Models;

namespace Okanshi.Dashboard
{
	public interface IGetHealthChecks
	{
		IEnumerable<HealthCheck> Execute(string instanceName);
	}

	public class GetHealthChecks : IGetHealthChecks
	{
		private readonly IConfiguration _configuration;

		public GetHealthChecks(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public IEnumerable<HealthCheck> Execute(string instanceName)
		{
			var webClient = new WebClient();
			var url = _configuration.GetAll().Single(x => x.Name.Equals(instanceName, StringComparison.OrdinalIgnoreCase)).Url;
			var response = webClient.DownloadString(string.Format("{0}/healthchecks", url));
			var jObject = JObject.Parse(response);
			var version = jObject.GetValueOrDefault("version", "0");

			if (version.Equals("0", StringComparison.OrdinalIgnoreCase))
			{
				var deserializeObject = JsonConvert.DeserializeObject<IDictionary<string, dynamic>>(response);
				return deserializeObject
					.Select(x => new HealthCheck
					{
						Name = x.Key,
						Success = x.Value,
					});
			}

			return Enumerable.Empty<HealthCheck>();
		}
	}
}