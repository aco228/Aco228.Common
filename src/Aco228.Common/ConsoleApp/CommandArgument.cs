using Aco228.Common.Extensions;
using Newtonsoft.Json;

namespace Aco228.Common.ConsoleApp;

public class CArgument : Attribute
{
    public string Name { get; set; }
    public string Description { get; set; }
    public bool Required { get; set; } = false;
}

public class CommandArgument
{
    private HashSet<string> _missingParameters = new();
    
    [CArgument(Name = "help", Description = "Show help")]
    public bool ShowHelp { get; set; }
    
    public CommandArgument (string[] args)
    {
        var processedParameters = new HashSet<string>();
        var parameters = GetType().GetPropertyWithAttribute<CArgument>();

        foreach (var arg in args)
        {
            var split = arg.Trim().Split("=");
            if (split.Length == 1)
            {
                var paramName = split.First().Trim().Remove("--").Remove("-");
                var flagProp = parameters.FirstOrDefault(x => x.Attribute.Name.Equals(paramName)).Info;
                if (flagProp != null && flagProp.PropertyType == typeof(bool))
                {
                    processedParameters.Add(paramName);
                    flagProp.SetValue(this, true);
                }
                
                continue;
            }
            
            if (split.Length != 2)
                continue;

            var name = split.First().Remove("--").Remove("-");
            var prop = parameters.FirstOrDefault(x => x.Attribute.Name.Equals(name)).Info;
            if (prop == null)
                continue;

            var value = split.Last().Trim();
            if (string.IsNullOrEmpty(value))
                continue;

            processedParameters.Add(name);
            if (prop.PropertyType == typeof(bool))
            {
                prop.SetValue(this, (value == "1" || value.Equals("true", StringComparison.InvariantCultureIgnoreCase)));
                continue;
            }
            
            prop.SetValue(this, split.Last().CastObject(prop.PropertyType));
        }

        foreach (var requiredParams in parameters.Where(x => x.Attribute.Required))
            if (!processedParameters.Contains(requiredParams.Attribute.Name))
                _missingParameters.Add(requiredParams.Attribute.Name);
    }

    public bool IsValid(out int returnInt)
    {
        returnInt = 0;
        if (_missingParameters.Any())
        {
            PrintErrorMessage();
            returnInt = -1;
            return false;
        }
        
        if (!ShowHelp)
            return true;
        
        PrintHelp();
        return false;
    }

    private void PrintErrorMessage()
    {
        Console.WriteLine(Console.Title);
        Console.WriteLine("=============================================");
        Console.WriteLine();
        Console.WriteLine();
        
        Console.WriteLine("Missing required parameters: ");
        Console.WriteLine("  " + string.Join(", ", _missingParameters));
        Environment.ExitCode = -1;
    }
    
    private void PrintHelp()
    {
        Console.WriteLine(Console.Title);
        Console.WriteLine("=============================================");
        Console.WriteLine();
        Console.WriteLine();
        
        var parameters = this.GetType().GetPropertyWithAttribute<CArgument>();
        foreach (var (info, attr) in parameters)
        {
            var defaultValue = info.GetValue(this)?.ToString() ?? "null";
            Console.WriteLine($"{attr.Name} ({info.PropertyType.Name}) [default: {defaultValue}]");
            Console.WriteLine($"    == {attr.Description}");
            Console.WriteLine();
        }
    }
}