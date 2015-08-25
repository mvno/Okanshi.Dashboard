using System;
using Microsoft.Owin.Hosting;
using Owin;

namespace Okanshi.Dashboard
{
	public class Startup
	{
		private IDisposable webApp;

		public void Configuration(IAppBuilder app)
		{
			app.UseNancy();
		}

		public void Start()
		{
			const string url = "http://+:13016";
			webApp = WebApp.Start<Startup>(url);
		}

		public void Stop()
		{
			webApp.Dispose();
		}
	}
}