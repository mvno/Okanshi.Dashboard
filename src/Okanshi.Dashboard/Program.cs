using Topshelf;

namespace Okanshi.Dashboard
{
	public class Program
	{
		public static void Main(string[] args)
		{
			HostFactory.Run(
				x =>
				{
					x.Service<Startup>(
						s =>
						{
							s.ConstructUsing(name => new Startup());
							s.WhenStarted(su => su.Start());
							s.WhenStopped(su => su.Stop());
						});
					x.RunAsLocalSystem();
					x.SetDescription("Runs Okanshi dashboard");
					x.SetServiceName("Okanshi.Dashboard");
					x.SetDisplayName("Okanshi.Dashboard");
				});
		}
	}
}
