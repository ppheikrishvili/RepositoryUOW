using System.Collections.Concurrent;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using RepositoryUOW.Data.Exceptions;
using RepositoryUOWDomain.Entities.Base;
using RepositoryUOWDomain.Interface;
using RepositoryUOWDomain.Shared.Common;
using RepositoryUOWDomain.Shared.Enums;
using RepositoryUOWDomain.Shared.Extensions;
using RepositoryUOWDomain.ValueObject;

namespace RepositoryUOW.Data.Repositories;

public class RepositoryUow<T> : IRepositoryUow<T> where T : BaseEntity
{
    readonly SemaphoreSlim _semaphore = new(1, 1);
    public string? ErrorMessage = "";
    public IFactoryUow FactoryUowRepo { get; set; }

    protected delegate ResponseResult<TT> ExecuteResponse<TT>(TT res);

    protected delegate TT ExecuteAction<out TT>();
    
    protected ResponseResult<TT> ExecuteAsync<TT>(ExecuteResponse<TT> response, ExecuteAction<TT> action)
    {
        try
        {
            return response(action());
        }
        catch (Exception ex)
        {
            return ex switch
            {
                { } ExcType when ExcType is RequestException ReqExcType => new ResponseResult<TT>(
                    ReqExcType.Message, ReqExcType.ResponseCode, default!),
                _ => new ResponseResult<TT>(ex.ToErrorStr().Result, ResponseCodeEnum.InternalSystemError, default!)
            };
        }
    }

    public RepositoryUow()
    {
        FactoryUowRepo = new FactoryUow();
    }

    public RepositoryUow(AppDbContext appContext)
    {
        FactoryUowRepo = new FactoryUow(appContext);
    }

    public IQueryable<T> WhereCondition(Expression<Func<T, bool>> conditionLambda,
        Expression<Func<T, object>>? orderBy = null, string? include = null)
    {
        try
        {
            return FactoryUowRepo.Repository<T>().WhereCondition(conditionLambda, orderBy, include);
        }
        catch (Exception ex)
        {
            throw new RequestException(ResponseCodeEnum.IncorrectParameters, ex);
        }
    }

    public IQueryable<T> DbEntity() => FactoryUowRepo.Repository<T>().DbEntity();

    public IQueryable<T> WhereCondition(Expression<Func<T, bool>> conditionLambda, Expression<Func<T, object>> orderBy,
        IEnumerable<string> include)
    {
        try
        {
            return FactoryUowRepo.Repository<T>().WhereCondition(conditionLambda, orderBy, include);
        }
        catch (Exception ex)
        {
            throw new RequestException(ResponseCodeEnum.IncorrectParameters, ex);
        }
    }

    public async Task<IEnumerable<T>> AllItems(CancellationToken cancellationToken = default)
    {
        try
        {
            return await FactoryUowRepo.Repository<T>().SelectFullActiveList(cancellationToken).ConfigureAwait(false);
        }

        catch (Exception ex)
        {
            throw new RequestException(ResponseCodeEnum.InternalSystemError, ex);
        }
    }

    public ResponseResult<IList<T>> AllItemsAsync(CancellationToken cancellationToken = default) =>
         ExecuteAsync(b => new ResponseResult<IList<T>>(null, ResponseCodeEnum.Success, b),
             () => FactoryUowRepo.Repository<T>().SelectFullActiveList(cancellationToken).Result);

    public async Task<PagingValue<T>?> PagingAsync(int pageIndex, int pageSize, IQueryable<T>? sourceEntities = null, int indexFrom = 0,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await FactoryUowRepo.Repository<T>().Paging(pageIndex, pageSize, sourceEntities, indexFrom,
                cancellationToken).ConfigureAwait(false);
        }

