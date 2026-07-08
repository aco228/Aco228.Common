using System.Buffers;
using MessagePack;
using MessagePack.Formatters;
using MongoDB.Bson;

namespace Aco228.Common.Helpers.Json;

public class MessagePackObjectIdFormatter: IMessagePackFormatter<ObjectId>
{
    public static readonly MessagePackObjectIdFormatter Instance = new();
 
    public void Serialize(ref MessagePackWriter writer, ObjectId value, MessagePackSerializerOptions options)
    {
        writer.Write(value.ToByteArray()); // fixed 12-byte representation
    }
 
    public ObjectId Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        var bytes = reader.ReadBytes()?.ToArray();
        return bytes == null ? ObjectId.Empty : new ObjectId(bytes);
    }
}