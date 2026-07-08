using MongoDB.Bson;
using Newtonsoft.Json;

namespace Aco228.Common.Helpers.Json;

public class ObjectIdJsonConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(ObjectId) || objectType == typeof(ObjectId?);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
            return objectType == typeof(ObjectId?) ? (ObjectId?)null : ObjectId.Empty;

        var value = reader.Value?.ToString();

        if (string.IsNullOrEmpty(value))
            return objectType == typeof(ObjectId?) ? (ObjectId?)null : ObjectId.Empty;

        return ObjectId.Parse(value);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if (value == null)
            writer.WriteNull();
        else
            writer.WriteValue(value.ToString());
    }
}