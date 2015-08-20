using Owin;

namespace Okanshi.Dashboard
{
	public class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			app.UseNancy();
		}
	}
}