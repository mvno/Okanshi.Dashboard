using System;
using System.Collections.Generic;
using System.Linq;

namespace Okanshi.Dashboard
{
	public interface IStorage
	{
		void Add(OkanshiServer server);
		IEnumerable<OkanshiServer> GetAll();
	}

	public class InMemoryStorage : IStorage
	{
		private static readonly List<OkanshiServer> storage = new List<OkanshiServer>(); 

		public void Add(OkanshiServer server)
		{
			storage.Add(server);
		}

		public IEnumerable<OkanshiServer> GetAll()
		{
			return storage.ToArray();
		}
	}
}