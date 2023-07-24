using Newtonsoft.Json;
using System.Linq.Dynamic.Core;

namespace ASPNetVueTemplate.Helpers;

public static class DynamicQueries
{
	public static IQueryable<T> DynamicOrderBy<T>(this IQueryable<T> items, string sort, string defaultSort = "")
	{
		return items.OrderBy(GetSortName(sort, defaultSort));
	}

	public static IQueryable<T> DynamicThenBy<T>(this IOrderedQueryable<T> items, string sort, string defaultSort = "")
	{
		return items.ThenBy(GetSortName(sort, defaultSort));
	}

	private static string GetSortName(string sort, string defaultSort)
	{
		if (string.IsNullOrEmpty(sort))
		{
			sort = defaultSort;
		}

		string sortName = char.ToUpper(sort[0]) + sort.Substring(1);
		if (sort.Contains("_desc"))
		{
			sortName = sortName.Replace("_desc", " desc");
		}

		return sortName;
	}

	public static object GetPropertyValue<T>(this T obj, string propertyName)
	{
		return obj.GetType().GetProperty(propertyName).GetValue(obj, null);
	}

	public static void SetPropertyValue<T, V>(this T obj, string propertyName, V value)
	{
		obj.GetType().GetProperty(propertyName).SetValue(obj, value);
	}

	public static bool DoesPropertyExist(dynamic obj, string name)
	{
		string json = JsonConvert.SerializeObject(obj, JsonUtilities.GetSameCaseSettings());
		var dict = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(json);

		return dict.ContainsKey(name);
	}
}
