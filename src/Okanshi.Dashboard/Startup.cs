using System;
using System.Configuration;
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
			webApp = WebApp.Start<Startup>(ConfigurationManager.AppSettings["dashboard.url"]);
		}

		public void Stop()
		{
			webApp.Dispose();
		}
	}
}