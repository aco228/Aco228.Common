using System.Reflection;
using Aco228.Common.Extensions;
using Aco228.Common.Models;

namespace Aco228.Common.Helpers;

public static class AssemblyFileLocator
{
    private static List<string> IgnoreExtensions = new() { ".dll", ".pdb", ".env", ".exe"};
    public static ConcurrentList<FileInfo> AssemblyFiles { get; private set; } = new();

    public static void CacheAssemblyFiles(Assembly assembly)
    {
        AssemblyFiles = GetAssemblyFiles(assembly).ToConcurrentList();
    }

    public static bool TryFindAssemblyFile(string fileName, out FileInfo fileInfo)
    {
        fileInfo =  AssemblyFiles.FirstOrDefault(x => x.Name.Contains(fileName, StringComparison.InvariantCultureIgnoreCase));
        return fileInfo != null;
    }

    public static string ReadAssemblyFile(string fileName)
    {
        if (TryReadAssemblyFile(fileName, out var content))
            return content;
        
        throw new FileNotFoundException($"File {fileName} not found in assembly");
    }

    public static bool TryReadAssemblyFile(string fileName, out string? fileContent)
    {
        fileContent = null;
        var file = AssemblyFiles.FirstOrDefault(x => x.Name.Contains(fileName, StringComparison.InvariantCultureIgnoreCase));
        if (file == null)
            return false;
        
        fileContent = File.ReadAllText(file.FullName);
        return true;
    }
    
    public static IEnumerable<FileInfo> GetAssemblyFiles(Assembly assembly)
    {
        if(assembly == null)
            throw new ArgumentNullException(nameof(assembly));
        
        if(string.IsNullOrEmpty(assembly.Location))
            return Enumerable.Empty<FileInfo>();
        
        var directory = Path.GetDirectoryName(assembly.Location);
        if(directory == null)
            return Enumerable.Empty<FileInfo>();
        
        return IterateDirectory(new DirectoryInfo(directory));
    }

    private static IEnumerable<FileInfo> IterateDirectory(DirectoryInfo directory)
    {
        if(directory.Name.StartsWith("."))
            yield break;
        
        foreach (var fileInfo in directory.GetFiles())
            if(!IgnoreExtensions.Contains(fileInfo.Extension.ToLowerInvariant()))
                yield return fileInfo;

        foreach (var insideDirectory in directory.GetDirectories())
            foreach (var fileInfo in IterateDirectory(insideDirectory))
                yield return fileInfo;
    }
    
}