using Nancy;
using Nancy.TinyIoc;

namespace Okanshi.Dashboard
{
	public class Bootstrapper : DefaultNancyBootstrapper
	{
		protected override void ConfigureApplicationContainer(TinyIoCContainer container)
		{
			base.ConfigureApplicationContainer(container);
			container.Register<IConfiguration>(new InMemoryConfiguration());
		}
	}
}