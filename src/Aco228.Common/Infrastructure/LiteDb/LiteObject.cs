namespace Aco228.Common.Infrastructure.LiteDb;

public abstract class LiteObject
{
    public virtual string FolderName { get; } = string.Empty;
    public virtual string FileName { get; } = string.Empty;
}