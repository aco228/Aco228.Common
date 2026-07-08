using Aco228.Common.Helpers.Json;
using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;

namespace Aco228.Common.Helpers;

public static class MessagePackHelper
{
    public static FileInfo WriteToFile<T>(string fileLocation, T obj)
    {
        // unique per-call temp name so concurrent writers (e.g. different worker
        // machines) never collide on the same temp file
        var tempFile = $"{fileLocation}.{Guid.NewGuid():N}.tmp";

        using (var saveFile = new FileStream(tempFile, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            MessagePackSerializer.Serialize<T>(saveFile, obj);
            saveFile.Flush(true); // force to disk before we rename
        }

        File.Move(tempFile, fileLocation, overwrite: true);
        return new FileInfo(fileLocation);
    }

    public static async Task<FileInfo> WriteToFileAsync<T>(string fileLocation, T obj)
    {
        var tempFile = $"{fileLocation}.{Guid.NewGuid():N}.tmp";

        await using (var saveFile = new FileStream(tempFile, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            await MessagePackSerializer.SerializeAsync<T>(saveFile, obj);
            await saveFile.FlushAsync();
        }

        File.Move(tempFile, fileLocation, overwrite: true);
        return new FileInfo(fileLocation);
    }

    public static T Read<T>(string fileLocation)
    {
        using var fileStream = new FileStream(fileLocation, FileMode.Open, FileAccess.Read, FileShare.Read);
        var result = MessagePackSerializer.Deserialize<T>(fileStream);
        fileStream.Close();
        fileStream.Dispose();
        return result;
    }

    public static async Task<T> ReadAsync<T>(string fileLocation)
    {
        await using var fileStream = new FileStream(fileLocation, FileMode.Open, FileAccess.Read, FileShare.Read);
        var result = await MessagePackSerializer.DeserializeAsync<T>(fileStream);
        fileStream.Close();
        await fileStream.DisposeAsync();
        return result;
    }

    public static async Task<T?> TryReadAsync<T>(string fileLocation)
    {
        if (!File.Exists(fileLocation))
            return default;

        try
        {
            await using var fileStream = new FileStream(fileLocation, FileMode.Open, FileAccess.Read, FileShare.Read);
            var result = await MessagePackSerializer.DeserializeAsync<T>(fileStream);
            fileStream.Close();
            await fileStream.DisposeAsync();
            return result;
        }
        catch
        {
            return default;
        }
    }
    
    
    public static readonly MessagePackSerializerOptions Default = MessagePackSerializerOptions.Standard
        .WithResolver(
            CompositeResolver.Create(
                new IMessagePackFormatter[] { MessagePackObjectIdFormatter.Instance },
                new IFormatterResolver[]
                {
                    GeneratedMessagePackResolver.Instance, // source-generated formatters for [MessagePackObject] classes in this assembly
                    StandardResolver.Instance
                }
            )
        );
 
    /// <summary>
    /// Call this once at startup (e.g. top of Program.cs) so every
    /// MessagePackSerializer.Serialize/Deserialize call anywhere in the app
    /// — including inside Aco228.Common.Helpers.MessagePackHelper — picks up
    /// this resolver automatically, with no per-call options needed.
    /// </summary>
    public static void Register()
    {
        MessagePackSerializer.DefaultOptions = Default;
    }
}