namespace Aco228.Common.LocalStorage;

public interface IStorageManager
{
    DirectoryInfo GetDocumentsDirectoryInfo();
    DirectoryInfo GetCurrentAssemblyLocation();
    DirectoryInfo? GetBaseGithubFolder();
    string GetAssemblyLocation(string fileLocation);
    string GetAssemblyLocation(string assetsFolder, string fileName);
    string GetDocumentsDirectoryPath();
    IStorageFolder GetTempFolder();
    IStorageFolder GetFolder(string folderName, bool createIfNotExists = true);
    FileInfo? DeepSearchFor(string fileName, DirectoryInfo? startPosition = null);
}

public class StorageManager : IStorageManager
{
    public StorageManager()
    {
        
    }
    
    public static StorageManager Instance { get; } = new();
    
    public DirectoryInfo GetDocumentsDirectoryInfo()
    {
        var directory = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            AcoCommonConfigurable.DocumentFolderName));

        if (!directory.Exists)
            directory.Create();

        return directory;
    }

    public DirectoryInfo GetCurrentAssemblyLocation()
    {
        var file = new FileInfo(System.Reflection.Assembly.GetEntryAssembly()?.Location ?? "");
        return file.Directory!;
    }

    public DirectoryInfo? GetBaseGithubFolder()
    {
        var baseFolder = GetCurrentAssemblyLocation();
        for (int i = 0; i < 50; i++)
        {
            if (baseFolder == null)
                return null;
            
            if (baseFolder.Name.Equals(AcoCommonConfigurable.ProjectName))
                return baseFolder;

            baseFolder = baseFolder.Parent;
            
        }

        return null;
    }

    public string GetAssemblyLocation(string fileLocation)
        => Path.Combine(GetCurrentAssemblyLocation().FullName, fileLocation.Replace("/", @"\"));

    public string GetAssemblyLocation(string assetsFolder, string fileName)
        => Path.Combine(GetCurrentAssemblyLocation().FullName, assetsFolder, fileName.Replace("/", @"\"));

    public async Task<string?> ReadAssemblyLocationAsync(string assetsFolder, string fileName)
    {
        var path = GetAssemblyLocation(assetsFolder, fileName);
        if (!File.Exists(path))
            return null;
        
        var raw = await File.ReadAllTextAsync(path);
        return raw;
    }

    public string GetDocumentsDirectoryPath()
        => GetDocumentsDirectoryInfo().FullName;

    public IStorageFolder GetTempFolder()
        => GetFolder(AcoCommonConfigurable.TempFolderName, createIfNotExists: true);

    public IStorageFolder GetFolder()
    {
        var documentFolder = new DirectoryInfo(GetDocumentsDirectoryPath());
        return new StorageFolder(documentFolder);
    }

    public IStorageFolder GetFolder(string folderName, bool createIfNotExists = true)
    {
        var documentFolder = new DirectoryInfo(Path.Combine(GetDocumentsDirectoryPath(), folderName));
        if (!documentFolder.Exists)
        {
            if (createIfNotExists)
                documentFolder.Create();
            else
                throw new ArgumentException($"Folder name {folderName} does not exists");
        }

        return new StorageFolder(documentFolder);
    }

    public FileInfo? DeepSearchFor(string fileName, DirectoryInfo? startPosition = null)
    {
        if (startPosition == null)
            startPosition = GetDocumentsDirectoryInfo();

        foreach (var fileInfo in startPosition.GetFiles())
            if (fileInfo.Name.Equals(fileName, StringComparison.InvariantCultureIgnoreCase))
                return fileInfo;

        foreach (var directoryInfo in startPosition.GetDirectories())
        {
            var result = DeepSearchFor(fileName, directoryInfo);
            if (result != null)
                return result;
        }

        return null;
    }
}