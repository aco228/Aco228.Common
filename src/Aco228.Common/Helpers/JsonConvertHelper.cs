using Aco228.Common.Helpers.Json;
using Newtonsoft.Json;

namespace Aco228.Common.Helpers;

public static class JsonConvertHelper
{
    
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