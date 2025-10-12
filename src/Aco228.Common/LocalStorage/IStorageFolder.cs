namespace Aco228.Common.LocalStorage;

public interface IStorageFolder
{
    string GetCurrentPath();
    DirectoryInfo GetDirectoryInfo();
    IStorageFolder GetFolder(string folderName, bool createIfNotExists = true);

    string GetPathForFile(string fileName);
    DirectoryInfo GetDirectoryInside(string directoryPath, bool createIfNotExists = true);

    void DeleteAllContent();

    bool IsEmpty();
    bool TryGetFile(string fileName, out FileInfo fileInfo);
    bool InsideFolderExists(string folderName);

    FileInfo MoveFile(FileInfo originalFileInfo, string newFileName = "");
    FileInfo MoveFile(string originalFilePath, string newFileName = "");

    FileInfo CopyFile(FileInfo originalFileInfo, string newFileName = "");
    FileInfo CopyFile(string originalFilePath, string newFileName = "");

    FileInfo? FindFile(string fileName);
    FileInfo? DeepSearchFor(string fileName, DirectoryInfo? startPosition = null);
}

public class StorageFolder : IStorageFolder
{
    private readonly DirectoryInfo _directory;

    public StorageFolder (DirectoryInfo directory)
    {
        _directory = directory;
    }

    public string GetCurrentPath()
        => GetDirectoryInfo().FullName;

    public DirectoryInfo GetDirectoryInfo()
        => _directory;

    public IStorageFolder GetFolder(string folderName, bool createIfNotExists = true)
    {
        var directory = GetDirectoryInside(folderName, createIfNotExists: createIfNotExists);
        return new StorageFolder(directory);
    }

    public string GetPathForFile(string fileName)
        => Path.Combine(_directory.FullName, fileName.Replace("/", @"\"));

    public DirectoryInfo GetDirectoryInside(string directoryPath, bool createIfNotExists = true)
    {
        var directory = new DirectoryInfo(Path.Combine(_directory.FullName, directoryPath));
        if (!directory.Exists)
        {
            if (createIfNotExists)
                directory.Create();
            else
                throw new ArgumentException($"Directory does not exists: {directoryPath}");
        }
        return directory;
    }

    public void DeleteAllContent()
    {
        foreach (var directory in _directory.GetDirectories())
            directory.Delete(true);

        foreach (var file in _directory.GetFiles())
            file.Delete();
    }

    public bool IsEmpty()
        => GetDirectoryInfo().GetDirectories().Any() == false && GetDirectoryInfo().GetFiles().Any() == false;

    public bool TryGetFile(string fileName, out FileInfo fileInfo)
    {
        fileInfo = new FileInfo(Path.Combine(GetCurrentPath(), fileName));
        return fileInfo.Exists;
    }

    public bool InsideFolderExists(string folderName)
        => new DirectoryInfo(Path.Combine(GetDirectoryInfo().FullName, folderName)).Exists;


    public FileInfo MoveFile(string originalFilePath, string newFileName = "")
        => MoveFile(new FileInfo(originalFilePath), newFileName);

    public FileInfo MoveFile(FileInfo originalFileInfo, string newFileName = "")
    {   
        if (!originalFileInfo.Exists)
            throw new InvalidOperationException($"File {originalFileInfo.FullName} does not exists");

        if (string.IsNullOrEmpty(newFileName))
            newFileName = originalFileInfo.Name;

        if (!newFileName.EndsWith(originalFileInfo.Extension))
            newFileName += originalFileInfo.Extension;

        string newLocation = GetPathForFile(newFileName);
        File.Move(originalFileInfo.FullName, newLocation);
        
        var newFile = new FileInfo(newLocation);
        if (!newFile.Exists)
            throw new InvalidOperationException($"Error moving file {originalFileInfo.Name} to location {_directory.Name}");

        return newFile;
    }
    
    public FileInfo CopyFile(string originalFilePath, string newFileName = "")
        => CopyFile(new FileInfo(originalFilePath), newFileName);

    public FileInfo? FindFile(string fileName)
        => _directory.GetFiles().FirstOrDefault(x => x.Name.Equals(fileName));

    public FileInfo CopyFile(FileInfo originalFileInfo, string newFileName = "")
    {
        if (!originalFileInfo.Exists)
            throw new InvalidOperationException($"File does not exists");

        if (string.IsNullOrEmpty(newFileName))
            newFileName = originalFileInfo.Name;

        if (newFileName.EndsWith(originalFileInfo.Extension))
            newFileName += originalFileInfo.Extension;

        var newLocation = GetPathForFile(newFileName);
        File.Copy(originalFileInfo.FullName, newLocation);
        
        var newFile = new FileInfo(newLocation);
        if (!newFile.Exists)
            throw new InvalidOperationException($"Error moving file {originalFileInfo.Name} to location {_directory.Name}");

        return newFile;
    }
    
    

    public FileInfo? DeepSearchFor(string fileName, DirectoryInfo? startPosition = null)
    {
        if (startPosition == null)
            startPosition = _directory;

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