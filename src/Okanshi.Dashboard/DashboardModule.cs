using Nancy;
using Okanshi.Dashboard.Models;

namespace Okanshi.Dashboard
{
	public class DashboardModule : NancyModule
	{
		private readonly IGetHealthChecks _getHealthChecks;

		public DashboardModule(IStorage storage, IGetMetrics getMetrics, IGetHealthChecks getHealthChecks)
		{
			_getHealthChecks = getHealthChecks;
			Get["/"] = p => View["index.html", storage.GetAll()];
			Get["/instance/{instanceName}"] = p =>
			{
				string instanceName = p.instanceName.ToString();
				var service = new Service { Metrics = getMetrics.Execute(instanceName), HealthChecks = _getHealthChecks.Execute(instanceName) };
				return Response.AsJson(service);
			};
		}
	}
}