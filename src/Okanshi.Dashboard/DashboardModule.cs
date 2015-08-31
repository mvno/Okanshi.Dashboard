using Nancy;
using Okanshi.Dashboard.Models;

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
				var service = new Service { Metrics = getMetrics.Execute(instanceName) };
				return Response.AsJson(service);
			};
		}
	}
}