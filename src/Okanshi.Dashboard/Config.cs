using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Okanshi.Dashboard
{
	public class Config
	{
		static Config()
		{
			using (var reader = new StreamReader(File.OpenRead("config.yml")))
			{
				var deserializer = new Deserializer(namingConvention: new CamelCaseNamingConvention());
				Instance = deserializer.Deserialize<Config>(reader);
			}
		}

		public static Config Instance { get; private set; }

		public string Url { get; set; }
	}
}