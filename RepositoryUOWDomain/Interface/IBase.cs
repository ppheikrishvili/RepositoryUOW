using System.Linq.Expressions;
using RepositoryUOWDomain.Shared.Enums;
using RepositoryUOWDomain.ValueObject;

namespace RepositoryUOWDomain.Interface;

public interface IBase<T> where T : IBaseEntity
{
    public string? AttributeError { get; set; }

    Task<int> CountAsync(Expression<Func<T, bool>> condLambda);

    IQueryable<T> DbEntity();

    IQueryable<T> WhereCondition(Expression<Func<T, bool>> condLambda, Expression<Func<T, object>>? orderBy = null,
        string? include = null, Expression<Func<T, object>>? orderByDesc = null);

    IQueryable<T> WhereCondition(Expression<Func<T, bool>> condLambda, Expression<Func<T, object>> orderBy,
        ICollection<Expression<Func<T, object>>> include, Expression<Func<T, object>>? orderByDesc = null);

    IQueryable<T> WhereCondition(Expression<Func<T, bool>> condLambda, Expression<Func<T, object>> orderBy, IEnumerable<string> include,
            Expression<Func<T, object>>? orderByDesc = null);

    Task<List<T>> GetListAsync(Expression<Func<T, bool>> condLambda, Expression<Func<T, object>> orderBy, IEnumerable<string> include,
        Expression<Func<T, object>>? orderByDesc = null, int takeLines = 0, CancellationToken token = default);

    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>>? condLambda = null, CancellationToken token = default);

    Task<IList<T>> SelectFullActiveList(CancellationToken token = default);

    Task<T?> ElementByIdAsync(object id);

    Task<int> ElementCountAsync(Expression<Func<T, bool>>? condLambda = null);

    Task<bool> AnyAsync(Expression<Func<T, bool>>? condLambda = null);

    Task<bool> InsertOrUpdateAsync(T baseEntity, InsertUpdateEnum isModified = InsertUpdateEnum.Update);

    Task<bool> InsertOrUpdateAsync(IEnumerable<T> baseEntities, InsertUpdateEnum isModified = InsertUpdateEnum.Update);

    Task<bool> SaveRangeAsync(IEnumerable<T> baseEntities, InsertUpdateEnum isModified = InsertUpdateEnum.Update, CancellationToken token = default);

    Task<bool> Delete(T baseEntity, Func<T, Task<bool>>? validateDelete = null);

    Task<bool> DeleteRange(Expression<Func<T, bool>> condLambda, CancellationToken token = default);

    Task<bool> DeleteRange(IEnumerable<T> baseEntities, CancellationToken token = default);

    Task<bool> ValidateDelete(T deletedObject);

    Task<bool> ValidateSave(T baseEntity, Func<T, Task<bool>> validateSave);

    Task<bool> ValidateSave(T baseEntity);

    Task<bool> Save(T baseEntity, InsertUpdateEnum isModified = InsertUpdateEnum.Update, Func<T, Task<bool>>? validateDelete = null);

    Task<bool> Save(IEnumerable<T> baseEntities, InsertUpdateEnum isModified = InsertUpdateEnum.Update);

    void Reload(T refreshItem);

    Task<PagingValue<T>?> Paging(int pageIndex, int pageSize, IQueryable<T>? sourceEntities = null, int indexFrom = 0,
        CancellationToken cancellationToken = default);

    IQueryable<T>? PagingList(int pageIndex, int pageSize, IQueryable<T>? sourceEntities = null, int indexFrom = 0);

    IQueryable<T>? PagingList(int pageIndex, int pageSize, Expression<Func<T, bool>>? condLambda = null,
        int indexFrom = 0);
}