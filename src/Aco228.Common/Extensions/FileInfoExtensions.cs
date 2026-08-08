namespace Aco228.Common.Extensions;

public static class FileInfoExtensions
{
    public static FileInfo Rename(this FileInfo file, string newName, bool overwrite = false)
    {
        if (file == null)
            throw new ArgumentNullException(nameof(file));
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("New name cannot be null or empty.", nameof(newName));

        string newPath = Path.Combine(file.DirectoryName!, newName);
        file.MoveTo(newPath, overwrite);
        return file;
    } 
}