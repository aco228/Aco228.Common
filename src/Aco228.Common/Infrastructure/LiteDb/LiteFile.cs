using System.Linq.Expressions;
using Aco228.Common.LocalStorage;
using LiteDB;

namespace Aco228.Common.Infrastructure.LiteDb;

public class LiteFile<T> : IDisposable
{
    public LiteDatabase? Db { get; private set; }
    public ILiteCollection<T> Collection { get; private set; }
    
    public LiteFile() {}

    public LiteFile(string fileLocation, string folderName = "")
    {   
        LoadFile(fileLocation, folderName);
    }

    public LiteFile<T> LoadFile(string fileName, string folderName = "")
    {
        if (string.IsNullOrEmpty(folderName) && typeof(LiteObject).IsAssignableFrom(typeof(T)))
        {
            var dummype = Activator.CreateInstance<T>() as LiteObject;
            folderName = dummype!.FolderName;
        }
        
        if(!fileName.EndsWith(".lite"))
            fileName += ".lite";
        
        var folder = StorageManager.Instance.GetFolder("lite");
        if(!string.IsNullOrEmpty(folderName))
            folder = folder.GetFolder(folderName);
        
        var filePath = folder.GetPathForFile(fileName);
        
        Db = new LiteDatabase(filePath);
        Collection = Db.GetCollection<T>();
        return this;
    }
    
    public IEnumerable<T> GetAll() 
        => Collection.FindAll();
    
    public T? GetDefault()
        => Find(x => true).FirstOrDefault();
    
    public IEnumerable<T> Find(Expression<Func<T, bool>> predicate, int skip = 0, int limit = 2147483647)
        => Collection.Find(predicate, skip, limit);
    
    public void Update(T entry)
        => Collection.Update(entry);
    
    public void Insert(T entity) 
        => Collection.Insert(entity);


    public void Dispose()
    {
        Db?.Dispose();
    }
}