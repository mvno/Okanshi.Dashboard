using Newtonsoft.Json.Linq;

namespace Okanshi.Dashboard
{
	public static class JObjectExtensions
	{
		public static T GetValueOrDefault<T>(this JObject jObject, string propertyName, T defaultValue)
		{
			JToken token;
			jObject.TryGetValue(propertyName, out token);
			if (token == null)
			{
				return defaultValue;
			}

			var value = token.Value<T>();
			return value.Equals(default(T)) ? defaultValue : value;
		}
	}
}