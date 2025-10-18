namespace Aco228.Common.Extensions;

public static class DictionaryExtensions
{
    public static T? TryExtractValue<T>(this IDictionary<string, T> dictionary, string key, T? defaultValue = default)
    {
        if (dictionary.TryGetValue(key, out T value))
            return value;

        return default;
    }
    
    public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
    {
        if(dictionary.ContainsKey(key))
            dictionary[key] = value;
        else
            dictionary.Add(key, value);
    }
}