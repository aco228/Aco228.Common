using Aco228.Common.Extensions;
using Aco228.Common.LocalStorage;

namespace Aco228.Common.ConsoleApp;

public class ConfigurationFilePropertyAttribute : Attribute
{
    public string Name { get; set; }
    public bool Required { get; set; } = false;
}

public abstract class ConsoleConfigurationFile
{
    public ConsoleConfigurationFile(string name, IStorageFolder? storageFolder = null)
    {
        if (storageFolder == null)
            storageFolder = StorageManager.Instance.GetFolder();
        
        var file = storageFolder.GetPathForFile(name + ".txt");
        if (!File.Exists(file))
            throw new  FileNotFoundException($"File {file} does not exist");
        
        var processedParameters = new HashSet<string>();
        var parameters = GetType().GetPropertyWithAttribute<ConfigurationFilePropertyAttribute>();

        foreach (var line in File.ReadAllLines(file))
        {
            var split = line.Trim().Split("=");
            if (split.Length != 2)
                continue;

            var propName = split[0].Trim();
            var prop = parameters.FirstOrDefault(x => x.Attribute.Name.Equals(propName)).Info;
            if (prop == null)
                continue;
            
            processedParameters.Add(propName);
            prop.SetValue(this, split.Last().Trim().CastObject(prop.PropertyType));
        }

        foreach (var requiredParams in parameters.Where(x => x.Attribute.Required))
            if (!processedParameters.Contains(requiredParams.Attribute.Name))
                throw new ArgumentException("Missing required parameter: " + requiredParams.Attribute.Name);
    }
}