        catch (Exception ex)
        {
            throw new RequestException(ResponseCodeEnum.InternalSystemError, ex);
        }
    }

    public IQueryable<T>? PagingList(int pageIndex, int pageSize, IQueryable<T>? sourceEntities = null, int indexFrom = 0)
    {
        try
        {
            return FactoryUowRepo.Repository<T>().PagingList(pageIndex, pageSize, sourceEntities, indexFrom);
        }
        catch (Exception ex)
        {
            throw new RequestException(ResponseCodeEnum.InternalSystemError, ex);
        }
    }


    public async Task<bool> DeleteRangeAsync(Expression<Func<T, bool>> condLambda, CancellationToken token = default)
    {
        try
        {
            await FactoryUowRepo.Repository<T>().DeleteRange(condLambda, token).ConfigureAwait(false);
            return await FactoryUowRepo.CommitAsync(token).ConfigureAwait(false) > 0;
        }

        catch (Exception ex)
        {
            throw new RequestException(ResponseCodeEnum.InternalSystemError, ex);
        } 
    }


    public async Task<bool> DeleteRangeAsync(IEnumerable<T> baseEntities, CancellationToken token = default)
    {
        try
        {
            await FactoryUowRepo.Repository<T>().DeleteRange(baseEntities, token).ConfigureAwait(false);
            return await FactoryUowRepo.CommitAsync(token).ConfigureAwait(false) > 0;
        }
        catch (Exception ex)
        {
            switch (ex)
            {
                case { } ReqExcType when ReqExcType is RequestException:
                    throw;
                default:
                    throw new RequestException( ResponseCodeEnum.InternalSystemError, ex);
            }
        }
    }

    public async Task<List<T>[]?> ReadParallel(Expression<Func<T, bool>> whereExpression, CancellationToken token = default)
    {
        try
        {
            int recCount = await FactoryUowRepo.Repository<T>().CountAsync(whereExpression).ConfigureAwait(false);
            int processorUsed = 12;
            int chunkCount = (processorUsed > recCount) ? 1 : Convert.ToInt32((decimal)recCount / processorUsed);
            if (recCount % processorUsed > 0) chunkCount++;
            var tasks = Enumerable.Range(1, processorUsed).AsParallel().Select(async chunks =>
            {
                await using var tmpContext = new AppDbContext();
                tmpContext.Database.SetCommandTimeout(3200);
                return await tmpContext.Set<T>().AsNoTracking().Skip((chunks - 1) * chunkCount)
                    .Take(chunkCount).ToListAsync(token).ConfigureAwait(false);
            }).ToList();

            return await Task.WhenAll(tasks).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            throw new RequestException(ResponseCodeEnum.InternalSystemError, ex);
        }

    }

    public async Task<T?> ElementByIdAsync(object id)
    {
        try
        {
            return await FactoryUowRepo.Repository<T>().ElementByIdAsync(id).ConfigureAwait(false);
        }

        catch (Exception ex)
        {
            throw new RequestException(ResponseCodeEnum.InternalSystemError, ex);
        }

    }

    async Task<T?> InsertOrUpdate(T baseEntityClass, InsertUpdateEnum isModified = InsertUpdateEnum.Update, CancellationToken token = default)
    {
        try
        {
            await _semaphore.WaitAsync(token).ConfigureAwait(false);
            await FactoryUowRepo.Repository<T>().InsertOrUpdateAsync(baseEntityClass, isModified).ConfigureAwait(false);
            return await FactoryUowRepo.CommitAsync(token).ConfigureAwait(false) > 0 ? baseEntityClass : null;
        }
        catch (Exception ex)
        { 
            switch (ex)
            {
                case { } ReqExcType when ReqExcType is RequestException:
                    throw;
                default:
                    throw new RequestException(ResponseCodeEnum.InternalSystemError, ex);
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<T?> SaveAsync(T savedObject, InsertUpdateEnum isModified = InsertUpdateEnum.Update,
        CancellationToken token = default)
    {
        try
        {
            if (await FactoryUowRepo.Repository<T>().ValidateSave(savedObject).ConfigureAwait(false))
                return await InsertOrUpdate(savedObject, isModified, token).ConfigureAwait(false);
            return null;
        }
        catch (Exception ex)
        {
            switch (ex)
            {
                case { } ReqExcType when ReqExcType is RequestException:
                    throw;
                default:
                    throw new RequestException(ResponseCodeEnum.InternalSystemError, ex);
            }
        }
    }


    public async Task<bool> SaveRangeAsync(IList<T> savedObjects, InsertUpdateEnum isModified = InsertUpdateEnum.Update,
        CancellationToken token = default)
    {
        if (savedObjects is not {Count: > 0}) return false;
        ConcurrentBagAsync<T> savedObjectsConcurrence = new(savedObjects);
        return await SaveRangeAsync(savedObjectsConcurrence, isModified, token).ConfigureAwait(false);
    }


    public async Task<bool> SaveRangeAsync(ConcurrentBagAsync<T> savedObjects, InsertUpdateEnum isModified = InsertUpdateEnum.Update,
        CancellationToken token = default)
    {
        try
        {
            if (savedObjects is not { Count: > 0 }) return false;

            int processorUsed = Convert.ToInt32(Math.Ceiling(Environment.ProcessorCount * 0.75 * 2.0));
            int chunkCount = (processorUsed > savedObjects.Count) ? 1 : savedObjects.Count / processorUsed;

            var tasks = Partitioner.Create(savedObjects.ToList(), true).AsParallel()
                .WithDegreeOfParallelism(processorUsed).Chunk(chunkCount)
                .Select(async chunks =>
                {
                    //await throttler.WaitAsync(token);
                    try
                    {
                        await using FactoryUow tmpContext = new ();
                        IBase<T> baseRep = tmpContext.Repository<T>();
                        if (await baseRep.SaveRangeAsync(chunks, isModified, token).ConfigureAwait(false))
                            await tmpContext.CommitAsync(token).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        ErrorMessage = await ex.ToErrorStr(token: token).ConfigureAwait(false);
                    }
                    //finally
                    //{
                    //    throttler.Release();
                    //}
                });

            await Task.WhenAll(tasks).ConfigureAwait(false);

            return true;
        }
        catch (Exception ex)
        {
            throw new RequestException(ResponseCodeEnum.InternalSystemError, ex);
        }
    }

    public async Task<bool> Save(ICollection<T> savedObjects,
        InsertUpdateEnum isModified = InsertUpdateEnum.Update, CancellationToken token = default)
    {
        try
        {
            if (savedObjects is not { Count: > 0 }) return false;

            var baseRep = FactoryUowRepo.Repository<T>();

            foreach (T[] chunk in savedObjects.Chunk(1000).AsParallel().WithDegreeOfParallelism(Convert.ToInt32(Math.Ceiling((Environment.ProcessorCount * 0.75) * 2.0))))
            {
                await baseRep.InsertOrUpdateAsync(chunk, isModified).ConfigureAwait(false);
            }
            return await FactoryUowRepo.CommitAsync(token).ConfigureAwait(false) > 0;
        }
        catch (Exception ex)
        {
            throw new RequestException(ResponseCodeEnum.InternalSystemError, ex);
        }
    }
    
    public async Task<bool> DeleteAsync(T deletedObject, CancellationToken token = default)
    {
        try
        {
            IBase<T> res = FactoryUowRepo.Repository<T>();
            return await res.ValidateDelete(deletedObject).ConfigureAwait(false) && await res.Delete(deletedObject).ConfigureAwait(false)
                                                           && await FactoryUowRepo.CommitAsync(token).ConfigureAwait(false) > 0;
        }
        catch (Exception ex)
        {
            throw new RequestException(ResponseCodeEnum.InternalSystemError, ex);
        }
    }
}