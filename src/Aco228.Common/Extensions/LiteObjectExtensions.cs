using Aco228.Common.Infrastructure.LiteDb;

namespace Aco228.Common.Extensions;

public static class LiteObjectExtensions
{
    public static void InsertOrUpdate<T>(this T obj)
        where T : LiteObject
    {
        using var db = new LiteFile<T>(obj.FileName, obj.FolderName);
        db.Insert(obj);
    }
}