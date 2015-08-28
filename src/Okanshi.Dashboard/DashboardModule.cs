using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Nancy;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Okanshi.Dashboard.Models;

namespace Okanshi.Dashboard
{
	public class DashboardModule : NancyModule
	{
		public DashboardModule(IStorage storage)
		{
			Get["/"] = p => View["index.html", storage.GetAll()];
			Get["/instance/{instanceName}"] = p =>
			{
				var webClient = new WebClient();
				var instanceName = p.instanceName.ToString();
				var response = webClient.DownloadString(storage.GetAll().Single(x => x.Name.Equals(instanceName, StringComparison.OrdinalIgnoreCase)).Url);
				var deserializeObject = JsonConvert.DeserializeObject<IDictionary<string, dynamic>>(response);
				var metrics = deserializeObject
					.Select(x => new Metric
					{
						Name = x.Key,
						Measurements = x.Value.measurements.ToObject<IEnumerable<Measurement>>(),
						WindowSize = x.Value.windowSize.ToObject<float>()
					});
				return Response.AsJson(metrics);
			};
		}
	}
}