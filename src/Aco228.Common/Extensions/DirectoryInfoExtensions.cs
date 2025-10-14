namespace Aco228.Common.Extensions;

public static class DirectoryInfoExtensions
{
    public static void TryDelete(string path)
    {
        var di = new DirectoryInfo(path);
        if (!di.Exists) return;
        try
        {
            di.Delete(true);
        }
        catch{}
    }
    
    public static void CopyFilesRecursively(string sourcePath, string targetPath)
    {
        //Now Create all of the directories
        foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
        {
            Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
        }

        //Copy all the files & Replaces any files with the same name
        foreach (string newPath in Directory.GetFiles(sourcePath, "*.*",SearchOption.AllDirectories))
        {
            File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
        }
    }
}