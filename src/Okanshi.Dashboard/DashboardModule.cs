using Nancy;

namespace Okanshi.Dashboard
{
	public class DashboardModule : NancyModule
	{
		public DashboardModule(IConfiguration configuration)
		{
			Get["/"] = p => View["index.html", configuration.GetAll()];
			Get["/instances/{instanceName}"] = p => View["details.html", p.instanceName];
		}
	}
}