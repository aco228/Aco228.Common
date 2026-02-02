using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Aco228.Common.Infrastructure.Yaml;

public static class YamlReader
{
    private static async Task<Tuple<string, IDeserializer>> ReadYamlFile(string filePath)
    {
        var fileContent = await File.ReadAllTextAsync(filePath);
        if(string.IsNullOrEmpty(fileContent))
            throw new Exception("File is empty");
        
        IDeserializer deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
    
        return new(fileContent, deserializer);
    }
    
    public static async Task<Dictionary<string, object>> ReadYamlAsDictionary(string filePath)
    {
        var (fileContent, deserializer) = await ReadYamlFile(filePath);
        return deserializer.Deserialize<Dictionary<string, object>>(fileContent);
    }
    
    public static async Task<dynamic> ReadYamlAsDynamic(string filePath)
    {
        var (fileContent, deserializer) = await ReadYamlFile(filePath);
        return deserializer.Deserialize<dynamic>(fileContent);
    }
    
    public static async Task<T> ReadYamlAsObject<T>(string filePath)
    {
        var (fileContent, deserializer) = await ReadYamlFile(filePath);
        return deserializer.Deserialize<T>(fileContent);
    }
    
}