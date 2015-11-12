using Nancy;
using Okanshi.Dashboard.Models;

namespace Okanshi.Dashboard
{
	public class ApiModule : NancyModule
	{
		public ApiModule(IConfiguration configuration, IGetMetrics getMetrics, IGetHealthChecks getHealthChecks)
		{
			Get["/api/instances"] = _ => Response.AsJson(configuration.GetAll());
			Get["/api/instances/{instanceName}"] = p =>
			{
				string instanceName = p.instanceName.ToString();
				var service = new Service { Metrics = getMetrics.Execute(instanceName), HealthChecks = getHealthChecks.Execute(instanceName) };
				return Response.AsJson(service);
			};
			Get["/api/instances/{instanceName}/healthchecks"] = p =>
			{
				string instanceName = p.instanceName.ToString();
				return Response.AsJson(getHealthChecks.Execute(instanceName));
			};
			Get["/api/instances/{instanceName}/metrics"] = p =>
			{
				string instanceName = p.instanceName.ToString();
				return Response.AsJson(getMetrics.Execute(instanceName));
			};
		}
	}
}