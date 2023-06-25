using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using RepositoryUOWDomain.Interface;

namespace RepositoryUOW.Data.Repositories;

public class FactoryUow : IFactoryUow, IAsyncDisposable
{
    public AppDbContext AppContext { get; set; }

    ConcurrentDictionary<string, dynamic>? _repositories;

    public FactoryUow(AppDbContext appContext)
    {
        AppContext = appContext;
        AppContext.Database.SetCommandTimeout(160);
    }

    public FactoryUow()
    {
        AppContext = new AppDbContext();
        AppContext.Database.SetCommandTimeout(160);
    }
    
    private Type? GetSubClassType(Type baseT) => Assembly.GetAssembly(baseT)?.GetTypes().FirstOrDefault(t => t.IsSubclassOf(baseT)
                           && !t.IsAbstract && t.IsClass);

    public Task<int> CommitAsync(CancellationToken token = default) => AppContext.SaveChangesAsync(token);

    public ValueTask DisposeAsync() => AppContext.DisposeAsync();
     
    public IBase<T> Repository<T>() where T : class, IBaseEntity
    {
        _repositories ??= new ConcurrentDictionary<string, dynamic>();
        return _repositories.GetOrAdd(typeof(T).Name, _ => Repository<T>(AppContext));
    }

    public IBase<T> Repository<T>(IAppDbContext contextDb) where T : class, IBaseEntity
    {
        Type? repositoryType = GetSubClassType(typeof(Base<T>));
        if (repositoryType == null) return ((IBase<T>)Activator.CreateInstance(typeof(Base<T>), contextDb)!);
        return (IBase<T>)Activator.CreateInstance(repositoryType, contextDb)!;
    }

    public void RollBack()
    {
        foreach (EntityEntry entry in AppContext.ChangeTracker.Entries())
        {
            switch (entry.State)
            {
                case EntityState.Modified:
                    entry.State = EntityState.Unchanged;
                    break;
                case EntityState.Added:
                    entry.State = EntityState.Detached;
                    break;
                case EntityState.Deleted:
                    entry.Reload();
                    break;
            }
        }
    }
}