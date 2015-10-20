using System.Collections.Generic;

namespace Okanshi.Dashboard
{
	public class InMemoryConfiguration : IConfiguration
	{
		private static readonly List<OkanshiServer> _configuration = new List<OkanshiServer>(); 

		public void Add(OkanshiServer server)
		{
			_configuration.Add(server);
		}

		public IEnumerable<OkanshiServer> GetAll()
		{
			return _configuration.ToArray();
		}
	}
}