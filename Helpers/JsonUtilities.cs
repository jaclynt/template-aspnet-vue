using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace ASPNetVueTemplate.Helpers;

public static class JsonUtilities
{
	public static object AsLowercase<T>(this T v)
	{
		var settings = new JsonSerializerSettings
		{
			ContractResolver = new LowercaseContractResolver()
		};

		var itemSerialize = JsonConvert.SerializeObject(v, settings);

		return JsonConvert.DeserializeObject(itemSerialize, settings);
	}

	public static object PreserveCase<T>(this T v)
	{
		var settings = new JsonSerializerSettings
		{
			ContractResolver = new SameCaseContractResolver()
		};

		var itemSerialize = JsonConvert.SerializeObject(v, settings);

		return JsonConvert.DeserializeObject(itemSerialize, settings);
	}

	public static JsonSerializerSettings GetSameCaseSettings()
	{
		return new JsonSerializerSettings
		{
			ContractResolver = new SameCaseContractResolver()
		};
	}
}

public class LowercaseContractResolver : DefaultContractResolver
{
	protected override string ResolvePropertyName(string propertyName)
	{
		return propertyName.ToLower();
	}
}

public class SameCaseContractResolver : DefaultContractResolver
{
	protected override string ResolvePropertyName(string propertyName)
	{
		return propertyName;
	}
}
