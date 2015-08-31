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
				string instanceName = p.instanceName.ToString();
				var metrics = getMetrics.Execute(instanceName);
				return Response.AsJson(metrics);
			};
		}
	}
}