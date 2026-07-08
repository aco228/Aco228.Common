using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace Aco228.Common.Helpers;

public static class BsonPackHelper
{
    public static void Write<T>(string path, T obj)
    {
        using var stream = File.Create(path);
        using var writer = new BsonDataWriter(stream);
        new JsonSerializer().Serialize(writer, obj);
    }

    public static T Read<T>(string path)
    {
        using var stream = File.OpenRead(path);
        using var reader = new BsonDataReader(stream);
        return new JsonSerializer().Deserialize<T>(reader);
    }
}