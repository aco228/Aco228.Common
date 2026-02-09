namespace Aco228.Common.Infrastructure.LiteDb;

public abstract class LiteObject
{
    protected virtual string FolderName { get; } = string.Empty;
    protected virtual string FileName { get; } = string.Empty;
}