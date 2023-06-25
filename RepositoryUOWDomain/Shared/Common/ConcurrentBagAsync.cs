using System.Collections.Concurrent;
using RepositoryUOWDomain.Interface;

namespace RepositoryUOWDomain.Shared.Common;

public class ConcurrentBagAsync<T> : ConcurrentBag<T>, IConcurrentBagAsync<T>
{
    public ConcurrentBagAsync() { }
    public ConcurrentBagAsync(IEnumerable<T> collect) : base(collect) { } 

    public async Task Add(T elem, CancellationToken token = default) =>
        await Task.Run(() => { if (elem != null) base.Add(elem); }, token).ConfigureAwait(false);

    public async Task AddRange(IEnumerable<T> elemCollect, CancellationToken token = default) =>
        await Task.Run(() => Partitioner.Create(elemCollect.ToList(), true).AsParallel()
            .ForAll(item => base.Add(item)), token).ConfigureAwait(false);
}