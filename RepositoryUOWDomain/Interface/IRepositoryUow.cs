using System.Linq.Expressions;
using RepositoryUOWDomain.Shared.Common;
using RepositoryUOWDomain.Shared.Enums;
using RepositoryUOWDomain.ValueObject;

namespace RepositoryUOWDomain.Interface;

public interface IRepositoryUow<T> where T : class
{
    //ResponseCodeEnum ResponseCode { get; set; }

    IQueryable<T> DbEntity();

    IQueryable<T> WhereCondition(Expression<Func<T, bool>> conditionLambda, Expression<Func<T, object>> orderBy, IEnumerable<string> include);
    
    Task<T?> SaveAsync(T savedObject, InsertUpdateEnum isModified = InsertUpdateEnum.Update, CancellationToken token = default);
    
    Task<bool> SaveRangeAsync(IList<T> savedObjects, InsertUpdateEnum isModified = InsertUpdateEnum.Update, CancellationToken token = default);

    Task<bool> SaveRangeAsync(ConcurrentBagAsync<T> savedObjects, InsertUpdateEnum isModified = InsertUpdateEnum.Update,
        CancellationToken token = default);

    Task<bool> DeleteAsync(T deletedObject, CancellationToken token = default);

    Task<bool> DeleteRangeAsync(Expression<Func<T, bool>> condLambda, CancellationToken token = default);

    Task<bool> DeleteRangeAsync(IEnumerable<T> baseEntities, CancellationToken token = default);

    Task<T?> ElementByIdAsync(object id);

    Task<IEnumerable<T>> AllItems(CancellationToken cancellationToken = default);
    ResponseResult<IList<T>> AllItemsAsync(CancellationToken cancellationToken = default);

    Task<PagingValue<T>?> PagingAsync(int pageIndex, int pageSize, IQueryable<T>? sourceEntities = null, int indexFrom = 0, 
        CancellationToken cancellationToken = default);

    IQueryable<T>? PagingList(int pageIndex, int pageSize, IQueryable<T>? sourceEntities = null, int indexFrom = 0);
}