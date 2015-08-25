using System;
using System.Linq;
using System.Net;
using Nancy;
using Newtonsoft.Json;

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
				object jsonObject = JsonConvert.DeserializeObject<dynamic>(response);
				return Response.AsJson(jsonObject);
			};
		}
	}
}