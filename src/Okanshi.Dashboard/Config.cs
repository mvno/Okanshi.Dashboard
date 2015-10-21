using System.Collections.Generic;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Okanshi.Dashboard
{
	public class Config : IConfiguration
	{
		private readonly Serializer _serializer;

		static Config()
		{
			using (var reader = new StreamReader(File.OpenRead("config.yml")))
			{
				var deserializer = new Deserializer(namingConvention: new CamelCaseNamingConvention());
				Instance = deserializer.Deserialize<Config>(reader);
			}
		}

		public Config()
		{
			_serializer = new Serializer(namingConvention: new CamelCaseNamingConvention());
		}

		public static Config Instance { get; private set; }

		public string Url { get; set; }
		public IList<OkanshiServer> OkanshiInstances { get; set; }

		public void Add(OkanshiServer server)
		{
			OkanshiInstances.Add(server);
			Save();
		}

		public IEnumerable<OkanshiServer> GetAll()
		{
			return OkanshiInstances;
		}

		private void Save()
		{
			using (var writer = new StreamWriter(File.OpenWrite("config.yml")))
			{
				_serializer.Serialize(writer, this);
				writer.Flush();
			}
		}
	}
}