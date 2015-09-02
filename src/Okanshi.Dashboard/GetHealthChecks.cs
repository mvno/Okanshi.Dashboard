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
		private readonly IStorage _storage;

		public GetHealthChecks(IStorage storage)
		{
			_storage = storage;
		}

		public IEnumerable<HealthCheck> Execute(string instanceName)
		{
			var webClient = new WebClient();
			var url = _storage.GetAll().Single(x => x.Name.Equals(instanceName, StringComparison.OrdinalIgnoreCase)).Url;
			var response = webClient.DownloadString(string.Format("{0}/healthchecks", url));
			var jObject = JObject.Parse(response);
			JToken versionToken;
			jObject.TryGetValue("version", out versionToken);
			var version = versionToken.Value<string>() ?? "0";

			if (version.Equals("0", StringComparison.OrdinalIgnoreCase))
			{
				var deserializeObject = JsonConvert.DeserializeObject<IDictionary<string, bool>>(response);
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