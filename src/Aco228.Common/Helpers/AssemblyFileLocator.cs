using System.Reflection;

namespace Aco228.Common.Helpers;

public static class AssemblyFileLocator
{
    private static List<string> IgnoreExtensions = new() { ".dll", ".pdb", ".env", ".exe"};
    public static List<FileInfo> AssemblyFiles { get; private set; } = new();

    public static void CacheAssemblyFiles(Assembly assembly)
    {
        AssemblyFiles = GetAssemblyFiles(assembly).ToList();
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
        foreach (var fileInfo in directory.GetFiles())
            if(!IgnoreExtensions.Contains(fileInfo.Extension.ToLowerInvariant()))
                yield return fileInfo;

        foreach (var insideDirectory in directory.GetDirectories())
            foreach (var fileInfo in IterateDirectory(insideDirectory))
                yield return fileInfo;
    }
    
}