using System;
using System.Linq;
using System.Net;
using Nancy;
using Newtonsoft.Json.Linq;

namespace Okanshi.Dashboard
{
	public class DashboardModule : NancyModule
	{
		public DashboardModule(IStorage storage, IGetMetrics getMetrics)
		{
			Get["/"] = p => View["index.html", storage.GetAll()];
			Get["/instance/{instanceName}"] = p =>
			{
				var webClient = new WebClient();
				var instanceName = p.instanceName.ToString();
				var response = webClient.DownloadString(storage.GetAll().Single(x => x.Name.Equals(instanceName, StringComparison.OrdinalIgnoreCase)).Url);
				var metrics = getMetrics.Deserialize(response);
				return Response.AsJson(metrics);
			};
		}
	}
}