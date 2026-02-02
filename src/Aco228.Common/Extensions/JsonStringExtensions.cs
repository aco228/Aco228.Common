using System.Text.Json;

namespace Aco228.Common.Extensions;

public static class JsonStringExtensions
{
    public static T? JsonDeserialize<T>(this string json)
        => JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
}