using System;

namespace Okanshi.Dashboard
{
	public class OkanshiServer
	{
		public OkanshiServer(string name, string url, long refreshRate)
		{
			Name = name;
			RefreshRate = refreshRate;
			Url = new Uri(url);
		}

		public string Name
		{
			get;
			private set;
		}

		public Uri Url
		{
			get;
			private set;
		}

		public long RefreshRate
		{
			get;
			private set;
		}
	}
}