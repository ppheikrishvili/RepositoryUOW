using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using RepositoryUOW.Data.Exceptions;
using RepositoryUOWDomain.Interface;
using RepositoryUOWDomain.Shared.Enums;
using RepositoryUOWDomain.ValueObject;

namespace RepositoryUOW.Data.Repositories;

public class Base<T> : IBase<T> where T : class, IBaseEntity
{
    public DbSet<T> DbSetEntity { get; set; }

    public string? AttributeError { get; set; }

    public AppDbContext AppContext { get; set; }

    public Base()
    {
        AppContext = new AppDbContext();
        DbSetEntity = AppContext.Set<T>();
    }

    public Base(AppDbContext context)
    {
        AppContext = context;
        DbSetEntity = context.Set<T>();
    }

    public async Task<bool> Delete(T baseEntities, Func<T, Task<bool>>? validateDelete = null)
    {
        if (!await ValidateDelete(baseEntities).ConfigureAwait(false)) return await Task.FromResult(false).ConfigureAwait(false);
        DbSetEntity.Remove(baseEntities);
        
        return await Task.FromResult(true).ConfigureAwait(false);
    }

    public Task<int> CountAsync(Expression<Func<T, bool>> condLambda)
    {
        return DbSetEntity.AsQueryable().AsNoTracking().CountAsync(condLambda);
    }

    public async Task<bool> DeleteRange(Expression<Func<T, bool>> condLambda, CancellationToken token = default)
    {
        await using var transaction = await AppContext.Database.BeginTransactionAsync(token).ConfigureAwait(false);
        bool retValue = await DbSetEntity.Where(condLambda).ExecuteDeleteAsync(token).ConfigureAwait(false) > 0;
        await transaction.CommitAsync(token).ConfigureAwait(false);
        return retValue;
    }


    public async Task<bool> DeleteRange(IEnumerable<T> baseEntities, CancellationToken token = default)
    {
        await Task.Run( () => DbSetEntity.RemoveRange(baseEntities), token).ConfigureAwait(false);
        return true;
    }

    public async Task<T?> ElementByIdAsync(object id) => await DbSetEntity.FindAsync(id).ConfigureAwait(false);

    public async Task<bool> InsertOrUpdateAsync(T? baseEntity, InsertUpdateEnum isModified = InsertUpdateEnum.Update)
    {
        if (baseEntity == null) return await Task.FromResult(true).ConfigureAwait(false);
        AppContext.Entry(baseEntity).State = EntityState.Detached;
        _ = DbSetEntity.Attach(baseEntity);
        AppContext.Entry(baseEntity).State = isModified == InsertUpdateEnum.Update ? EntityState.Modified : EntityState.Added;
        return await Task.FromResult(true).ConfigureAwait(false);
    }

    private async Task<bool> InsertOrUpdateRangeAsync(IEnumerable<T>? baseEntities, InsertUpdateEnum isModified = InsertUpdateEnum.Update,
        CancellationToken token = default)
    {
        if (baseEntities is null) return false;

        var enumerable = baseEntities.ToList();

        var invalidItem = enumerable.FirstOrDefault(f => !f.IsValid());

        if (invalidItem != null)
            throw new RequestException(invalidItem.AttributeError!, ResponseCodeEnum.ValidationError);

        if (isModified == InsertUpdateEnum.Update)
        {
            enumerable.ForEach(f => AppContext.Entry(f).State = EntityState.Detached);

            await AppContext.AddRangeAsync(enumerable, token).ConfigureAwait(false);

            enumerable.ForEach(f => AppContext.Entry(f).State = EntityState.Modified);
        }
        else await AppContext.AddRangeAsync(enumerable, token).ConfigureAwait(false);

        return true;
    }

    public Task<bool> SaveRangeAsync(IEnumerable<T>? baseEntities, InsertUpdateEnum isModified = InsertUpdateEnum.Update,
        CancellationToken token = default) => InsertOrUpdateRangeAsync(baseEntities, isModified, token);

