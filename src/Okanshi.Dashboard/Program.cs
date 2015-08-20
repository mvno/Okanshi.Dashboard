using System;
using Microsoft.Owin.Hosting;

namespace Okanshi.Dashboard
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var url = "http://+:13004";
			using (WebApp.Start<Startup>(url))
			{
				Console.WriteLine("Running on {0}", url);
				Console.WriteLine("Press enter to exit...");
				Console.ReadLine();
			}
		}
	}
}
