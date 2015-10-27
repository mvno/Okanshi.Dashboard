using System.Collections.Generic;

namespace Okanshi.Dashboard
{
	public interface IConfiguration
	{
		void Add(OkanshiServer server);
		IEnumerable<OkanshiServer> GetAll();
		void Remove(string name);
	}
}