    public async Task<bool> InsertOrUpdateAsync(IEnumerable<T> baseEntities,
        InsertUpdateEnum isModified = InsertUpdateEnum.Update)
    {
        foreach (T one in baseEntities)
        {
            await InsertOrUpdateAsync(one, isModified).ConfigureAwait(false);
        }

        return true;
    }

    public Task<bool> Save(IEnumerable<T> baseEntities,
        InsertUpdateEnum isModified = InsertUpdateEnum.Update) => InsertOrUpdateAsync(baseEntities, isModified);

    public async Task<bool> Save(T baseEntity, InsertUpdateEnum isModified = InsertUpdateEnum.Update,
        Func<T, Task<bool>>? validateSave = null)
    {
       return (validateSave == null
            ? await ValidateSave(baseEntity).ConfigureAwait(false)
            : await ValidateSave(baseEntity, validateSave).ConfigureAwait(false))
            ? await InsertOrUpdateAsync(baseEntity, isModified).ConfigureAwait(false)
            : await Task.FromResult(false).ConfigureAwait(false);
    }

    public async Task<IList<T>> SelectFullActiveList(CancellationToken token = default) =>
        await DbSetEntity.AsQueryable().ToListAsync(token).ConfigureAwait(false);

    public virtual Task<bool> ValidateDelete(T baseEntity) => Task.FromResult(true);

    public async Task<bool> ValidateDelete(T baseEntity, Func<T, Task<bool>>? validateDelete) =>
        validateDelete != null
            ? await validateDelete(baseEntity).ConfigureAwait(false)
            : await Task.FromResult(true).ConfigureAwait(false);

    public async Task<bool> ValidateSave(T baseEntity, Func<T, Task<bool>>? validateSave) =>
        validateSave != null
            ? await validateSave(baseEntity).ConfigureAwait(false)
            : await Task.FromResult(true).ConfigureAwait(false);

    public virtual async Task<bool> ValidateSave(T baseEntity)
    {
        if (await baseEntity.IsValidAsync().ConfigureAwait(false)) return true;
        
        throw new RequestException(baseEntity.AttributeError!, ResponseCodeEnum.ValidationError);
    }

    public IQueryable<T> DbEntity() => DbSetEntity.AsQueryable().AsNoTracking();
    

    public IQueryable<T> WhereCondition(Expression<Func<T, bool>>? condLambda,
        Expression<Func<T, object>>? orderBy = null, string? include = null,
        Expression<Func<T, object>>? orderByDesc = null)
    {

        IQueryable<T> cond = DbSetEntity.AsQueryable().AsNoTracking();

        if (!string.IsNullOrWhiteSpace(include)) include.Split(',').ToList().ForEach(c => cond = cond.Include(c));

        if (condLambda != null) cond = cond.Where(condLambda);

        if (orderBy != null) cond = cond.OrderBy(orderBy);

        if (orderByDesc != null) cond = cond.OrderByDescending(orderByDesc);

        return cond;
    }

    public IQueryable<T> WhereCondition(Expression<Func<T, bool>>? condLambda,
        Expression<Func<T, object>>? orderBy, IEnumerable<string>? include,
        Expression<Func<T, object>>? orderByDesc = null)
    {
        IQueryable<T> cond = DbSetEntity.AsQueryable().AsNoTracking();

        include?.ToList().ForEach(c => cond = cond.Include(c));

        if (condLambda != null) cond = cond.Where(condLambda);

        if (orderBy != null) cond = cond.OrderBy(orderBy);

        if (orderByDesc != null) cond = cond.OrderByDescending(orderByDesc);

        return cond;
    }

