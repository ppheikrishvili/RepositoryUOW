using RepositoryUOW.Data.Repositories;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using RepositoryUOWDomain.Entities.Base;
using RepositoryUOWDomain.Shared.Common;
using RepositoryUOWDomain.Shared.Enums;

namespace RepositoryUOW.Services;

public class RepUowService<T> where T : BaseEntity
{
    public RepositoryUow<T> UowRep = new();
    public ConcurrentBagAsync<T> ExistList = new();

    public async Task GetBaseExistListAsync(Expression<Func<T, bool>> loadFilter = null!)
    {
        if (!ExistList.Any() && loadFilter == null) await ExistList.AddRange(UowRep.AllItemsAsync().ReturnValue).ConfigureAwait(false);
        else if (!ExistList.Any()) await ExistList.AddRange(await UowRep.WhereCondition(loadFilter).ToListAsync().ConfigureAwait(false)).ConfigureAwait(false);
    }

    public T? GetObject(Func<T, bool> predicate) => ExistList.Where(predicate).FirstOrDefault();

    public virtual async Task<ICollection<T>> TryPutAsync(ICollection<T> ts, Expression<Func<T, bool>> sourceFilter,
        Func<T, T, bool> predicate)
    {
        ConcurrentBagAsync<T> NewList = new();
        await GetBaseExistListAsync(sourceFilter).ConfigureAwait(false);
        await NewList.AddRange(ts.Where(s => !ExistList.Any(e => predicate.Invoke(s, e))).ToList()).ConfigureAwait(false);
        await UowRep.SaveRangeAsync(NewList, InsertUpdateEnum.Insert).ConfigureAwait(false);
        if (NewList.Count > 0) await ExistList.AddRange(NewList).ConfigureAwait(false);
        return ExistList.ToList();
    }
}