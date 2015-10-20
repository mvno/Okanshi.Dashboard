using System;
using System.Collections.Generic;
using System.Linq;

namespace Okanshi.Dashboard
{
	public interface IConfiguration
	{
		void Add(OkanshiServer server);
		IEnumerable<OkanshiServer> GetAll();
	}

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