    public Task<List<T>> GetListAsync(Expression<Func<T, bool>> condLambda, Expression<Func<T, object>> orderBy,
        IEnumerable<string> include, Expression<Func<T, object>>? orderByDesc = null, int takeLines = 0, CancellationToken token = default)
    {
        return Task.Run(() =>
        {
            IQueryable<T> cond = DbSetEntity.AsQueryable().AsNoTracking();

            include?.ToList().ForEach(c => cond = cond.Include(c));

            if (condLambda != null) cond = cond.Where(condLambda);

            if (orderBy != null) cond = cond.OrderBy(orderBy);

            if (orderByDesc != null) cond = cond.OrderByDescending(orderByDesc);

            return (takeLines == 0)
                ? cond.ToListAsync(cancellationToken: token)
                : cond.Take(takeLines).ToListAsync(cancellationToken: token);
        }, token);
    }

    public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>>? condLambda = null,
        CancellationToken token = default)
    {
        if (condLambda != null)
            return await DbSetEntity.AsNoTracking().FirstOrDefaultAsync(condLambda, token).ConfigureAwait(false);
        return await DbSetEntity.AsNoTracking().FirstOrDefaultAsync(token).ConfigureAwait(false);
    }

    public async Task<int> ElementCountAsync(Expression<Func<T, bool>>? condLambda = null)
    {
        if (condLambda != null)
            return await DbSetEntity.AsNoTracking().CountAsync(condLambda).ConfigureAwait(false);
        return await DbSetEntity.AsNoTracking().CountAsync().ConfigureAwait(false);
    }

    public async Task<bool> AnyAsync(Expression<Func<T, bool>>? condLambda = null)
    {
        if (condLambda != null) return await DbSetEntity.AsNoTracking().AnyAsync(condLambda).ConfigureAwait(false);
        return await DbSetEntity.AsNoTracking().AnyAsync().ConfigureAwait(false);
    }

    public IQueryable<T> WhereCondition(Expression<Func<T, bool>>? condLambda,
        Expression<Func<T, object>>? orderBy, ICollection<Expression<Func<T, object>>>? include,
        Expression<Func<T, object>>? orderByDesc = null)
    {
        IQueryable<T> cond = DbSetEntity.AsQueryable().AsNoTracking();
        include?.ToList().ForEach(c => cond = cond.Include(c));
        if (condLambda != null) cond = cond.Where(condLambda);
        if (orderBy != null) cond = cond.OrderBy(orderBy);
        if (orderByDesc != null) cond = cond.OrderByDescending(orderByDesc);
        return cond;
    }

    public void Reload(T refreshItem) => AppContext.Entry(refreshItem).Reload();

    public async Task<PagingValue<T>?> Paging(int pageIndex, int pageSize, IQueryable<T>? sourceEntities = null,
        int indexFrom = 0, CancellationToken cancellationToken = default)
    {
        if (indexFrom > pageIndex) return null;
        IQueryable<T> source = sourceEntities ?? DbSetEntity.AsQueryable().AsNoTracking();
        int countIfItem = await source.CountAsync(cancellationToken).ConfigureAwait(false);
        List<T> items = await source.Skip((pageIndex - indexFrom) * pageSize)
            .Take(pageSize).ToListAsync(cancellationToken).ConfigureAwait(false);

        PagingValue<T> paging = new()
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
            IndexFrom = indexFrom,
            TotalCount = countIfItem,
            Items = items,
            TotalPages = (int) Math.Ceiling(countIfItem / (double) pageSize)
        };

        return paging;
    }

    public IQueryable<T>? PagingList(int pageIndex, int pageSize, IQueryable<T>? sourceEntities = null,
        int indexFrom = 0)
    {
        if (indexFrom > pageIndex) return null;
        IQueryable<T> source = sourceEntities?.AsNoTracking() ?? DbSetEntity.AsNoTracking().AsQueryable();
        return source.Skip((pageIndex - indexFrom) * pageSize).Take(pageSize);
    }


    public IQueryable<T>? PagingList(int pageIndex, int pageSize, Expression<Func<T, bool>>? condLambda = null,
        int indexFrom = 0)
    {
        if (indexFrom > pageIndex) return null;
        IQueryable<T> source = (condLambda != null)
            ? WhereCondition(condLambda)
            : DbSetEntity.AsNoTracking().AsQueryable();
        return source.Skip((pageIndex - indexFrom) * pageSize).Take(pageSize);
    }
}