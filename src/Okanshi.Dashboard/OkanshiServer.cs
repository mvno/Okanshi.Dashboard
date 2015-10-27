namespace Okanshi.Dashboard
{
	public class OkanshiServer
	{
		public OkanshiServer()
		{
			RefreshRate = 10;
		}

		public OkanshiServer(string name, string url, long refreshRate)
		{
			Name = name;
			RefreshRate = refreshRate;
			Url = url;
		}

		public string Name
		{
			get;
			set;
		}

		public string Url
		{
			get;
			set;
		}

		public long RefreshRate
		{
			get;
			set;
		}
	}
}