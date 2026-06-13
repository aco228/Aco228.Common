using Newtonsoft.Json;

namespace Aco228.Common.Helpers;

public static class JsonConvertHelper
{
    public static bool TrySerialize<T>(string input, out T output)
    {
        output = default;
        if (string.IsNullOrEmpty(input))
            return false;

        output = JsonConvert.DeserializeObject<T>(input);
        return output != null;
    }
}