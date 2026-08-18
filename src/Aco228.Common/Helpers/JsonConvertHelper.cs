using Aco228.Common.Helpers.Json;
using Newtonsoft.Json;

namespace Aco228.Common.Helpers;

public static class JsonConvertHelper
{
    
    public static string JsonEscape(this string value)
    {
        // Serializes the string as a JSON string literal, then strips the surrounding quotes,
        // so you get exactly the escaped inner content to drop into your template.
        var encoded = System.Text.Json.JsonSerializer.Serialize(value);
        return encoded.Substring(1, encoded.Length - 2);
    }
    
    public static bool TryReadFile<T>(string filePath, out T output)
    {
        output = default;
        if (!File.Exists(filePath))
            return false;

        string? content;
        try
        {
            content = File.ReadAllText(filePath);
            if (string.IsNullOrEmpty(content))
                return false;
        }
        catch
        {
            return false;
        }
        
        return TrySerialize(content, out output);
    }
    
    
    public static bool TrySerialize<T>(string input, out T output)  
    {
        try
        {
            output = default;
            if (string.IsNullOrEmpty(input))
                return false;

            var settings = new JsonSerializerSettings
            {
                Converters = new List<JsonConverter> { new ObjectIdJsonConverter() }
            };
            output = JsonConvert.DeserializeObject<T>(input, settings);
            return output != null;
        }
        catch
        {
            output = default;
            return false;
        }
    }
}