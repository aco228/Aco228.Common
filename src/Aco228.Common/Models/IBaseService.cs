namespace Aco228.Common.Models;

public interface IBaseService
{
    
}

public interface IScoped : IBaseService { }
public interface ITransient : IBaseService { }
public interface ISingleton : IBaseService { }