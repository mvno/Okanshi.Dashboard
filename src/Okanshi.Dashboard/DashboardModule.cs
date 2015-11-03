using System;
using System.Linq;
using Nancy;

namespace Okanshi.Dashboard
{
	public class DashboardModule : NancyModule
	{
		public DashboardModule(IConfiguration configuration)
		{
			Get["/"] = p => View["index.html", configuration.GetAll()];
			Get["/instances/{instanceName}"] = p =>
			{
				var instanceName = (string)p.instanceName;
				var server = configuration.GetAll().Single(x => x.Name.Equals(instanceName, StringComparison.OrdinalIgnoreCase));
				return View["details.html", server];
			};
		}
	}
}