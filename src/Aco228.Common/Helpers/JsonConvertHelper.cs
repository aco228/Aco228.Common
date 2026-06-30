using Newtonsoft.Json;

namespace Aco228.Common.Helpers;

public static class JsonConvertHelper
{
    
    public static bool TryReadFile<T>(string filePath, out T output)
    {
        output = default;
        if (!File.Exists(filePath))
            return false;
        
        var content = File.ReadAllText(filePath);
        return TrySerialize(content, out output);
    }
    
    
    public static bool TrySerialize<T>(string input, out T output)  
    {
        output = default;
        if (string.IsNullOrEmpty(input))
            return false;

        output = JsonConvert.DeserializeObject<T>(input);
        return output != null;
    }
}